namespace aspnetcore_log4net.Log4Net
{
  using Microsoft.Extensions.Logging;

  public static class Log4NetExtensions
  {
    private const string defaultConfigFileName = "log4net.config";

    public static ILoggerFactory AddLog4Net( this ILoggerFactory factory, string configFilename )
    {
      factory.AddProvider( new Log4NetLoggerProvider( configFilename ) );
      return factory;
    }

    public static ILoggerFactory AddLog4Net( this ILoggerFactory factory ) => 
      factory.AddLog4Net( defaultConfigFileName );
  }
}
