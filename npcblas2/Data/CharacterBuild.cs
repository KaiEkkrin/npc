using System;
using System.Collections.Generic;

namespace npcblas2.Data
{
    /// <summary>
    /// Represents a character that might be done or still in process of being built,
    /// along with metadata.
    /// </summary>
    public class CharacterBuild
    {
        /// <summary>
        /// The current build output version.
        /// </summary>
        public const int CurrentVersion = 1;

        /// <summary>
        /// Uniquely identifies the character.
        /// </summary>
        public Guid Id { get; set; }

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
        /// Summary info: the character's level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// A textual summary of the character, if available.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// True if this character can be viewed by other users, else false.
        /// </summary>
        public bool? IsPublic { get; set; }

        /// <summary>
        /// The version of this record (at a mismatching version, we shouldn't expect to
        /// be able to interpret the build output correctly.)
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The choices that we made to create this build, first to last.
        /// (The character can be reconstructed from it.)
        /// </summary>
        public List<Choice> Choices { get; set; }
    }
}