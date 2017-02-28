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

The main interface of  `Codestellation.Statsd` is `IStatsdClient` interface which contains a bunch of methods to send metrics to a statsd server. It has two implementations^ 
* `StatsdClient` - simple synchronous client which is not thread safe. If you want to use it in a multithreaded environment consider instace per thread scenario.
* `BackgroundStatsdClient` - asynchronous client which uses a background task to send metrics. It's absolutely thread safe. 


The usage is as simple as:

```
var channel = new UdpChannel("statsd-server.org", 8888);
var client = new StatsdClient(channel);

var count = new Count("bananas", 10);
client.LogCount(count);
```

One thing to notice: `IChannel` implementations are not thread safe and thus it's not recommended to reuse them among multiple instances of `IStatsdClient` implementations.

# How to contribute

* Use core.autocrlf=True
* Use spaces instead of tabs
* Use [Allman indent style](https://en.wikipedia.org/wiki/Indent_style#Allman_style)