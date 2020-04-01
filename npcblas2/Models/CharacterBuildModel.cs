using Npc;
using npcblas2.Data;

namespace npcblas2.Models
{
    /// <summary>
    /// Constains a character build and the current output:
    /// </summary>
    public class CharacterBuildModel
    {
        public CharacterBuild Build { get; set; }

        public BuildOutput BuildOutput { get; set; }
    }
}
