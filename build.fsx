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
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.Installer

Target.create "Clean" (fun _ ->
  DotNet.exec (fun p -> p) "clean" "PPBackup.sln" |> ignore
  Shell.cleanDir "build"
)

Target.create "Build" (fun _ ->
  DotNet.build (fun p -> p) "PPBackup.sln"
)

let publish variant runtime project =
  let publishDir = "build" @@ ("publish-" + variant)
  Shell.cleanDir publishDir
  DotNet.publish (fun p ->
  { p with
      OutputPath = Some(publishDir)
      SelfContained = Some(true)
      Runtime = Some(runtime)
      MSBuildParams =
      { MSBuild.CliArguments.Create() with
          Properties = [("PublishTrimmed", "true")]
      }
  }) (project @@ (project + ".csproj"))

Target.create "Publish-Win64" (fun _ ->
  publish "win64" "win-x64" "PPBackup.WinApp"
)

Target.create "Installer-Win64" (fun _ ->
  InnoSetup.build (fun p -> 
  { p with 
     ScriptFile = "ppbackup.iss"
     ToolPath = Environment.ProgramFilesX86 @@ "Inno Setup 6" @@ "ISCC.exe"
  })
)

"Publish-Win64"
  ==> "Installer-Win64"

"Publish"
  ==> "Installer-Win"

// *** Start Build ***
Target.runOrDefaultWithArguments "Installer-Win64"
