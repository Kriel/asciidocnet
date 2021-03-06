﻿namespace Scripts

open System
open System.IO
open System.Text
open Fake.Core 
open Projects
open Versioning

module Release = 
    let private year = sprintf "%i" DateTime.UtcNow.Year
    
    let private addKeyValue key value (builder:StringBuilder) =
        if (not (String.IsNullOrEmpty value)) then builder.AppendFormat("{0}=\"{1}\";", key, value)
        else builder
        
    let private currentMajorVersion version = sprintf "%i" <| version.Full.Major
    let private nextMajorVersion version = sprintf "%i" <| version.Full.Major + 1u

    let private props version =
        let currentMajorVersion = currentMajorVersion version
        let nextMajorVersion = nextMajorVersion version
        new StringBuilder()
        |> addKeyValue "currentMajorVersion" currentMajorVersion
        |> addKeyValue "nextMajorVersion" nextMajorVersion
        |> addKeyValue "year" year

    let pack file n properties version  = 
        Tooling.Nuget.Exec [ "pack"; file; 
             "-version"; version.Full.ToString(); 
             "-outputdirectory"; Paths.BuildOutput; 
             "-properties"; properties; 
        ] |> ignore
        printfn "%s" Paths.BuildOutput
        let file = sprintf "%s.%O.nupkg" n version.Full
        let nugetOutFile = Paths.Output(file)
        let outputFile = Path.Combine(Paths.NugetOutput, file)
        
        File.Move(nugetOutFile, outputFile)

    let private nugetPackMain (p:DotNetProject) nugetId nuspec properties version = 
        pack nuspec nugetId properties version
        
    let private packProjects version callback  =
        Directory.CreateDirectory Paths.NugetOutput |> ignore
            
        DotNetProject.AllPublishable
        |> Seq.iter(fun p ->

            let properties =
                props version
                |> StringBuilder.toText
                
            let nugetId = p.Name 
            let nuspec = (sprintf @"build/%s.nuspec" nugetId)

            callback p nugetId nuspec properties version
        )
            
    let nugetPack (ArtifactsVersion(version))  = packProjects version nugetPackMain 