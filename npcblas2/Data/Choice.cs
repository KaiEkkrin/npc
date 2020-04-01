using System;

namespace npcblas2.Data
{
    /// <summary>
    /// Represents a choice made during character build.
    /// </summary>
    public class Choice
    {
        /// <summary>
        /// The character build it applies to.
        /// </summary>
        public Guid CharacterBuildId { get; set; }

        /// <summary>
        /// The character build.
        /// </summary>
        public CharacterBuild CharacterBuild { get; set; }

        /// <summary>
        /// Defines the order between choices made for the same character.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The choice that was made.
        /// </summary>
        public string Value { get; set; }
    }
}
