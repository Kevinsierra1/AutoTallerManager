using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Application.Abstractions;
using Application.Extensions;
using Application.Common;
using Domain.Enums;

namespace Application.UseCase.Empleados;

public record GetEmpleadosQuery(TipoEmpleadoEnum? Tipo, int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<EmpleadoDto>>;

public class GetEmpleadosQueryHandler : IRequestHandler<GetEmpleadosQuery, PagedResult<EmpleadoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmpleadosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<EmpleadoDto>> Handle(GetEmpleadosQuery request, CancellationToken cancellationToken)
    {
        var q = _context.Empleados.AsQueryable();
        if (request.Tipo.HasValue) q = q.Where(x => x.TipoEmpleado == request.Tipo);
        return await q.ProjectTo<EmpleadoDto>(_mapper.ConfigurationProvider).ToPagedResultAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
