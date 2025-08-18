
using Domain.Entities.CoreEntites.EmergencyEntities;
using Services.Specification_Implementation;
using SharedData.DTOs.RequestsDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specification_Implementation.CarOwnerSpecifications
{
    public class CarOwnerSpecification : Specification<CarOwner, int>
    {
        public CarOwnerSpecification(CreatePreRequestDTO request)
            : base(co => co.Id == request.CarOwnerId)
        {
            AddInclude(co => co.ApplicationUser);
        }
        public CarOwnerSpecification() : base()
        {
            AddInclude(co => co.ApplicationUser);
        }
        public CarOwnerSpecification(DateTime firstDayOfMonth) : base(c=>c.ApplicationUser.CreatedAt>= firstDayOfMonth)
        {

        }
        public CarOwnerSpecification(DateTime startDate,DateTime endDate) : base(c => c.ApplicationUser.CreatedAt >= startDate && c.ApplicationUser.CreatedAt < endDate)
        {

        }
    }

}
