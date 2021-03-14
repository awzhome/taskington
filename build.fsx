#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli //
nuget Fake.Core.Target //
nuget Fake.Installer.InnoSetup //"

#load "./.fake/build.fsx/intellisense.fsx"
#load "Version.fsx"

open Fake.Core
open Fake.DotNet
open Fake.Core.TargetOperators
open Fake.Core.Operators
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Installer
open Versioning

let defaults p = p

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
                        Properties = [ ("PublishTrimmed", "true") ] } }) (project @@ (project + ".csproj"))

let writeVersion (template: string) version (files: string seq) =
    files
    |> Shell.regexReplaceInFilesWithEncoding
        (template.Replace("$$$", "(.*)"))
           (template.Replace("$$$", version))
           System.Text.Encoding.UTF8

Target.create "Clean" (fun _ ->
    DotNet.exec defaults "clean" "PPBackup.sln"
    |> ignore
    Shell.cleanDir "build")

Target.create "Build" (fun _ -> DotNet.build defaults "PPBackup.sln")

Target.create "Publish-Win64" (fun _ -> publish "win64" "win-x64" "PPBackup.WinApp")

Target.create "Installer-Win64" (fun _ ->
    InnoSetup.build (fun p ->
        { p with
              ScriptFile = "ppbackup.iss"
              ToolPath =
                  Environment.ProgramFilesX86
                  @@ "Inno Setup 6"
                  @@ "ISCC.exe" }))

"Publish-Win64" ==> "Installer-Win64"

Target.create "UpdateVersion-CI" (fun p ->
    let version = parseVersionArgs p.Context.Arguments

    let installerFile = !! "ppbackup.iss"
    installerFile
    |> writeVersion "#define APP_VERSION \"$$$\"" (version |> withoutSuffix)
    installerFile
    |> writeVersion "#define APP_FULL_VERSION \"$$$\"" (version |> asString)

    !! "**\\*.csproj"
    ++ "**\\*.fsproj"
    |> writeVersion "<Version>$$$</Version>" (version |> withBuild))

Target.create "UpdateVersion" (fun p ->
    let version = parseVersionArgs p.Context.Arguments

    !! "appveyor.yml"
    |> writeVersion "version: '$$$.{build}'" (version |> withMajorMinorOnly)

    !! "CurrentVersion.fsx"
    |> writeVersion "let currentVersion = \"$$$\"" (version |> withMajorMinorOnly))

"UpdateVersion-CI" ==> "UpdateVersion"

// *** Start Build ***
Target.runOrDefaultWithArguments "Installer-Win64"
