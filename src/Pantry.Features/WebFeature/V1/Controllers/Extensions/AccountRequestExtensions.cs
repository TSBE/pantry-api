using System;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class AccountRequestExtensions
{
    public static Account ToModelNotNull(this AccountRequest dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new Account
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
        };
    }
}
