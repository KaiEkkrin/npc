module Npcsrv.InMemory

// From suggestions at
// https://medium.com/@leocavalcante/rest-api-with-mongodb-and-f-on-net-core-605a2336f264
// TODO remove this when I have a real database?

open System.Collections
open Npcsrv.Models

let find (inMemory: Hashtable) (criteria: CharacterBuildCriteria) = [|
    if inMemory.ContainsKey criteria.Id
    then yield inMemory.[criteria.Id] :?> CharacterBuild
|]

let save (inMemory: Hashtable) (bld: CharacterBuild) =
    if inMemory.ContainsKey bld.Id
    then inMemory.[bld.Id] <- bld
    else inMemory.Add (bld.Id, bld)
    bld
