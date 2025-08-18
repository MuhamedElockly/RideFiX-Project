using Domain.Entities.CoreEntites.EmergencyEntities;
using Services.Specification_Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specification_Implementation.RequestSpecifications
{
    public class AcceptReverseRequestSpecification : Specification<TechReverseRequest, int>
    {
        public AcceptReverseRequestSpecification(int requestID) : base(s => s.Id == requestID)
        {
            AddInclude(s => s.EmergencyRequest);
            AddInclude(s => s.Technician);
            AddInclude(s => s.Technician.ApplicationUser);
            AddInclude(s => s.EmergencyRequest.CarOwner);
            AddInclude(s => s.EmergencyRequest.CarOwner.ApplicationUser);

        }
    }
}
