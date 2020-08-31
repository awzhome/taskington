module Versioning

type Version =
    { Major: string
      Minor: string
      Build: string option
      Suffix: string option }

    static member Create(major, minor) =
        { Major = major
          Minor = minor
          Build = None
          Suffix = None }

    static member Create(major, minor, build) =
        { Major = major
          Minor = minor
          Build = Some(build)
          Suffix = None }

    static member Create(major, minor, build, suffix) =
        { Major = major
          Minor = minor
          Build = Some(build)
          Suffix = Some(suffix) }

let asString version =
    match version with
    | { Major = major; Minor = minor; Build = build; Suffix = suffix } ->
        major
        + "."
        + minor
        + ((build |> Option.map (fun b -> "." + b))
           |> Option.defaultValue "")
        + ((suffix |> Option.map (fun s -> "-" + s))
           |> Option.defaultValue "")

let withBuild version =
    { version with
          Build = (version.Build |> Option.orElse (Some("0"))) }
    |> asString

let withoutSuffix version =
    { version with Suffix = None } |> asString

let withMajorMinorOnly version =
    asString
        { version with
              Build = None
              Suffix = None }

let splitVersionStr (version: string) =
    match version.Split('.') with
    | [| major; minor |] -> Version.Create(major, minor)
    | [| major; minor; build |] -> Version.Create(major, minor, build)
    | _ -> Version.Create("0", "0")

let parseVersionArgs (args: string list) =
    match args with
    | [ version ] -> splitVersionStr version
    | [ version; suffix ] ->
        { (splitVersionStr version) with
              Suffix = Some(suffix) }
    | _ -> Version.Create("0", "0")
