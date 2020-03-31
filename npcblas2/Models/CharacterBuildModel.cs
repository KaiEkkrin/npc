using System;
using Npc;

namespace npcblas2.Models
{
    /// <summary>
    /// Represents a character build that has been retrieved from the data store.
    /// </summary>
    public class CharacterBuildModel
    {
        /// <summary>
        /// Uniquely identifies the character.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The id of the user who made this character.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Summary info: the datetime at which this character was created.
        /// </summary>
        public DateTime CreationDateTime { get; set; }

        /// <summary>
        /// Summary info: the character name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Summary info: the character level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The version of this record (at a mismatching version, we shouldn't expect to
        /// be able to interpret the build output correctly.)
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The build output.
        /// </summary>
        public BuildOutput BuildOutput { get; set; }
    }
}