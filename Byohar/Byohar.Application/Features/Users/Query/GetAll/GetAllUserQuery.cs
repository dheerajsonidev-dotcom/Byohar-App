using AutoMapper;
using Byohar.Application.Interfaces.Persistence;
using Byohar.Application.Requests.Users;
using Byohar.Application.Responses;
using Byohar.Domain.Entities.Identity;
using Byohar.Infrastructure.Extensions;
using Byohar.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Byohar.Application.Features.Users.Query.GetAll
{
    public class GetAllUserQuery
        : UserPagingRequest, IRequest<Result<PaginatedResult<UserResponse>>>
    {
        // StringSearch already exists in UserPagingRequest
    }

    public class GetAllUserQueryHandler
        : IRequestHandler<GetAllUserQuery, Result<PaginatedResult<UserResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllUserQueryHandler> _logger;

        public GetAllUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllUserQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<UserResponse>>> Handle(
            GetAllUserQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _unitOfWork
                    .Repository<ApplicationUser>()
                    .Entities()
                    .Include(x => x.UserRoles)
                        .ThenInclude(r => r.Role)
                    .AsQueryable();

                // 🔍 ✅ APPLY SEARCH (THIS FIXES EVERYTHING)
                if (!string.IsNullOrWhiteSpace(request.StringSearch))
                {
                    var search = request.StringSearch.Trim().ToLower();

                    query = query.Where(u =>
                        u.FirstName.ToLower().Contains(search) ||
                        u.LastName.ToLower().Contains(search) ||
                        u.Email.ToLower().Contains(search) ||
                        u.PhoneNumber.ToLower().Contains(search) ||
                        u.UserRoles.Any(ur =>
                            ur.Role.Name.ToLower().Contains(search))
                    );
                }

                // ↕ Sorting (already working)
                if (!string.IsNullOrEmpty(request.SortColumn))
                {
                    query = query.ApplySorting(
                        request.SortColumn,
                        request.SortOrder
                    );
                }

                // 📄 Pagination
                var pagedUsers = await query.ToPaginatedListAsync(
                    request.PageNumber,
                    request.PageSize
                );

                var result = _mapper.Map<PaginatedResult<UserResponse>>(pagedUsers);

               _logger.LogInformation("Fetched {Count} users for page {PageNumber} with page size {PageSize}.",
                    result.Data.Count, request.PageNumber, request.PageSize);
                return Result<PaginatedResult<UserResponse>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users: {Message}", ex.Message);
                return Result<PaginatedResult<UserResponse>>.Fail(ex.Message);
            }
        }
    }
}

