using System.ComponentModel.DataAnnotations;

namespace npcblas2.Models
{
    public class ExportModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(40)] // TODO Also make it complain about characters not valid in file names
        public string FileName { get; set; }
    }
}