using AutoMapper;
using Byohar.Application.Interfaces.Persistance;
using Byohar.Application.Requests;
using Byohar.Application.Responses.Events;
using Byohar.Application.Specifications.Events;
using Byohar.Domain.Entities.Events;
using Byohar.Infrastructure.Extensions;
using Byohar.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Features.Events.Queries.GetAll;

public class GetAllEventQuery : FilterPagedRequest,
    IRequest<Result<PaginatedResult<EventResponse>>>
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
    public bool? IsActive { get; set; }

    public string? Location { get; set; }

    public bool? HasBanner { get; set; }
    public Guid? TenantId { get; set; }
}

public class GetAllEventQueryHandler
    : IRequestHandler<GetAllEventQuery,
        Result<PaginatedResult<EventResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllEventQueryHandler> _logger;

    public GetAllEventQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllEventQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<EventResponse>>> Handle(
        GetAllEventQuery request,
        CancellationToken ct)
    {
        try
        {
            var filter = new EventFilterSpecification(request);

            var query = _unitOfWork.Repository<Event>()
                .Entities()
                .Specify(filter)
                .AsNoTracking()
                .OrderByDescending(x => x.Date);

            var pagedData = await query
                .ToPaginatedListAsync(
                    request.PageNumber,
                    request.PageSize);

            var result = _mapper.Map<
                PaginatedResult<EventResponse>>(pagedData);

            _logger.LogInformation(
                "Retrieved all events successfully.");

            return Result<PaginatedResult<EventResponse>>
                .Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving events");

            return Result<PaginatedResult<EventResponse>>
                .Fail(ex.Message);
        }
    }
}
