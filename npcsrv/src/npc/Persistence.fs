namespace Npc

open System
open System.Collections.Concurrent
open System.Collections.Generic
open Npc.Attributes

// Interface and helpers for storing character builds, conceived to be conveniently callable
// from C#

type IPersistence =
    
    // Adds a new character (with identifier and level) and returns the current state of the build.
    abstract member Add : string * int -> BuildOutput

    // Makes a choice for a character by index and returns the subsequent state of the build.
    abstract member Build : string * int -> BuildOutput

    // Retrieves an existing character by identifier and returns the current state of the build.
    abstract member Get : string -> BuildOutput

    // Lists all known character names
    abstract member GetAll : unit -> ICollection<string>

    // Removes a character
    abstract member Remove : string -> unit

// An in-memory persistence layer.
type InMemoryPersistence () =
    let dict = ConcurrentDictionary<string, BuildOutput>()

    let startCharacter (name, level) =
        let c, imps = Build.start name (level * 1<Level>)
        Build.build (None, c, imps)

    interface IPersistence with
        member this.Add (name: string, level: int) =
            let create n =
                if level < 1 || level > 20
                then raise <| ArgumentOutOfRangeException ("level", "Level must be between 1 and 20")
                else
                    let c, imps = Build.start name (level * 1<Level>)
                    Build.build (None, c, imps)

            dict.GetOrAdd (name, create)

        member this.Build (name: string, index: int) =
            let update = function
                | MakeChoice (_, chs, c, imps) -> Build.build (Some chs.[index], c, imps)
                | BadChoice (_, chs, c, imps) -> Build.build (Some chs.[index], c, imps)
                | _ -> raise <| InvalidOperationException ("Character already completed")

            // It's weird that ConcurrentDictionary has AddOrUpdate but no simple Update
            // accepting a function, but so be it.  If the character is absent we'll just
            // start a new one at level 1 :)
            let fallback = startCharacter (name, 1)
            dict.AddOrUpdate (name, fallback, fun _ b -> update b)

        member this.Get (name: string) = dict.[name]
        member this.GetAll () = dict.Keys
        member this.Remove (name: string) = dict.TryRemove name |> ignore
