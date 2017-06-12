# Codestellation.Statsd _Simple Statsd Client for .Net Applications_

Codestellation.Statsd provides a blazingly fast, almost garbageless client for [Etsy Statsd Implementation](https://github.com/etsy/statsd). Currently it supports three kinds of metrics:
* Count
* Gauge
* Timing

# Installation

Development builds of **Statsd** are available from [myget.org](https://www.myget.org/feed/Packages/codestellation). Install it using the NuGet Package Console window:

```
PM> Install-Package Codestellation.Statsd -Source https://www.myget.org/F/codestellation/api/v2/package
```

Stable builds are available from [nuget.org](https://nuget.org)

```
PM> Install-Package Codestellation.Statsd
```

# Usage

he main interface of  `Codestellation.Statsd` is `IStatsdClient` which contains a bunch of methods to send count, gauge, and timing metrics to a statsd server. It has two implementations: 
* `StatsdClient` - simple synchronous client which is not thread safe. If you want to use it in a multithreaded environment consider instace per thread scenario.
* `BackgroundStatsdClient` - asynchronous client which uses a background task to send metrics. It's absolutely thread safe. 

# Configuration
## Configuration by code
An instance of `IStatsdClient` could be configured manually
```
var settings = new UdpChannelSettings
{
    Host = "my-host",
    IgnoreSocketExceptions = true,
    Port = 8085
};
var channel = new UdpChannel(settings);
var client = new StatsdClient(channel, prefix:"ideal");

var count = new Count("bananas", 10);
client.LogCount(count);

// or even simplier using extension methods:

client.LogCount("oranges", 11);

```
## Configuration by URI

```
var uri = "udp://my-host:8085?prefix=the.service&background=false&ignore_exceptions=true";

IStatsdClient actual = BuildStatsd.From(uri);
```
### URI Parameters
* `prefix` - a prefix which will be applied before every metric name. Default value is `string.Empty`
* `background` - which kind of client should be used - `BackgroundStatsdClient` or synchronous `StatsdClient`. Default value is true.  Note  `udp://my-host:8085?background=true` and `udp://my-host:8085?background` are treated equally. 
* `ignore_exceptions` - ignore any `SocketException` when sending metrics. Default value is false. `udp://my-host:8085?ignore_exceptions=true` and `udp://my-host:8085?ignore_exceptions` are treated equally.


## Sending metrics

The usage is as simple as:
```
var client = BuildStatsd.From("udp://my-host:8085");
// send metrics using IStatsdClient interface
client.LogCount(new Count("bananas", 10));
client.LogGauge(new Gauge("watermark", 42));
client.LogTiming(new Timing("processing.time", 983));

// or even simplier using extension methods:
client.LogCount("oranges", 11);
client.LogGauge("watermark", 42);
client.LogTiming("processing.time", 983);
```

## Static statsd client usage

It's possible to use static instance of  `IStatsdClient` through static class `GlobalStatsd`. Before using it call `Configure(Uri)` method. Due to thread safety reasons it's recommended to use static `GlobalStatsd` with background client only.

```
//Configure GlobalStatsd instance
var uri = new Uri("udp://my-host:8085?prefix=the.service&ignore_exceptions");
GlobalStatsd.Configure(uri);

// After configuration either send metrics using static methods
GlobalStatsd.LogCount("count", 1);
GlobalStatsd.LogGauge("gauge", 2);
GlobalStatsd.LogTiming("timing", 42);

// Or access IStatsdClient to send metrics using it's instance and extension methods
var client = GlobalStatsd.Client;
```

## Timing measure

.NET API contains `System.Diagnostics.Stopwatch` class which could be used to provide precise measurements of time intervals. However it's a class 
and thus it's usage produce a bit of pressure of on garbage collectior. 
`Codestellation.Statsd` provide a better way to avoid this using a `LeanStopwatch` structure:
```
var client = BuildStatsd.From("udp://my-host:8085");
var leanStopwatch = LeanStopwatch.StartNew();

//Do some stuff. 
var timing = leanStopWatch.Elapsed("eating.cookies");

//Log the measurement using IStatsdClient. 
client.LogTiming(timing);

//Or use it's extension method which accepts LeanStopwatch instance
client.LogTiming("eating.cookies", leanStopwatch);
```

One thing to notice: `IChannel` implementations are not thread safe and thus it's not recommended to reuse them among multiple instances of `IStatsdClient` implementations.

# How to contribute

* Use core.autocrlf=True
* Use spaces instead of tabs
* Use [Allman indent style](https://en.wikipedia.org/wiki/Indent_style#Allman_style)