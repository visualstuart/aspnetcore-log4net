using aspnetcore_log4net.Log4Net;
using aspnetcore_log4net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace aspnetcore_log4net.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger log;

    public HomeController( ILogger<HomeController> log ) =>
      this.log = log;

    public IActionResult Index()
    {
      log.LogInformation(
        LogMessage.ControllerActionMessage(
          nameof( HomeController ),
          nameof( Index ),
          "Executing Index action." ) );

      return View();  
    }

    public IActionResult About()
    {
      DateTime start = DateTime.Now;
      Stopwatch stopwatch = Stopwatch.StartNew();

      ViewData[ "Message" ] = "Your application description page.";

      TimeSpan duration = stopwatch.Elapsed;
      log.LogInformation(
        LogMessage.ControllerActionMessage(
          nameof( HomeController ),
          nameof( About ),
          start,
          duration,
          "Message with start and duration." ) );

      return View();
    }

    public IActionResult Contact()
    {
      try
      {
        throw new InvalidOperationException( "Sample exception." );
      }
      catch ( Exception exception )
      {
        log.LogWarning(
          LogMessage.ControllerActionMessage(
            nameof( HomeController ),
            nameof( Contact ),
            "Sample message.",
            exception ) );
      }

      ViewData[ "Message" ] = "Your contact page.";

      return View();
    }

    public IActionResult Error()
    {
      return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
    }
  }
}
