using System;

namespace npcblas2.Models
{
    /// <summary>
    /// A data transfer object matching <see cref="Data.Choice"/>
    /// </summary>
    public class ChoiceDto
    {
        public Guid CharacterBuildId { get; set; }

        public int Order { get; set; }

        public string Value { get; set; }
    }
}