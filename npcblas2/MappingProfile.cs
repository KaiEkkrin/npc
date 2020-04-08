using AutoMapper;
using npcblas2.Data;
using npcblas2.Models;

namespace npcblas2
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Choice, ChoiceDto>().ReverseMap();
            CreateMap<CharacterBuild, CharacterBuildDto>().ReverseMap();
        }
    }
}