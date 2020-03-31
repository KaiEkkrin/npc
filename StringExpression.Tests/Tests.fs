module StringExpression.Tests

open StringExpression
open Xunit

type Tests() =
    [<Theory; InlineData(""); InlineData("test")>]
    member this.``A single string can be returned`` (str: string) =
        let expr = string {
            return str
        }
        Assert.Equal(str, sprintf "%A" expr)

    [<Theory; InlineData(true); InlineData(false)>]
    member this.``A single boolean can be returned`` (b: bool) =
        let expr = string {
            return b
        }
        let expected = (sprintf "%b" b).ToUpperInvariant()
        let actual = (sprintf "%A" expr).ToUpperInvariant()
        Assert.Equal(expected, actual)

    static member TestPairs : obj [] seq = seq {
        yield [| "ook"; "eek" |]
        yield [| "test"; "" |]
        yield [| ""; "test" |]
        yield [| "banana"; "fish" |]
    }

    [<Theory; MemberData("TestPairs")>]
    member this.``Two strings are concatenated`` (str1: string) (str2: string) =
        let expr = string {
            yield str1
            yield str2
        }
        Assert.Equal(str1 + str2, sprintf "%A" expr)

    static member TestNumberPairs : obj [] seq = seq {
        yield [| 1; 2 |]
        yield [| 0; 14 |]
        yield [| 2; 14 |]
    }

    [<Theory; MemberData("TestNumberPairs")>]
    member this.``Two numbers are concatenated`` (n1: int) (n2: int) =
        let expr = string {
            yield n1
            yield n2
        }
        Assert.Equal(sprintf "%d%d" n1 n2, sprintf "%A" expr)

    [<Theory; MemberData("TestNumberPairs")>]
    member this.``String expression can be deconstructed to numbers`` (n1: int) (n2: int) =
        let expr = string {
            let! n1s = string { n1 }            
            let! n2s = string { n2 }
            return n1s + n2s
        }
        Assert.Equal(sprintf "%d" (n1 + n2), sprintf "%A" expr)

    static member TestLists : obj [] seq =
        seq {
            []
            ["test"]
            ["foo"; "bar"]
            ["one"; "two"; "three"]
            ["one"; ""; "two"; "three"]
        } |> Seq.map (fun l -> [| l |])

    [<Theory; MemberData("TestLists")>]
    member this.``A list can be iterated over`` (strs: string list) =
        let expr = string {
            for s in strs do
                yield s
        }
        let expected = strs |> List.fold (+) ""
        Assert.Equal(expected, sprintf "%A" expr)

    [<Theory; MemberData("TestLists")>]
    member this.``A sequence can be iterated over`` (strs: string list) =
        let strSeq = Seq.ofList strs
        let expr = string {
            for s in strSeq do
                yield s
        }
        let expected = strs |> List.fold (+) ""
        Assert.Equal(expected, sprintf "%A" expr)

    [<Theory; MemberData("TestLists")>]
    member this.``A list can be iterated over in an inner expression`` (strs: string list) =
        let prefix = "start"
        let expr = string {
            yield prefix
            yield! string {
                for s in strs do
                    yield s
            }
        }
        let expected = strs |> List.fold (+) prefix
        Assert.Equal(expected, sprintf "%A" expr)

    [<Theory; MemberData("TestLists")>]
    member this.``A string expression can be unpacked and iterated over`` (strs: string list) =
        let expr1 = string {
            for s in strs do
                yield s
        }
        let expr2 = string {
            for s in expr1 do
                yield s + ".1"
        }
        let expected = strs |> List.map (fun s -> s + ".1") |> List.fold (+) ""
        Assert.Equal(expected, sprintf "%A" expr2)

    [<Theory; MemberData("TestLists")>]
    member this.``A nested expression can be iterated over`` (strs: string list) =
        let prefix = "start"
        let expr = string {
            yield prefix
            let! inner = string {
                for s in strs do
                    yield s
            }
            yield inner + ".1"
        }
        let expected = strs |> List.map (fun s -> s + ".1") |> List.fold (+) prefix
        Assert.Equal(expected, sprintf "%A" expr)
