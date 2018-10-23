# aspnetcore-log4net
An ASP.NET Core logging provider for log4net
 * Support for semantic logging.

## Purpose
Investigate the ASP.NET Core 2.0 logging mechanism by creating a log4net logging provider. 
This project aims to explore the ASP.NET Core logging customization and extensibility.
The concepts should generalize to creating logging providers for other logging mechanisms. 

### Choosing log4net
Log4net might be considered a little dated as a logging solution for .NET, however it remains
relevant as it is widely used and consumed, and it is a known quantity for operations. 
For example, the Logly log aggregator can consume logs from log4net.

### Value of semantic logging
Semantic logging adds type safety to logging APIs, making them easier to program against.
It also enables logs containing structured data (e.g., lines of CSV records in this sample)
which are easy for operations to consume for health monitoring, notifications, analysis, reporting, and so on.

## Highlights
* The project has added the **log4net NuGet package**.
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
* The **log4net.config** file contains two appenders: one appends logs to the console for interactive purpose, the other uses the RollingFileWithOneHeaderAppender to write logs to a sequence of files configured to create one per hour or per 50KB logged, whichever comes first.
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
* The HomeController invokes three different semantic logging methods, one per controller action.
