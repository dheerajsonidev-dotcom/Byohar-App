using Byohar.Application.Features.Events.Queries.GetAll;
using Byohar.Application.Specifications.Base;
using Byohar.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Specifications.Events;

internal class EventFilterSpecification : Specification<Event>
{
    public EventFilterSpecification(GetAllEventQuery request)
    {
        // 🔍 SEARCH
        if (!string.IsNullOrWhiteSpace(request.StringSearch))
        {
            var search = request.StringSearch.Trim();

            And(x =>
                (x.Title ?? "").Contains(search) ||
                (x.Description ?? "").Contains(search) ||
                (x.Location ?? "").Contains(search));
        }

        // 📅 FROM DATE
        if (request.FromDate.HasValue)
        {
            And(x => x.Date >= request.FromDate.Value.Date);
        }

        // 📅 TO DATE
        if (request.ToDate.HasValue)
        {
            var endDate = request.ToDate.Value.Date.AddDays(1).AddTicks(-1);

            And(x => x.Date <= endDate);
        }

        // ✅ ACTIVE / INACTIVE FILTER
        if (request.IsActive.HasValue)
        {
            And(x => x.IsActive == request.IsActive.Value);
        }

        // 📍 LOCATION FILTER
        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            var location = request.Location.Trim();

            And(x =>
                (x.Location ?? "").Contains(location));
        }

        // 🖼️ BANNER AVAILABLE FILTER
        if (request.HasBanner.HasValue)
        {
            if (request.HasBanner.Value)
            {
                And(x =>
                    !string.IsNullOrWhiteSpace(x.BannerImageUrl));
            }
            else
            {
                And(x =>
                    string.IsNullOrWhiteSpace(x.BannerImageUrl));
            }
        }

        // 🏢 TENANT FILTER
        if (request.TenantId.HasValue)
        {
            And(x => x.TenantId == request.TenantId.Value);
        }
    }
}