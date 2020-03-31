namespace Npc

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading.Tasks
open Npc.Attributes

// Interface and helpers for storing character builds, conceived to be conveniently callable
// from C#

type IPersistence =
    
    // Adds a new character (with identifier and level) and returns the current state of the build.
    abstract member AddAsync : string * int -> Task<BuildOutput>

    // Makes a choice for a character by index and returns the subsequent state of the build.
    abstract member BuildAsync : string * int -> Task<BuildOutput>

    // Retrieves an existing character by identifier and returns the current state of the build.
    abstract member GetAsync : string -> Task<BuildOutput>

    // Lists all known character names
    abstract member GetAllAsync : unit -> Task<ICollection<string>>

    // Removes a character
    abstract member RemoveAsync : string -> Task

// An in-memory persistence layer.
type InMemoryPersistence () =
    let dict = ConcurrentDictionary<string, BuildOutput>()

    let startCharacter (name, level) =
        let c, imps = Build.start name (level * 1<Level>)
        Build.build (None, c, imps)

    interface IPersistence with
        member this.AddAsync (name: string, level: int) =
            let create n =
                if level < 1 || level > 20
                then raise <| ArgumentOutOfRangeException ("level", "Level must be between 1 and 20")
                else
                    let c, imps = Build.start name (level * 1<Level>)
                    Build.build (None, c, imps)

            dict.GetOrAdd (name, create) |> Task.FromResult

        member this.BuildAsync (name: string, index: int) =
            let update = function
                | MakeChoice (_, chs, c, imps) -> Build.build (Some chs.[index], c, imps)
                | BadChoice (_, chs, c, imps) -> Build.build (Some chs.[index], c, imps)
                | _ -> raise <| InvalidOperationException ("Character already completed")

            // It's weird that ConcurrentDictionary has AddOrUpdate but no simple Update
            // accepting a function, but so be it.  If the character is absent we'll just
            // start a new one at level 1 :)
            let fallback = startCharacter (name, 1)
            dict.AddOrUpdate (name, fallback, fun _ b -> update b) |> Task.FromResult

        member this.GetAsync (name: string) = dict.[name] |> Task.FromResult
        member this.GetAllAsync () = dict.Keys |> Task.FromResult
        member this.RemoveAsync (name: string) =
            dict.TryRemove name |> ignore
            Task.CompletedTask
