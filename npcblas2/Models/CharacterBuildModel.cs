using Npc;

namespace npcblas2.Models
{
    /// <summary>
    /// Constains a character build and the current output:
    /// </summary>
    public class CharacterBuildModel
    {
        /// <summary>
        /// The build.
        /// </summary>
        public CharacterBuildDto Build { get; set; }

        /// <summary>
        /// The build output (created dynamically.)
        /// </summary>
        public IBuildAbstraction BuildOutput { get; set; }

        /// <summary>
        /// True if this character build can be edited, else false.
        /// </summary>
        public bool CanEdit { get; set; }
    }
}
