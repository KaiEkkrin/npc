// Learn more about F# at http://fsharp.org
// This is a scratch project for learning how to make viable
// computation expressions.

open System

// type DoublesBuilder () =
//     member this.Bind (x, f) = x * 2 |> f
//     member this.Return x = x

// type LoggingBuilder () =
//     member this.Bind (x, f) =
//         printfn "Value is %A" x
//         f x

//     member this.Return x = x
//     member this.ReturnFrom x = x

// module Exercise1 =
//     let strToInt (str: string) = Int32.TryParse str
//     let strAdd (str: string) i =
//         match strToInt str with
//         | true, v -> true, i + v
//         | w -> w

//     let (>>=) m f =
//         match m with
//         | true, v -> f v
//         | _ -> m

//     let testGood = strToInt "1" >>= strAdd "2" >>= strAdd "3"
//     let testBad = strToInt "1" >>= strAdd "x" >>= strAdd "3"

// type Exercise1Builder () =
//     member this.Bind (x, f) =
//         match x with
//         | true, v -> f v
//         | _ -> x

//     member this.Return x = true, x

type PromptBuilder () =
    member this.Bind (x, f) =
        let numbered = x |> List.mapi (fun i _ -> i)
        printfn "Choose one : %A" numbered
        match (Console.ReadLine () |> Int32.TryParse) with
        | true, i -> f x.[i - 1]
        | false, _ -> f x.[0] // default to the first value

    member this.Return x = [x]
    member this.ReturnFrom x = x

[<EntryPoint>]
let main argv =
    // let dbl = DoublesBuilder ()
    // let doubled = dbl {
    //     let! v = 4
    //     let! w = 5
    //     return v + w
    // }
    // printfn "doubled: %d" doubled

    // let log = LoggingBuilder ()
    // let logged = log {
    //     let! a = 4
    //     let! b = 5
    //     return a + b
    // }
    // printfn "logged: %d" logged

    // let logged2 = log {
    //     let! a = 4
    //     let! b = 5
    //     return! a + b
    // }
    // printfn "logged2: %d" logged2

    // let exercise1 = Exercise1Builder ()
    // let stringAddWorkflow x y z = exercise1 {
    //     let! a = Exercise1.strToInt x
    //     let! b = Exercise1.strToInt y
    //     let! c = Exercise1.strToInt z
    //     return a + b + c
    // }

    // let good = stringAddWorkflow "12" "3" "2"
    // let bad = stringAddWorkflow "12" "xyz" "2"
    // printfn "exercise1 good: %A" good
    // printfn "exercise1 bad: %A" bad
    // printfn "exercise1 testGood: %A" <| Exercise1.testGood
    // printfn "exercise1 testBad: %A" <| Exercise1.testBad

    let prompt = PromptBuilder ()
    let chosen = prompt {
        let! a = [1; 2; 3; 4; 5]
        let! b = [1; 2; 3; 4]
        return! a * b
    }
    printfn "product: %d" chosen

    0 // return an integer exit code
