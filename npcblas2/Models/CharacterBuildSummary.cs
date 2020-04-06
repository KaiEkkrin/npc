using npcblas2.Data;

namespace npcblas2.Models
{
    /// <summary>
    /// A character build and info about it.
    /// </summary>
    public class CharacterBuildSummary
    {
        /// <summary>
        /// The build.
        /// </summary>
        public CharacterBuild Build { get; set; }

        /// <summary>
        /// The name of the user that created it.  (Mustn't be null.)
        /// </summary>
        public string UserName { get; set; }
    }
}