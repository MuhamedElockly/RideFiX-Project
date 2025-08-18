using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.IdentityEntities;
using SharedData.DTOs.Account;

namespace ServiceAbstraction.CoreServicesAbstractions.Account
{
    public interface IJwtService
    {
        string generateToken (JwtTokenDto user , IList<string>roles , int roleEntityId);

    }
}
