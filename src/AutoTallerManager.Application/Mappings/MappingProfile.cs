using AutoMapper;
using AutoTallerManager.Application.Features.Auth.DTOs;
using AutoTallerManager.Application.Features.Auditoria.DTOs;
using AutoTallerManager.Application.Features.Citas.DTOs;
using AutoTallerManager.Application.Features.Clientes.DTOs;
using AutoTallerManager.Application.Features.Empleados.DTOs;
using AutoTallerManager.Application.Features.Facturas.DTOs;
using AutoTallerManager.Application.Features.Inventario.DTOs;
using AutoTallerManager.Application.Features.Ordenes.DTOs;
using AutoTallerManager.Application.Features.Repuestos.DTOs;
using AutoTallerManager.Application.Features.Vehiculos.DTOs;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Clientes
        CreateMap<Cliente, ClienteDto>()
            .ForCtorParam("tipoDocumento", o => o.MapFrom(s => s.TipoDocumento != null ? s.TipoDocumento.Nombre : string.Empty));
        CreateMap<CreateClienteDto, Cliente>()
            .ForMember(d => d.TipoDocumentoId, o => o.Ignore());
        CreateMap<UpdateClienteDto, Cliente>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // Vehiculos
        CreateMap<Vehiculo, VehiculoDto>()
            .ForCtorParam("marca", o => o.MapFrom(s => s.ModeloVehiculo != null && s.ModeloVehiculo.Marca != null ? s.ModeloVehiculo.Marca.Nombre : null))
            .ForCtorParam("modelo", o => o.MapFrom(s => s.ModeloVehiculo != null ? s.ModeloVehiculo.Nombre : null))
            .ForCtorParam("color", o => o.MapFrom(s => s.Color != null ? s.Color.Nombre : null));
        CreateMap<CreateVehiculoDto, Vehiculo>();
        CreateMap<UpdateVehiculoDto, Vehiculo>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // Citas
        CreateMap<Cita, CitaDto>()
            .ForCtorParam("clienteNombre", o => o.MapFrom(s => s.Cliente != null ? $"{s.Cliente.Nombres} {s.Cliente.Apellidos}" : null))
            .ForCtorParam("vehiculoPlaca", o => o.MapFrom(s => s.Vehiculo != null ? s.Vehiculo.Placa : null));
        CreateMap<CreateCitaDto, Cita>();

        // Ordenes
        CreateMap<OrdenServicio, OrdenServicioDto>()
            .ForCtorParam("clienteNombre", o => o.MapFrom(s => s.Cliente != null ? $"{s.Cliente.Nombres} {s.Cliente.Apellidos}" : null))
            .ForCtorParam("vehiculoPlaca", o => o.MapFrom(s => s.Vehiculo != null ? s.Vehiculo.Placa : null))
            .ForCtorParam("mecanicoNombre", o => o.MapFrom(s => s.Mecanico != null ? $"{s.Mecanico.Nombres} {s.Mecanico.Apellidos}" : null));
        CreateMap<CreateOrdenDto, OrdenServicio>();

        // Repuestos
        CreateMap<Repuesto, RepuestoDto>()
            .ForCtorParam("categoria", o => o.MapFrom(s => s.CategoriaRepuesto != null ? s.CategoriaRepuesto.Nombre : null));
        CreateMap<CreateRepuestoDto, Repuesto>();
        CreateMap<UpdateRepuestoDto, Repuesto>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // Inventario
        CreateMap<MovimientoInventario, MovimientoInventarioDto>()
            .ForCtorParam("repuestoNombre", o => o.MapFrom(s => s.Repuesto != null ? s.Repuesto.Nombre : null));

        // Facturas
        CreateMap<Factura, FacturaDto>()
            .ForCtorParam("clienteNombre", o => o.MapFrom(s => s.Cliente != null ? $"{s.Cliente.Nombres} {s.Cliente.Apellidos}" : null));

        // Empleados
        CreateMap<Empleado, EmpleadoDto>();
        CreateMap<CreateEmpleadoDto, Empleado>();

        // Auditoria
        CreateMap<Auditoria, AuditoriaDto>();
    }
}
