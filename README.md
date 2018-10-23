# aspnetcore-log4net
An ASP.NET Core logging provider for log4net with support for semantic logging suitable for operations.

# Purpose
Investigate the ASP.NET Core 2.0 logging mechanism through the process creating a log4net logging provider. 
This project aims to explore the ASP.NET Core logging customization and extensibility.
The concepts should generalize to creating logging providers for other logging mechanisms.

Compare with `[Lock2018]` section 17.3.2 "Replacing the default ILoggerFactory with Serilog."

**Choosing log4net.** 
Log4net might be considered a little dated as a logging solution for .NET, however it remains
relevant as it is widely used and consumed, and it is a known quantity for operations. 
For example, the Logly log aggregator can consume logs from log4net.

**Value of semantic logging.**
Semantic logging adds type safety to logging APIs, making them easier to program against.
It also enables logs containing structured data (e.g., lines of CSV records in this sample)
which are easy for operations to consume for health monitoring, notifications, analysis, reporting, and so on.

# Solution tour
## Dependencies
* The project has added the **log4net NuGet package**.

## Implementing ILogger support for log4net
* **Log4Net** is a project folder/namespace which contains:
  * **Log4NetExtensions** has extension methods to add a logger provider for log4net to the logger factory.
  * **Log4NetLoggingProvider** is the logging provider for log4net.
It manages a thread-safe collection of Log4NetLoggers, indexed by category name.
(This sample doesn't use of category names.)
  * **Log4NetLogger** is the intermediary that maps between Microsoft.Extensions.Logging constructs
and their cognates in log4net.
  * **Log4NetFilters** is a custom filtering mechanism to suppress log requests for specific sources
  (a source is the initial part of the namespace of the state type) and specific log levels.
  This is used to suppress the default ASP.NET logging that come from the "Microsoft" source with a level of Infomation.
  * **LogMessage** provides a layer of type-safe semantic logging, in this case by formatting the message into a CSV string.
  * **RollingFileWithOneHeaderAppender** specializes log4net's RollingFileAppender so that the header 
  is only written once per log file.

## Using ILogger support for log4net
* The **log4net.config** file contains two appenders: one appends logs to the console for interactive purpose, the other uses the RollingFileWithOneHeaderAppender to write logs to a sequence of files to C:\Temp, it is configured to create one per hour or per 50KB logged, whichever comes first.
* The project's **Startup** class:
  * Alters the signature of the **Configure** method to include an ILoggerFactory parameter:
```
public void Configure( IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
```    
  * In the **Configure** method, log4net is added to the logger factory and a filter is set to filter out Microsoft Informational logs.
```  
    loggerFactory.AddLog4Net();
    Log4NetFilters.Set(               // filter out the standard log messages with source "Microsoft"
      source: "Microsoft", 
      filteredLogLevel: LogLevel.Information );
```
* The **HomeController** adds a constructor to inject an ILogger dependency.
The three controller actions -- Index, About, and Contact -- each invokes a different semantic logging methods.

# Log consumption
The collection of files generated by the log4net file appender can be easily consumed using a utility like **Log Parser Studio** (free Microsoft tool, download https://gallery.technet.microsoft.com/office/Log-Parser-Studio-cd458765), and running a structured query over the logs, for example:
```
SELECT 
  Filename, RowNumber, date, level, 
  controller, action, start, duration, 
  exceptionType, exceptionMessage, stackTrace, 
  message
FROM '[LOGFILEPATH]'
WHERE duration > '0.03'
```

# Acknowledgements
This project has drawn on references (see below), Microsoft docs, ASP.NET Core open source code, and StackOverflow posts, 
for all of which the author is grateful. 

# References and resources
* `[Blum2016]` Blumhardt, Nicholas, "Smart Logging Middleware for ASP.NET Core", getseq.net blog post, 12/7/2016, https://blog.getseq.net/smart-logging-middleware-for-asp-net-core/. 
  * Nicholas Blumhardt is the instigator of Autofaq, Serilog, and other projects, see https://nblumhardt.com/about/
* `[Lock2018]` Lock, Andrew, ASP.NET Core In Action, Manning Publications, 2018. https://www.manning.com/books/asp-net-core-in-action
  * In particular, see section 17.3.2 "Replacing the default ILoggerFactory with Serilog."
* `[MicrosoftDocs]` Microsoft | Docs | ASP.NET Core 2.1 | Logging in ASP.NET Core, last updated 10/10/2018, https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1 
* `[Para2017]` - Parameswaran, Anuraj, "How to use Log4Net with ASP.NET Core for logging", dotnetthoughts.net blog post, 2/18/2017, https://dotnetthoughts.net/how-to-use-log4net-with-aspnetcore-for-logging/
