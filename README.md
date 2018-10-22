# aspnetcore-log4net
ASP.NET Core logging provider using log4net with support for semantic logging.

## Purpose
Investigate the ASP.NET Core 2.0 logging mechanism by creating a log4net logging provider.

## Why log4net?
Log4net is perhaps a little dated as a .NET logging solutions, however it is widely used and consumed,
making it a known quantity for operations. For example, the Logly log aggregator can consume logs from log4net.

## Why semantic logging support?
Semantic logging enables writing structured data to logs (e.g., CSV records)
which are better to consume for operational purposes such as health monitoring, notifications, and reporting.
