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

    packageVersion = fullVersion;
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
    var projects = new List<FileInfo>();
    projects.AddRange(solutionDirInfo.EnumerateFiles("*.csproj", SearchOption.AllDirectories));

    Information("Found {0} projects", projects.Count);

    foreach(FileInfo project in projects)
    {
        var file = Directory(project.DirectoryName) + File("SolutionVersion.cs");
        CreateAssemblyInfo(file, asmInfo);
        
        var content = System.IO.File.ReadAllText(project.FullName);
        var versionPrefix = string.Format("<VersionPrefix>{0}</VersionPrefix>", packageVersion);
        content = content.Replace("<VersionPrefix>0.0.0.0</VersionPrefix>", versionPrefix);
        System.IO.File.WriteAllText(project.FullName, content);
    }

    Information("AssemblyVersion is '{0}'", infoVersion);
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
    var artifacts = Directory("./artifacts");
    var settings = new ProcessSettings 
    { 
        Arguments = "pack Statsd -c Release --output " + artifacts.Path.FullPath,
        WorkingDirectory = Directory("./src")
    };
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