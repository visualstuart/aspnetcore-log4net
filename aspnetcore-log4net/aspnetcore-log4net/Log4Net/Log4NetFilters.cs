namespace aspnetcore_log4net.Log4Net
{
  using Microsoft.Extensions.Logging;
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Filter out specific log levels for specific sources from being logged.
  /// </summary>
  public static class Log4NetFilters
  {
    private static IDictionary<string, List<LogLevel>> sourceFilters =
      new Dictionary<string, List<LogLevel>>();

    /// <summary>
    /// Set a filter for a log level for a source.
    /// </summary>
    public static void Set( string source, LogLevel filteredLogLevel ) =>
      sourceFilters[ source ] = new List<LogLevel> { filteredLogLevel };

    /// <summary>
    /// Set a filter for a collection of log levels for a source.
    /// </summary>
    public static void Set( string source, IEnumerable<LogLevel> filteredLogLevels ) =>
      sourceFilters[ source ] = new List<LogLevel>( filteredLogLevels );

    /// <summary>
    /// Clear all log level filters for a source.
    /// </summary>
    /// <param name="source"></param>
    public static void Clear( string source ) => 
      sourceFilters.Remove( source );

    private const string customLogStateType = "Microsoft.Extensions.Logging.Internal.FormattedLogValues";

    /// <summary>
    /// Check if state should be filtered, i.e., not logged.
    /// </summary>
    public static bool IsFiltered<TState>( TState state, LogLevel logLevel )
    {
      Type stateType = state.GetType();

      // Don't filter out custom log states, which might otherwise be filtered as the Microsoft source.
      if ( stateType.FullName == customLogStateType )
        return false;

      string source = GetSource( stateType );
      return 
        !string.IsNullOrEmpty( source ) && // don't filter on 
        sourceFilters.TryGetValue( source, out List<LogLevel> filteredLogLevels )
          ? filteredLogLevels.Contains( logLevel )
          : false;
    }

    private static string GetSource( Type type )
    {
      if ( type.FullName == customLogStateType )
        return string.Empty;

      string stateNamespace = type.Namespace;
      int delimiterIndex = stateNamespace.IndexOf( '.' );

      return delimiterIndex != -1
        ? stateNamespace.Substring( 0, delimiterIndex )
        : stateNamespace;
    }
  }
}
