namespace aspnetcore_log4net.Log4Net
{
  using Microsoft.Extensions.Logging;
  using System;
  using System.Collections.Concurrent;
  using System.Xml;

  public class Log4NetLoggerProvider : ILoggerProvider
  {
    private readonly string configFilename;

    private readonly ConcurrentDictionary<string, Log4NetLogger> loggers =
        new ConcurrentDictionary<string, Log4NetLogger>();

    public Log4NetLoggerProvider( string configFilename )
    {
      this.configFilename = configFilename;
    }

    public ILogger CreateLogger( string categoryName ) => 
      loggers.GetOrAdd( 
        categoryName, 
        name => new Log4NetLogger( name, GetConfigDocumentElement( configFilename ) ) );

    private static XmlElement GetConfigDocumentElement( string configFilename )
    {
      XmlDocument config = new XmlDocument();
      config.Load( configFilename );
      return config.DocumentElement;
    }

    // Basic Dispose Pattern

    public void Dispose()
    {
      Dispose( true );
      GC.SuppressFinalize( this );
    }

    private bool disposedValue = false;

    protected virtual void Dispose( bool disposing )
    {
      if ( !disposedValue )
      {
        if ( disposing )
        {
          loggers.Clear();
        }

        disposedValue = true;
      }
    }
  }
}
