using Domain.Entities.CoreEntites.EmergencyEntities;
using Services.Specification_Implementation;
using SharedData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specification_Implementation.RequestSpecifications
{
    public class EmergencyRequestTotalSpecification : Specification<EmergencyRequest, int>
    {
        public EmergencyRequestTotalSpecification(int id) : base(
            e => e.Id == id)
        {

            AddInclude(e => e.Technician);

        }
        public EmergencyRequestTotalSpecification() : base() 
        {
        }
        public EmergencyRequestTotalSpecification(RequestState requestState) : base(e=>e.EmergencyRequestTechnicians.Any(emr=>emr.CallStatus==requestState))
        {
        }
    }
}
