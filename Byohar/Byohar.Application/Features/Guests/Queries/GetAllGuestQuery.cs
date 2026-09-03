using AutoMapper;
using Byohar.Application.Interfaces.Persistance;
using Byohar.Application.Requests;
using Byohar.Application.Responses.Guests;
using Byohar.Application.Specifications.Guests;
using Byohar.Domain.Entities.Guests;
using Byohar.Infrastructure.Extensions;
using Byohar.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Features.Guests.Queries
{
    public class GetAllGuestQuery : FilterPagedRequest, IRequest<Result<PaginatedResult<GuestResponse>>>
    {
        public Guid? EventId { get; set; }

        public bool? IsActive { get; set; } = true;
    }

    public class GetAllGuestQueryHandler
        : IRequestHandler<GetAllGuestQuery, Result<PaginatedResult<GuestResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllGuestQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedResult<GuestResponse>>> Handle(
            GetAllGuestQuery request,
            CancellationToken ct)
        {
            try
            {
                var guestFilter = new GuestFilterSpecification(request);

                var query = _unitOfWork
                    .Repository<Guest>()
                    .Entities()
                    .Include(x => x.Address)
                    .AsNoTracking()
                    .Specify(guestFilter)
                    .OrderByDescending(x => x.CreatedOn);

                var pagedData = await query.ToPaginatedListAsync(
                    request.PageNumber,
                    request.PageSize);

                var result = _mapper.Map<PaginatedResult<GuestResponse>>(pagedData);

                return Result<PaginatedResult<GuestResponse>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<GuestResponse>>.Fail(ex.Message);
            }
        }
    }
}
