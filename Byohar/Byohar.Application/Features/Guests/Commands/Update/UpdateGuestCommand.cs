using Byohar.Application.Interfaces.Persistance;
using Byohar.Application.Requests.Addresses;
using Byohar.Application.Requests.Guests;
using Byohar.Domain.Entities.Addresses;
using Byohar.Domain.Entities.Events;
using Byohar.Domain.Entities.Guests;
using Byohar.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Byohar.Application.Features.Guests.Commands.Update;

public class UpdateGuestCommand : GuestRequest, IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
}

public class UpdateGuestCommandHandler
    : IRequestHandler<UpdateGuestCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateGuestCommandHandler> _logger;

    public UpdateGuestCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateGuestCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        UpdateGuestCommand request,
        CancellationToken ct)
    {
        try
        {
            var guestRepo = _unitOfWork.Repository<Guest>();
            var addressRepo = _unitOfWork.Repository<Address>();

            var guest = await guestRepo
                .Entities()
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

            if (guest == null)
                return Result<Guid>.Fail("Guest not found.");

            if (request.EventId == Guid.Empty)
                return Result<Guid>.Fail("Event is required.");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                return Result<Guid>.Fail("Guest name is required.");

            bool eventExists = await _unitOfWork
                .Repository<Event>()
                .Entities()
                .AnyAsync(x => x.Id == request.EventId, ct);

            if (!eventExists)
                return Result<Guid>.Fail("Event not found.");

            guest.EventId = request.EventId;
            guest.FirstName = request.FirstName.Trim();
            guest.LastName = request.LastName?.Trim();
            guest.MobileNumber = request.MobileNumber;
            guest.Relation = request.Relation;
            guest.TotalMembers = request.TotalMembers <= 0 ? 1 : request.TotalMembers;
            guest.IsActive = request.IsActive;

            if (request.AddressId.HasValue && request.Address == null)
            {
                bool addressExists = await addressRepo
                    .Entities()
                    .AnyAsync(x => x.Id == request.AddressId.Value, ct);

                if (!addressExists)
                    return Result<Guid>.Fail("Address not found.");

                guest.AddressId = request.AddressId.Value;
            }
            else if (HasAddress(request.Address))
            {
                if (guest.AddressId.HasValue && guest.Address != null)
                {
                    guest.Address.AddressLine1 = request.Address!.AddressLine1;
                    guest.Address.AddressLine2 = request.Address.AddressLine2;
                    guest.Address.City = request.Address.City;
                    guest.Address.State = request.Address.State;
                    guest.Address.PinCode = request.Address.PinCode;
                    guest.Address.Country = string.IsNullOrWhiteSpace(request.Address.Country)
                        ? "India"
                        : request.Address.Country;

                    addressRepo.Update(guest.Address);
                }
                else
                {
                    var address = new Address
                    {
                        Id = Guid.NewGuid(),
                        AddressLine1 = request.Address!.AddressLine1,
                        AddressLine2 = request.Address.AddressLine2,
                        City = request.Address.City,
                        State = request.Address.State,
                        PinCode = request.Address.PinCode,
                        Country = string.IsNullOrWhiteSpace(request.Address.Country)
                            ? "India"
                            : request.Address.Country
                    };

                    addressRepo.Add(address);

                    guest.AddressId = address.Id;
                }
            }

            guestRepo.Update(guest);

            await _unitOfWork.SaveAsync(ct);

            _logger.LogInformation(
                "Guest updated successfully: {GuestId}",
                guest.Id);

            return Result<Guid>.Success(
                guest.Id,
                "Guest updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating guest");

            return Result<Guid>.Fail(ex.Message);
        }
    }

    private static bool HasAddress(AddressRequest? address)
    {
        return address != null &&
               (
                   !string.IsNullOrWhiteSpace(address.AddressLine1) ||
                   !string.IsNullOrWhiteSpace(address.AddressLine2) ||
                   !string.IsNullOrWhiteSpace(address.City) ||
                   !string.IsNullOrWhiteSpace(address.State) ||
                   !string.IsNullOrWhiteSpace(address.PinCode)
               );
    }
}