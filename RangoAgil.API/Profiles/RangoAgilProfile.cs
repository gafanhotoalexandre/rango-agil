using AutoMapper;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.Profiles;

public class RangoAgilProfile : Profile
{
    public RangoAgilProfile()
    {
        CreateMap<Rango, RangoDTO>().ReverseMap();
        CreateMap<Rango, CreateRangoDTO>().ReverseMap();
        CreateMap<Rango, UpdateRangoDTO>().ReverseMap();
        CreateMap<Ingrediente, IngredienteDTO>()
            .ForMember(dest => dest.RangoId, opt => opt.MapFrom(src => src.Rangos.First().Id));
    }
}
