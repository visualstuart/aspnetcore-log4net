namespace aspnetcore_log4net.Log4Net
{
  using System;
  using System.Globalization;

  public static class LogMessage
  {
    public static string ControllerActionMessage( string controller, string action, string message ) =>
      GetMessage(
        controller: controller,
        action: action,
        message: message );

    public static string ControllerActionMessage( string controller, string action, string message, Exception exception ) =>
      GetMessage(
        controller: controller,
        action: action,
        message: message,
        exception: exception );

    public static string ControllerActionMessage( string controller, string action, DateTime start, TimeSpan duration, string message ) =>
      GetMessage(
        controller: controller,
        action: action,
        start: start,
        duration: duration,
        message: message );

    // Internal details

    private static string GetMessage(
      string controller = null,
      string action = null,
      DateTime? start = null,
      TimeSpan? duration = null,
      Exception exception = null,
      string message = null )
      {
        string[] values =
          new string[]
          {
            controller,
            action,
            start?.ToString( "o", CultureInfo.InvariantCulture ),
            duration?.TotalMilliseconds.ToString( CultureInfo.InvariantCulture ),
            exception != null ? $"{exception.GetType().Name} ({exception.GetType().FullName})" : null,
            exception != null ? CommaSeparatedValues.Escape( exception.Message ) : null,
            exception != null ? CommaSeparatedValues.Escape( exception.StackTrace ) : null,
            CommaSeparatedValues.Escape( message )
          };
        return string.Join( ", ", values );
    }

  }
}
