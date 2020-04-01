using System.ComponentModel.DataAnnotations;

namespace npcblas2.Models
{
    public class NewCharacterModel
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        [Range(1, 10)] // TODO Increase the maximum level when I've filled that data in
        public int Level { get; set; } = 1;
    }
}