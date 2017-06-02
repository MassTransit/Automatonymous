#r @"src/packages/FAKE/tools/FakeLib.dll"
open System.IO
open Fake
open Fake.AssemblyInfoFile
open Fake.Git.Information
open Fake.SemVerHelper

let buildArtifactPath = FullName "./build_artifacts"
let packagesPath = FullName "./src/packages"

let assemblyVersion = "3.5.0.0"
let baseVersion = "3.5.12"

let semVersion : SemVerInfo = parse baseVersion

let Version = semVersion.ToString()

let branch = (fun _ ->
  (environVarOrDefault "APPVEYOR_REPO_BRANCH" (getBranchName "."))
)

let FileVersion = (environVarOrDefault "APPVEYOR_BUILD_VERSION" (Version + "." + "0"))

let informationalVersion = (fun _ ->
  let branchName = (branch ".")
  let label = if branchName="master" then "" else " (" + branchName + "/" + (getCurrentSHA1 ".").[0..7] + ")"
  (FileVersion + label)
)

let nugetVersion = (fun _ ->
  let branchName = (branch ".")
  let label = if branchName="master" then "" else "-" + (if branchName="mt3" then "beta" else branchName)
  (Version + label)
)

let InfoVersion = informationalVersion()
let NuGetVersion = nugetVersion()

let versionArgs = [ @"/p:Version=""" + NuGetVersion + @""""; @"/p:AssemblyVersion=""" + FileVersion + @""""; @"/p:FileVersion=""" + FileVersion + @""""; @"/p:InformationalVersion=""" + InfoVersion + @"""" ]


printfn "Using version: %s" Version

Target "Clean" (fun _ ->
  ensureDirectory buildArtifactPath

  CleanDir buildArtifactPath
)

Target "RestorePackages" (fun _ -> 
  DotNetCli.Restore (fun p -> { p with Project = "./src/" } )
)

Target "Build" (fun _ ->
  CreateCSharpAssemblyInfo @".\src\SolutionVersion.cs"
    [ Attribute.Title "Automatonymous"
      Attribute.Description "Automatonymous, an open source state machine library, usable with MassTransit"
      Attribute.Product "Automatonymous"
      Attribute.Version assemblyVersion
      Attribute.FileVersion FileVersion
      Attribute.InformationalVersion InfoVersion
    ]

  DotNetCli.Build (fun p-> { p with Project = @".\src\Automatonymous"
                                    Configuration= "Release"
                                    Output = buildArtifactPath
                                    AdditionalArgs = versionArgs })
)

type packageInfo = {
    Project: string
    PackageFile: string
    Summary: string
    Files: list<string*string option*string option>
}

Target "Package" (fun _ ->
  DotNetCli.Pack (fun p-> { p with 
                                Project = @".\src\Automatonymous"
                                Configuration= "Release"
                                OutputPath= buildArtifactPath
                                AdditionalArgs = versionArgs @ [ @"--include-symbols"; @"--include-source" ] })
  DotNetCli.Pack (fun p-> { p with 
                                Project = @".\src\Automatonymous.NHibernateIntegration"
                                Configuration= "Release"
                                OutputPath= buildArtifactPath
                                AdditionalArgs = versionArgs @ [ @"--include-symbols"; @"--include-source" ] })
  DotNetCli.Pack (fun p-> { p with 
                                Project = @".\src\Automatonymous.Visualizer"
                                Configuration= "Release"
                                OutputPath= buildArtifactPath
                                AdditionalArgs = versionArgs @ [ @"--include-symbols"; @"--include-source" ] })
)

Target "Default" (fun _ ->
  trace "Build starting..."
)

"Clean"
  ==> "RestorePackages"
  ==> "Build"
  ==> "Package"
  ==> "Default"

RunTargetOrDefault "Default"