module public StringExpression

open System.Text

// A computation expression for creating strings, a little like a StringBuilder.
// TODO Make this into its own repository that builds a NuGet package :)
[<StructuredFormatDisplay("{AsString}")>]
type Stringified<'a> = { Items: 'a list; MakeString: StringBuilder -> StringBuilder }
with
    member this.AsString = this.ToString()
    override this.ToString() =
        let sb = StringBuilder() |> this.MakeString
        sb.ToString()

type StringExpression() =
    let getItems s = s.Items

    member this.Bind (m: Stringified<'a>, f) = this.For (m.Items, f)

    member this.Combine (a: Stringified<'a>, b: Stringified<'a>) = {
        Items = List.append a.Items b.Items
        MakeString = a.MakeString >> b.MakeString
    }

    member this.Delay f = f()

    member this.For (m: 'a list, f) =
        this.Zero() |> List.foldBack (fun m2 s ->
            let s2 = f m2
            this.Combine (s2, s)) m

    member this.For (m: 'a seq, f) =
        this.Zero() |> Seq.foldBack (fun m2 s ->
            let s2 = f m2
            this.Combine (s2, s)) m

    member this.For (m: Stringified<'a>, f) = this.Bind (m, f)

    member this.Return (x: bool) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: char) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: char[]) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: decimal) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: double) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: int8) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: int16) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: int32) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: int64) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: string) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: uint8) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: uint16) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: uint32) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return (x: uint64) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Return x = { Items = [x]; MakeString = fun sb -> x :> obj |> sb.Append }

    member this.ReturnFrom m = m

    member this.Yield (x: bool) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: char) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: char[]) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: decimal) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: double) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: int8) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: int16) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: int32) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: int64) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: string) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: uint8) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: uint16) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: uint32) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield (x: uint64) = { Items = [x]; MakeString = fun sb -> sb.Append x }
    member this.Yield x = { Items = [x]; MakeString = fun sb -> x :> obj |> sb.Append }

    member this.YieldFrom m = m

    member this.Zero() = { Items = []; MakeString = id }

let string = StringExpression()
