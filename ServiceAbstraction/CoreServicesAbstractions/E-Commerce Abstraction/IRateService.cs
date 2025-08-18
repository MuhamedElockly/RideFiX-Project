using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.CoreServicesAbstractions.E_Commerce_Abstraction
{
    public interface IRateService
    {
        Task AddRateAsync(int productId, int value, string comment, string userId);
    }

}
