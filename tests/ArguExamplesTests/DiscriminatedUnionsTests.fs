module ArguExamples.DiscriminatedUnionsTests

open Microsoft.VisualStudio.TestPlatform.Utilities
open System
open Xunit
open Argu
open Xunit.Abstractions


type ComplexOutputArguments =
    | [<Unique>] StdOut 
    | [<Unique>] File of file:string
with
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | StdOut -> "Output as text to standard out."
            | File _ -> "Output to the specified file."
and ComplexCliArguments =
    | [<Unique>] Output of ComplexOutputArguments
with
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Output _ -> "Specify where to send the output"


type PrimitiveCliArguments =
    | [<Unique>] StdOut  
    | [<Unique>] File of file:string 
with
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | StdOut -> "Primitively send output to stdout"
            | File _ -> "Primitively send output to a file"


type ``Nested Enums only work with primitive types`` (output:ITestOutputHelper) = 
    [<Fact>]
    let ``cannot parse output as complex type`` () =
        let createParser () =
            let parser = ArgumentParser.Create<ComplexCliArguments>()
            ignore()
        Assert.Throws<ArguException>(createParser)

    [<Fact>]
    let ``can parse output as primitive union type to stdout`` () =
        let parser = ArgumentParser.Create<PrimitiveCliArguments>()
        let results = parser.Parse [|"--stdout"|]
        let actual = results.GetResult (<@ StdOut @>)
        output.WriteLine("Parsed {0}", (sprintf "%A" actual))
        Assert.Equal(StdOut, actual)
        
        
    [<Fact>]
    let ``can parse output as primitive union type file with path`` () =
        let parser = ArgumentParser.Create<PrimitiveCliArguments>()
        let results = parser.Parse [|"--file"; "foo.txt"|]
        let filePath = results.GetResult (<@ File @>)
        Assert.Equal(filePath, "foo.txt")        