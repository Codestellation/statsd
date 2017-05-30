var target = Argument("target", "Default");

var packageVersion = string.Empty;
var product = "Codestellation.Statsd";
var copyright = string.Format("Copyright (c) Codestellation Team- {0}", DateTime.Now.Year);

var solutionPath = Directory("./src");
var solutionDirInfo = new DirectoryInfo(solutionPath.Path.FullPath);

var buildPath = Directory(Argument("output", "./nuget"));

Task("Version")
  .Does(() => 
{
    var command = new ProcessSettings
    {
        Arguments = "describe --abbrev=7 --first-parent --long --dirty --always",
        RedirectStandardOutput = true
    };
    IEnumerable<string> output;
    var exitCode = StartProcess("git", command, out output);

    var describe = output.Single();

    Information("Git describe is '{0}'", describe);
    var annotatedTagPattern = @"(?<major>[0-9]+).(?<minor>[0-9]+)-(?<revision>[0-9]+)-g(?<hash>[\w]+)-?(?<dirty>[\w]+)*";
    var parts = System.Text.RegularExpressions.Regex.Match(describe, annotatedTagPattern);

    string major = "0";
    string minor = "0";
    string revision = "0";
    string build = "0"; // get it from appveyor
    string hash = string.Empty;
    string dirty = string.Empty;

    if(parts.Success)
    {
      major = parts.Groups["major"].Value;
      minor = parts.Groups["minor"].Value;
      revision = parts.Groups["revision"].Value;
      hash = parts.Groups["hash"].Value;
      dirty = parts.Groups["dirty"].Value;
    }
    else
    {
        var tokens = describe.Split('-');
        hash = tokens[0];
        if(tokens.Length > 1)
        {
          dirty = tokens[1];
        }
    }

    if(AppVeyor.IsRunningOnAppVeyor)
    {
        build = AppVeyor.Environment.Build.Number.ToString();
    }

    var assemblyVersion = string.Format("{0}.{1}", major, minor);
    var fullVersion = string.Format("{0}.{1}.{2}.{3}", major, minor, revision, build);

    packageVersion = string.Format("{0}.{1}.{2}", major, minor, revision);;
    if(!string.IsNullOrWhiteSpace(dirty))
    {
        packageVersion += ("-" + dirty);
    }

    var infoVersion = string.Format("{0} {1}", packageVersion, hash);

    var asmInfo = new AssemblyInfoSettings
    {
        Product = product,
        Version = assemblyVersion,
        FileVersion = fullVersion,
        InformationalVersion = infoVersion,
        Copyright = copyright
    };

    Information("Assembly Version is '{0}'", assemblyVersion);
    Information("Assembly Information Version is '{0}'", infoVersion);
    Information("Package Version is '{0}'", packageVersion);
});

Task("Clean")
  .Does(() => 
{
    DeleteDirectory(buildPath, recursive:true);
});

Task("Restore")
  .Does(() => 
{
    var settings = new ProcessSettings 
    { 
        Arguments = "restore",
        WorkingDirectory = Directory("./src")
    };
    int result;
    if((result = StartProcess("dotnet", settings)) != 0) 
    {
        throw new InvalidOperationException(string.Format("Process returned {0}", result));
    }
});

Task("Build")
    .IsDependentOn("Version")
    .IsDependentOn("Restore")
    .Does(()=>
{
    var settings = new ProcessSettings 
    { 
        Arguments = "build Statsd.sln",
        WorkingDirectory = Directory("./src")
    };
    int result;
    if((result = StartProcess("dotnet", settings)) != 0) 
    {
        throw new InvalidOperationException(string.Format("Process returned {0}", result));
    }
});

Task("Test")
    .IsDependentOn("Build")
    .Does(()=>
{
    var settings = new ProcessSettings 
    { 
        Arguments = "test",
        WorkingDirectory = Directory("./src/Tests")        

    };
    int result;
    if((result = StartProcess("dotnet", settings)) != 0) 
    {
        throw new InvalidOperationException(string.Format("Process returned {0}", result));
    }
});



Task("Pack")
    .IsDependentOn("test")
    .Does(()=>
{
    var artifacts = new DirectoryInfo("./artifacts");
    var args = string.Format("pack Statsd -c Release --output {0} /p:PackageVersion={1}", artifacts.FullName, packageVersion);
    var settings = new ProcessSettings 
    { 
        Arguments = args,
        WorkingDirectory = Directory("./src")
    };
    Information("Pack arguments is '{0}'", args);
    int result;
    if((result = StartProcess("dotnet", settings)) != 0) 
    {
        throw new InvalidOperationException(string.Format("Process returned {0}", result));
    }
});

Task("Push")
    .IsDependentOn("Pack")
    .Does(()=>
{
  // Get the path to the package.
  var package = string.Format("artifacts/Codestellation.Statsd.{0}.nupkg", packageVersion);

  // Push the package.
  NuGetPush(package, new NuGetPushSettings {
      Source = "https://www.myget.org/F/codestellation/api/v2/package",
      ApiKey = EnvironmentVariable("myget_key")
  });
});

Task("Make")
  .IsDependentOn("Push")
  .IsDependentOn("Pack")
  .Does(() =>
{
  
});

Task("Default")
  .IsDependentOn("Test")
  .Does(() =>
{
  
});

RunTarget(target);