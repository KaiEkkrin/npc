using System;
using System.Collections.Generic;

namespace npcblas2.Models
{
    /// <summary>
    /// A data transfer object matching <see cref="Data.CharacterBuild"/>
    /// </summary>
    public class CharacterBuildDto
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public DateTime CreationDateTime { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public string Summary { get; set; }

        public bool? IsPublic { get; set; }

        /// </summary>
        public int Version { get; set; }

        /// </summary>
        public List<ChoiceDto> Choices { get; set; }
    }
}