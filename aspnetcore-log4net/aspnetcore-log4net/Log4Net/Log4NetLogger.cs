namespace aspnetcore_log4net.Log4Net
{
  using log4net;
  using log4net.Config;
  using log4net.Repository;
  using log4net.Repository.Hierarchy;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using System.Xml;

  public class Log4NetLogger : ILogger
  {
    private readonly ILog log;

    public Log4NetLogger( string name, XmlElement configElement )
    {
      ILoggerRepository loggerRepository =
        LogManager.CreateRepository( Assembly.GetEntryAssembly(), typeof( Hierarchy ) );

      log = LogManager.GetLogger( loggerRepository.Name, name );

      XmlConfigurator.Configure( loggerRepository, configElement );
    }

    // TODO: Add support for scope
    public IDisposable BeginScope<TState>( TState state ) => null;

    /// <summary>
    /// Map Microsoft.Extensions.Logging.LogLevel to log4net.ILog enabled properties.
    /// </summary>
    private static IDictionary<LogLevel, Func<ILog, bool>> isEnabledFunctions =
      new Dictionary<LogLevel, Func<ILog, bool>>
      {
        { LogLevel.Trace,       log => log.IsDebugEnabled },
        { LogLevel.Debug,       log => log.IsDebugEnabled },
        { LogLevel.Warning,     log => log.IsWarnEnabled },
        { LogLevel.Error,       log => log.IsErrorEnabled },
        { LogLevel.Information, log => log.IsInfoEnabled },
        { LogLevel.Critical,    log => log.IsFatalEnabled }
      };

    public bool IsEnabled( LogLevel logLevel )
    {
      if ( !isEnabledFunctions.TryGetValue( logLevel, out Func<ILog, bool> isEnabled ) )
      {
        throw new ArgumentOutOfRangeException( nameof( logLevel ), $"Unsupported logLevel '{logLevel}'." );
      }

      return isEnabled( log );
    }

    /// <summary>
    /// Map Microsoft.Extensions.Logging.LogLevel to log4net logging action.
    /// </summary>
    private static IDictionary<LogLevel, Action<ILog, string>> logActions =
      new Dictionary<LogLevel, Action<ILog, string>>
      {
        { LogLevel.Trace,       ( log, message ) => log.Debug( message ) },
        { LogLevel.Debug,       ( log, message ) => log.Debug( message ) },
        { LogLevel.Warning,     ( log, message ) => log.Warn( message ) },
        { LogLevel.Error,       ( log, message ) => log.Error( message ) },
        { LogLevel.Information, ( log, message ) => log.Info( message ) },
        { LogLevel.Critical,    ( log, message ) => log.Fatal( message ) }
      };

    public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter )
    {
      ValidateIsNotNull( formatter, nameof( formatter ) );

      bool isFiltered = Log4NetFilters.IsFiltered( state, logLevel );

      if ( IsEnabled( logLevel ) && !isFiltered )
      {
        string message = FormatMessage( state, exception, formatter );

        if ( !string.IsNullOrEmpty( message ) || exception != null )
        {
          ExecuteLogAction( logLevel, message );
        }
      }
    }

    private void ExecuteLogAction( LogLevel logLevel, string message )
    {
      if ( logActions.TryGetValue( logLevel, out Action<ILog, string> logAction ) )
      {
        logAction( log, message );
      }
      else
      {
        log.Warn( $"{nameof( Log )} called with invalid log level '{logLevel}': logging as Info." );
        log.Info( message );
      }
    }

    private static string FormatMessage<TState>( TState state, Exception exception, Func<TState, Exception, string> formatter ) => 
      formatter != null
        ? formatter( state, exception )
        : null;

    private static void ValidateIsNotNull( object parameter, string name )
    {
      if ( parameter == null )
      {
        throw new ArgumentNullException( name );
      }
    }
  }
}
