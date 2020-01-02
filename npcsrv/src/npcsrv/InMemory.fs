module Npcsrv.InMemory

// From suggestions at
// https://medium.com/@leocavalcante/rest-api-with-mongodb-and-f-on-net-core-605a2336f264
// TODO remove this when I have a real database?

open System.Collections
open Npcsrv.Models

let all (inMemory: Hashtable) = [|
    for v in inMemory.Values do
        match v with
        | :? CharacterBuild as bld -> yield bld
        | _ -> ()
|]

let find (inMemory: Hashtable) (criteria: CharacterBuildCriteria) =
    match criteria.Id, criteria.Name with
    | None, None -> all inMemory
    | None, Some name ->
        let nameUpper = name.ToUpperInvariant()
        all inMemory |> Array.filter (fun bld -> (bld.Character.Name.ToUpperInvariant()) = nameUpper)
    | id, _ when inMemory.ContainsKey id ->
        [| inMemory.[id] :?> CharacterBuild |]
    | _ -> Array.empty

let save (inMemory: Hashtable) (bld: CharacterBuild) =
    if inMemory.ContainsKey bld.Id
    then inMemory.[bld.Id] <- bld
    else inMemory.Add (bld.Id, bld)
    bld

let delete (inMemory: Hashtable) (id: string) =
    if inMemory.ContainsKey id
    then
        inMemory.Remove id
        true
    else false