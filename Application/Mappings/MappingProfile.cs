using AutoMapper;
using Application.UseCase.Auth;
using Application.UseCase.Auditoria;
using Application.UseCase.Citas;
using Application.UseCase.Clientes;
using Application.UseCase.Empleados;
using Application.UseCase.Facturas;
using Application.UseCase.Inventario;
using Application.UseCase.Ordenes;
using Application.UseCase.Repuestos;
using Application.UseCase.Vehiculos;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Clientes
        CreateMap<Cliente, ClienteDto>()
            .ForCtorParam("TipoDocumento", o => o.MapFrom(s => s.TipoDocumento != null ? s.TipoDocumento.Nombre : string.Empty));
        CreateMap<CreateClienteDto, Cliente>()
            .ForMember(d => d.TipoDocumentoId, o => o.Ignore())
            .ForMember(d => d.TipoDocumento,   o => o.Ignore())
            .ForMember(d => d.UsuarioId,        o => o.Ignore())
            .ForMember(d => d.Usuario,          o => o.Ignore());
        CreateMap<UpdateClienteDto, Cliente>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // Vehiculos
        CreateMap<Vehiculo, VehiculoDto>()
            .ForCtorParam("Marca", o => o.MapFrom(s => s.ModeloVehiculo != null && s.ModeloVehiculo.Marca != null ? s.ModeloVehiculo.Marca.Nombre : null))
            .ForCtorParam("Modelo", o => o.MapFrom(s => s.ModeloVehiculo != null ? s.ModeloVehiculo.Nombre : null))
            .ForCtorParam("Color", o => o.MapFrom(s => s.Color != null ? s.Color.Nombre : null));
        CreateMap<CreateVehiculoDto, Vehiculo>();
        CreateMap<UpdateVehiculoDto, Vehiculo>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // Citas
        CreateMap<Cita, CitaDto>()
            .ForCtorParam("ClienteNombre", o => o.MapFrom(s => s.Cliente != null ? $"{s.Cliente.Nombres} {s.Cliente.Apellidos}" : null))
            .ForCtorParam("VehiculoPlaca", o => o.MapFrom(s => s.Vehiculo != null ? s.Vehiculo.Placa : null));
        CreateMap<CreateCitaDto, Cita>();
        CreateMap<UpdateCitaDto, Cita>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // Ordenes
        CreateMap<OrdenServicio, OrdenServicioDto>()
            .ForCtorParam("ClienteNombre", o => o.MapFrom(s => s.Cliente != null ? $"{s.Cliente.Nombres} {s.Cliente.Apellidos}" : null))
            .ForCtorParam("VehiculoPlaca", o => o.MapFrom(s => s.Vehiculo != null ? s.Vehiculo.Placa : null))
            .ForCtorParam("MecanicoNombre", o => o.MapFrom(s => s.Mecanico != null ? $"{s.Mecanico.Nombres} {s.Mecanico.Apellidos}" : null))
            .ForCtorParam("TipoServicioNombre", o => o.MapFrom(s => s.TipoServicio != null ? s.TipoServicio.Nombre : null))
            .ForCtorParam("Detalles", o => o.MapFrom(s => (List<DetalleOrdenDto>?)null))
            .ForCtorParam("ManosObra", o => o.MapFrom(s => (List<ManoObraOrdenDto>?)null));
        CreateMap<CreateOrdenDto, OrdenServicio>()
            .ForMember(d => d.ManosObra,            o => o.Ignore())
            .ForMember(d => d.DetallesOrdenServicio, o => o.Ignore());

        // Repuestos
        CreateMap<Repuesto, RepuestoDto>()
            .ForCtorParam("Categoria", o => o.MapFrom(s => s.CategoriaRepuesto != null ? s.CategoriaRepuesto.Nombre : null));
        CreateMap<CreateRepuestoDto, Repuesto>();
        CreateMap<UpdateRepuestoDto, Repuesto>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // Inventario
        CreateMap<MovimientoInventario, MovimientoInventarioDto>()
            .ForCtorParam("RepuestoNombre", o => o.MapFrom(s => s.Repuesto != null ? s.Repuesto.Nombre : null));

        // Facturas
        CreateMap<Factura, FacturaDto>()
            .ForCtorParam("ClienteNombre", o => o.MapFrom(s => s.Cliente != null ? $"{s.Cliente.Nombres} {s.Cliente.Apellidos}" : null));

        // Empleados
        CreateMap<Empleado, EmpleadoDto>();
        CreateMap<CreateEmpleadoDto, Empleado>();

        // Auditoria
        CreateMap<Auditoria, AuditoriaDto>();
    }
}
