using Byohar.Application.Features.Guests.Queries;
using Byohar.Application.Specifications.Base;
using Byohar.Domain.Entities.Guests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Specifications.Guests
{
    internal class GuestFilterSpecification : Specification<Guest>
    {
        public GuestFilterSpecification(GetAllGuestQuery request)
        {
            if (!string.IsNullOrWhiteSpace(request.StringSearch))
            {
                var search = request.StringSearch.Trim();

                And(x =>
                    (x.FirstName ?? "").Contains(search) ||
                    (x.LastName ?? "").Contains(search) ||
                    (x.MobileNumber ?? "").Contains(search) ||
                    (x.Relation ?? "").Contains(search) ||
                    (x.Address != null && (
                        (x.Address.City ?? "").Contains(search) ||
                        (x.Address.State ?? "").Contains(search) ||
                        (x.Address.PinCode ?? "").Contains(search)
                    ))
                );
            }

            if (request.EventId.HasValue)
            {
                And(x => x.EventId == request.EventId.Value);
            }

            if (request.IsActive.HasValue)
            {
                And(x => x.IsActive == request.IsActive.Value);
            }

            //if (request.FromDate.HasValue)
            //{
            //    And(x => x.CreatedOn >= request.FromDate.Value);
            //}

            //if (request.ToDate.HasValue)
            //{
            //    var endDate = request.ToDate.Value.Date.AddDays(1).AddTicks(-1);
            //    And(x => x.CreatedOn <= endDate);
            //}
        }
    }
}