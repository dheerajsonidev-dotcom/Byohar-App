using AutoMapper;
using Byohar.Application.Interfaces.Persistance;
using Byohar.Application.Requests.Guests;
using Byohar.Domain.Entities.Addresses;
using Byohar.Domain.Entities.Events;
using Byohar.Domain.Entities.Guests;
using Byohar.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Byohar.Application.Features.Guests.Commands.Create;

public class CreateGuestCommand : GuestRequest, IRequest<Result<Guid>>
{
}

public class CreateGuestCommandHandler
    : IRequestHandler<CreateGuestCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateGuestCommandHandler> _logger;

    public CreateGuestCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateGuestCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateGuestCommand request,
        CancellationToken ct)
    {
        try
        {
            if (request.EventId == Guid.Empty)
                return Result<Guid>.Fail("Event is required.");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                return Result<Guid>.Fail("Guest name is required.");

            var eventExists = await _unitOfWork
                .Repository<Event>()
                .Entities()
                .AnyAsync(x => x.Id == request.EventId, ct);

            if (!eventExists)
                return Result<Guid>.Fail("Event not found.");

            Guid? addressId = request.AddressId;

            if (addressId.HasValue)
            {
                var addressExists = await _unitOfWork
                    .Repository<Address>()
                    .Entities()
                    .AnyAsync(x => x.Id == addressId.Value, ct);

                if (!addressExists)
                    return Result<Guid>.Fail("Address not found.");
            }
            else if (request.Address != null)
            {
                var address = _mapper.Map<Address>(request.Address);

                address.Id = Guid.NewGuid();

                _unitOfWork.Repository<Address>().Add(address);

                addressId = address.Id;
            }

            var guest = _mapper.Map<Guest>(request);

            guest.Id = Guid.NewGuid();
            guest.AddressId = addressId;
            guest.IsActive = request.IsActive;
            guest.CreatedOn = DateTime.UtcNow;

            _unitOfWork.Repository<Guest>().Add(guest);

            await _unitOfWork.SaveAsync(ct);

            _logger.LogInformation(
                "Guest created successfully: {GuestId}",
                guest.Id);

            return Result<Guid>.Success(
                guest.Id,
                "Guest created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while creating guest");

            return Result<Guid>.Fail(ex.Message);
        }
    }
}
