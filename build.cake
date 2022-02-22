var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var singleExeSelfContained = Argument<bool>("singleexeselfcontained", false);

const string artifacts= "./artifacts";
const string src= "./src";
var UpdaterCoreLibCsproj=  $"{src}/Updater.CoreLib/Updater.CoreLib.csproj";
var UpdaterClientLibCsproj= $"{src}/Updater.ClientLib/Updater.ClientLib.csproj";
var UpdaterServerLibCsproj= $"{src}/Updater.ServerLib/Updater.ServerLib.csproj";
var UpdaterAppCsproj= $"{src}/UpdaterApp/UpdaterApp.csproj";
var UpdaterAppArtifacts= $"{artifacts}/UpdaterApp";
var UpdaterAppExe= $"{artifacts}/UpdaterApp/UpdaterApp.Exe";
var TestAppCsproj= $"{src}/TestApp/TestApp.csproj";
var TestAppArtifacts= $"{artifacts}/TestApp";
var TestAppExe= $"{artifacts}/TestApp/TestApp.Exe";

void StartUpdaterApp()
{
   var exe = MakeAbsolute(new FilePath(UpdaterAppExe));
   System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo (exe.ToString())
   {
      UseShellExecute= true,
      WorkingDirectory= System.IO.Path.GetDirectoryName(exe.ToString())
   });
}

void KillUpdaterApp()
{
   StartProcess("Taskkill", new ProcessSettings{
      Arguments= $"/F /IM {System.IO.Path.GetFileName(UpdaterAppExe)}",
      Silent=true
   });
}
void StartTestApp(string name)
{
   var exe = MakeAbsolute(new FilePath(TestAppExe));
   System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo (exe.ToString())
   {
      Arguments=$"\"-Name {name}\"",
      UseShellExecute= true,
      WorkingDirectory= System.IO.Path.GetDirectoryName(exe.ToString())
   });
}

void KillTestApp()
{
   StartProcess("Taskkill", $"/F /IM {System.IO.Path.GetFileName(TestAppExe)}");
}


DotNetBuildSettings LibsSettings()
   => new DotNetBuildSettings {
      Configuration= configuration,
   };

DotNetPublishSettings AppsSettings(string realArtifacts)
   => new DotNetPublishSettings {
      Configuration= configuration,
      Runtime= singleExeSelfContained ? "win-x64": "",
      SelfContained = singleExeSelfContained, 
      PublishSingleFile= singleExeSelfContained,
      OutputDirectory=realArtifacts,
   };

Task("KillApps")
.Does(() => {
   Information("Ingore this error messages");
   KillUpdaterApp();
   Console.WriteLine();
   KillTestApp();
   Console.WriteLine();
});

Task("Clean")
.Does(() => {
   if (DirectoryExists(artifacts))
      CleanDirectories($"{artifacts}/**");
   else
      CreateDirectory(artifacts);
});

Task("BuildUpdaterLibs")
.Does(() => {
   foreach(var csproj in new List<string> { UpdaterCoreLibCsproj, UpdaterClientLibCsproj, UpdaterServerLibCsproj})
   {
      DotNetRestore(csproj);
      DotNetBuild(csproj, LibsSettings());
   }
});

Task("PublishUpdaterApp")
.Does(() => {
   DotNetRestore(UpdaterAppCsproj);
   DotNetPublish(UpdaterAppCsproj, AppsSettings(UpdaterAppArtifacts));
});

Task("PublishTestApp")
.Does(() => {
   DotNetRestore(TestAppCsproj);
   DotNetPublish(TestAppCsproj, AppsSettings(TestAppArtifacts));
});

Task("Default")
.IsDependentOn("KillApps")
.IsDependentOn("Clean")
.IsDependentOn("BuildUpdaterLibs")
.IsDependentOn("PublishUpdaterApp")
.IsDependentOn("PublishTestApp")
.Does(() => {
});


Task("TestOneTestApp")
.IsDependentOn("KillApps")
.Does(() => {
   StartTestApp("TestApp 1/1");
   StartUpdaterApp();
});

Task("TestTwoTestApps")
.IsDependentOn("KillApps")
.Does(() => {
   StartTestApp("TestApp 1/2");
   StartTestApp("TestApp 2/2");
   StartUpdaterApp();
});

Task("TestThreeTestApps")
.IsDependentOn("KillApps")
.Does(() => {
   StartTestApp("TestApp 1/3");
   StartTestApp("TestApp 2/3");
   StartTestApp("TestApp 3/3");
   StartUpdaterApp();
});

RunTarget(target);