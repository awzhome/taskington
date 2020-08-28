#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli //
nuget Fake.Core.Target //
nuget Fake.Installer.InnoSetup //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.Core.TargetOperators
open Fake.Core.Operators
open Fake.IO.FileSystemOperators
open Fake.Installer

// *** Define Targets ***
Target.create "Clean" (fun _ ->
  DotNet.exec (fun p -> p) "clean" "PPBackup.sln" |> ignore
)

Target.create "Build" (fun _ ->
  DotNet.build (fun p -> p) "PPBackup.sln"
)

Target.create "Publish" (fun _ ->
  DotNet.build (fun p ->
  { p with
      OutputPath = Some("build" @@ "publish")
  }) ("PPBackup.WinApp" @@ "PPBackup.WinApp.csproj")
)

Target.create "Installer-Win" (fun _ ->
  InnoSetup.build (fun p -> 
  { p with 
     ScriptFile = "ppbackup.iss"
     ToolPath = Environment.ProgramFilesX86 @@ "Inno Setup 6" @@ "ISCC.exe"
  })
)

"Publish"
  ==> "Installer-Win"

// *** Start Build ***
Target.runOrDefaultWithArguments "Build"