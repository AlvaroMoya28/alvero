using AutoMapper;
using EventosBackend.Models.DTOs.Requests;
using EventosBackend.Models.DTOs.Responses;
using EventosBackend.Models.Entities;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<Usuario, UsuarioResponse>();
        CreateMap<UsuarioCreateRequest, Usuario>()
            .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.IdUsuario))
            .ForMember(dest => dest.TipoUsuario, opt => opt.MapFrom(src => src.Rol))
            .ForMember(dest => dest.ContrasenaHash, opt => opt.Ignore())
            .ForMember(dest => dest.Salt, opt => opt.Ignore());
    }
}