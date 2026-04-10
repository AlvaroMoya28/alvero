using AutoMapper;
using EventosBackend.Models.Entities;
using EventosBackend.Models.DTOs.Responses;

namespace EventosBackend.Mappings
{
    public class ReservaProfile : Profile
    {
        public ReservaProfile()
        {
            CreateMap<Reserva, ReservaResponse>()
                .ForMember(dest => dest.PrecioTotal, opt => opt.MapFrom(src => src.PrecioTotal));
        }
    }
}
