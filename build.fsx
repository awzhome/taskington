#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli //
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.Core.TargetOperators

// *** Define Targets ***
Target.create "Clean" (fun _ ->
  DotNet.exec (fun defaults -> defaults) "clean" "PPBackup.sln" |> ignore
)

Target.create "Build" (fun _ ->
  DotNet.build (fun defaults -> defaults) "PPBackup.sln"
)

Target.create "CleanBuild" (fun _ ->
  Target.run 1 "Clean" []
  Target.run 1 "Build" []
)

// *** Start Build ***
Target.runOrDefault "Build"