using AutoMapper;
using Byohar.Application.Interfaces.Persistance;
using Byohar.Application.Responses;
using Byohar.Domain.Entities.Identity;
using Byohar.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Byohar.Application.Features.Users.Query.GetById;

public class GetByIdQuery: IRequest<Result<UserResponse>>
{
    public Guid Id { get; set; }
}


public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, Result<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetByIdQueryHandler> _logger;

    public GetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UserResponse>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {

            var user = _unitOfWork.Repository<ApplicationUser>().Entities().FirstOrDefault(x=>x.Id == request.Id);

            if(user == null)
            {
                return Result<UserResponse>.Fail("User Not found");
            }
       

            var applicationUser = _mapper.Map<UserResponse>(user);

            _logger.LogInformation("GetByIdQueryHandler: User retrieved successfully with Id: {UserId}", request.Id);
            return Result<UserResponse>.Success(applicationUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdQueryHandler: Error retrieving user with Id: {UserId}", request.Id);
            return Result<UserResponse>.Fail(ex.Message);
        }
    }
}
