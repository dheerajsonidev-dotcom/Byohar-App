using Byohar.Application.Interfaces.Persistance;
using Byohar.Application.Requests.Events;
using Byohar.Domain.Entities.Events;
using Byohar.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Features.Events.Commands.Update;

public class UpdateEventCommand : EventRequest, IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
}

public class UpdateEventCommandHandler
    : IRequestHandler<UpdateEventCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateEventCommandHandler> _logger;

    public UpdateEventCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateEventCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        UpdateEventCommand request,
        CancellationToken ct)
    {
        try
        {
            var repo = _unitOfWork.Repository<Event>();

            // Filter ownership in SQL using the mapped Event.TenantId property.
            var entity = await repo.Entities().FirstOrDefaultAsync(x => x.Id == request.Id, ct);

            if (entity == null)
            {
                return Result<Guid>.Fail("Event not found.");
            }

            bool duplicateExists = await repo.Entities()
                .AnyAsync(x =>
                    x.Title == request.Title &&
                    x.Id != request.Id,
                    ct);

            if (duplicateExists)
            {
                _logger.LogWarning(
                    "Duplicate Event Title detected: {Title}",
                    request.Title);

                return Result<Guid>.Fail(
                    $"Another event with title '{request.Title}' already exists.");
            }

            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.Date = request.Date;
            entity.Location = request.Location;
            entity.IsActive = request.IsActive;
            entity.BannerImageUrl = request.BannerImageUrl;

            repo.Update(entity);

            await _unitOfWork.SaveAsync(ct);

            _logger.LogInformation(
                "Event updated successfully: {EventId}",
                entity.Id);

            return Result<Guid>.Success(
                entity.Id,
                "Event updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating event");

            return Result<Guid>.Fail(ex.Message);
        }
    }
}
