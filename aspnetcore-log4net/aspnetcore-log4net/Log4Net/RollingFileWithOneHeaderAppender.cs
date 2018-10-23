namespace aspnetcore_log4net.Log4Net
{
  using log4net.Appender;

  public class RollingFileWithOneHeaderAppender : RollingFileAppender
  {
    protected override void WriteHeader()
    {
      if ( LockingModel.AcquireLock().Length == 0 )
      {
        base.WriteHeader();
      }
    }
  }
}
