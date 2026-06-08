using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using AutoMapper;

namespace LocalCRM.Application.Users.Queries;

public record GetUserByIdQuery(int Id) : IRequest<UserDto?>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IRepository<ApplicationUser> _repository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IRepository<ApplicationUser> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }
}
