using Byohar.Application.Interfaces.Common;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses.Identity;
using Byohar.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Application.Interfaces.Identity;

public interface IOwnerAuthService : IService
{
    Task<IResult<OwnerRegisterResponse>> RegisterOwnerAsync(OwnerRegisterRequest request);
}
