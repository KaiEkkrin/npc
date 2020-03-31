using System.ComponentModel.DataAnnotations;

namespace npcblas2.Models
{
    public class NewCharacterModel
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        [Range(1, 20)]
        public int Level { get; set; } = 1;
    }
}