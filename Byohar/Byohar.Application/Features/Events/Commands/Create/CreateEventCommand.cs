using AutoMapper;
using Byohar.Application.Interfaces.Persistance;
using Byohar.Application.Requests.Events;
using Byohar.Domain.Entities.Events;
using Byohar.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Features.Events.Commands.Create;

public class CreateEventCommand : EventRequest, IRequest<Result<Guid>>
{
}

public class CreateEventCommandHandler
    : IRequestHandler<CreateEventCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateEventCommandHandler> _logger;

    public CreateEventCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateEventCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateEventCommand request,
        CancellationToken ct)
    {
        try
        {
            var repo = _unitOfWork.Repository<Event>();

            bool exists = await repo.Entities()
                .AnyAsync(x => x.Title == request.Title, ct);

            if (exists)
            {
                _logger.LogWarning(
                    "Event already exists with title: {Title}",
                    request.Title);

                return Result<Guid>.Fail(
                    $"Event '{request.Title}' already exists.");
            }

            var entity = _mapper.Map<Event>(request);

            repo.Add(entity);

            await _unitOfWork.SaveAsync(ct);

            _logger.LogInformation(
                "Event created successfully: {EventId}",
                entity.Id);

            return Result<Guid>.Success(
                entity.Id,
                "Event created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while creating event");

            return Result<Guid>.Fail(ex.Message);
        }
    }
}