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
            .ForMember(d => d.TipoDocumento,   o => o.Ignore());
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

        // Ordenes
        CreateMap<OrdenServicio, OrdenServicioDto>()
            .ForCtorParam("ClienteNombre", o => o.MapFrom(s => s.Cliente != null ? $"{s.Cliente.Nombres} {s.Cliente.Apellidos}" : null))
            .ForCtorParam("VehiculoPlaca", o => o.MapFrom(s => s.Vehiculo != null ? s.Vehiculo.Placa : null))
            .ForCtorParam("MecanicoNombre", o => o.MapFrom(s => s.Mecanico != null ? $"{s.Mecanico.Nombres} {s.Mecanico.Apellidos}" : null));
        CreateMap<CreateOrdenDto, OrdenServicio>();

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
