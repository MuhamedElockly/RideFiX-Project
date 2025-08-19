using Domain.Entities.CoreEntites.EmergencyEntities;
using Services.Specification_Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specification_Implementation.ReviewSpecification
{
    public class ReviewSpecification:Specification<Review,int>
    {
        public ReviewSpecification(int technicianId) : base(r=>r.TechnicianId==technicianId)
        {
           
            AddInclude(r => r.CarOwner.ApplicationUser);
            AddInclude(r => r.Technician.ApplicationUser);
           
        }
      
    }
}
