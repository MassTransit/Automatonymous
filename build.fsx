#r @"src/packages/FAKE/tools/FakeLib.dll"
open System.IO
open Fake
open Fake.AssemblyInfoFile
open Fake.Git.Information
open Fake.SemVerHelper

let buildOutputPath = "./build_output"
let buildArtifactPath = "./build_artifacts"
let nugetWorkingPath = FullName "./build_temp"
let packagesPath = FullName "./src/packages"
let keyFile = FullName "./Automatonymous.snk"

let assemblyVersion = "3.3.0.0"
let baseVersion = "3.3.0"

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


printfn "Using version: %s" Version

Target "Clean" (fun _ ->
  ensureDirectory buildOutputPath
  ensureDirectory buildArtifactPath
  ensureDirectory nugetWorkingPath

  CleanDir buildOutputPath
  CleanDir buildArtifactPath
  CleanDir nugetWorkingPath
)

Target "RestorePackages" (fun _ -> 
     "./src/Automatonymous.sln"
     |> RestoreMSSolutionPackages (fun p ->
         { p with
             OutputPath = packagesPath
             Retries = 4 })
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

  let buildMode = getBuildParamOrDefault "buildMode" "Release"
  let setParams defaults = { 
    defaults with
        Verbosity = Some(Quiet)
        Targets = ["Clean"; "Build"]
        Properties =
            [
                "Optimize", "True"
                "DebugSymbols", "True"
                "RestorePackages", "True"
                "Configuration", buildMode
                "SignAssembly", "True"
                "AssemblyOriginatorKeyFile", keyFile
                "TargetFrameworkVersion", "v4.5.2"
                "Platform", "Any CPU"
            ]
  }

  build setParams @".\src\Automatonymous.sln"
      |> DoNothing
)

let testDlls = !! ("./src/Automatonymous.Tests/bin/Release/*.Tests.dll")

Target "UnitTests" (fun _ ->
    testDlls
        |> NUnit (fun p -> 
            {p with
                Framework = "v4.0.30319"
                DisableShadowCopy = true; 
                OutputFile = buildArtifactPath + "/nunit-test-results.xml"})
)

type packageInfo = {
    Project: string
    PackageFile: string
    Summary: string
    Files: list<string*string option*string option>
}

Target "Package" (fun _ ->

  let nugs = [| { Project = "Automatonymous"
                  Summary = "Automatonymous, an open source state machine library, usable with MassTransit"
                  PackageFile = @".\src\Automatonymous\packages.config"
                  Files = [ (@"..\src\Automatonymous\bin\Release\Automatonymous.*", Some @"lib\net452", None);
                            (@"..\src\Automatonymous\**\*.cs", Some "src", None) ] }
                { Project = "Automatonymous.NHibernate"
                  Summary = "Automatonymous NHibernate Support"
                  PackageFile = @".\src\Automatonymous.NHibernateIntegration\packages.config"
                  Files = [ (@"..\src\Automatonymous.NHibernateIntegration\bin\Release\Automatonymous.NHibernateIntegration.*", Some @"lib\net452", None);
                            (@"..\src\Automatonymous.NHibernateIntegration\**\*.cs", Some @"src", None) ] } 
                { Project = "Automatonymous.Visualizer"
                  Summary = "Automatonymous Visualizer Support"
                  PackageFile = @".\src\Automatonymous.Visualizer\packages.config"
                  Files = [ (@"..\src\Automatonymous.Visualizer\bin\Release\Automatonymous.Visualizer.*", Some @"lib\net452", None);
                            (@"..\src\Automatonymous.Visualizer\**\*.cs", Some @"src", None) ] } 
             |]

  nugs
    |> Array.iter (fun nug ->

      let getDeps daNug : NugetDependencies =
        if daNug.Project = "Automatonymous" then (getDependencies daNug.PackageFile)
        else ("Automatonymous", NuGetVersion) :: (getDependencies daNug.PackageFile)

      let setParams defaults = {
        defaults with 
          Authors = ["Chris Patterson"]
          Description = "Automatonymous, an open source state machine library, usable with MassTransit."
          OutputPath = buildArtifactPath
          Project = nug.Project
          Dependencies = (getDeps nug)
          Summary = nug.Summary
          SymbolPackage = NugetSymbolPackage.Nuspec
          Version = NuGetVersion
          WorkingDir = nugetWorkingPath
          Files = nug.Files
      } 

      NuGet setParams (FullName "./template.nuspec")
    )
)

Target "Default" (fun _ ->
  trace "Build starting..."
)

"Clean"
  ==> "RestorePackages"
  ==> "Build"
  ==> "UnitTests"
  ==> "Package"
  ==> "Default"

RunTargetOrDefault "Default"