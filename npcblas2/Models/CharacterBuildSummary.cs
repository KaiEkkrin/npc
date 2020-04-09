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
        public CharacterBuildDto Build { get; set; }

        /// <summary>
        /// The handle of the user that created it.  (Mustn't be null.)
        /// </summary>
        public string Handle { get; set; }
    }
}