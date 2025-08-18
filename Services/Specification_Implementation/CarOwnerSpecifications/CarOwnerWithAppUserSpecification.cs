using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.CoreEntites.EmergencyEntities;
using Services.Specification_Implementation;
using SharedData.DTOs.RequestsDTOs;

namespace Service.Specification_Implementation.CarOwnerSpecifications
{
    public class CarOwnerWithAppUserSpecification : Specification<CarOwner , int>
    {
        public CarOwnerWithAppUserSpecification(int CarOwnerId)
    : base(co => co.Id == CarOwnerId)
        {
            AddInclude(co => co.ApplicationUser);
        }
    }
}
