using System;

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
        /// A serialized build output representing the point in the build we've reached.
        /// </summary>
        public string BuildOutput { get; set; }
    }
}