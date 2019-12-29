namespace Npc

open Npc.Attributes

// What is needed to build a character.
type Build2 = {
    // The character, possibly under construction.
    Character: Character

    // The list of improvements that still need applying, in order.
    Improvements: Improvement2 list
}