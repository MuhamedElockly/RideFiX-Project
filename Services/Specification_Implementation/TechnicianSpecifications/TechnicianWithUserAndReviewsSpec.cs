using Domain.Entities.CoreEntites.EmergencyEntities;
using Services.Specification_Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specification_Implementation.TechnicianSpecifications
{
    public class TechnicianWithUserAndReviewsSpec : Specification<Technician, int>
    {
        public TechnicianWithUserAndReviewsSpec():base()
        {
            AddInclude(t => t.reviews);
            AddInclude(t => t.ApplicationUser);
        }

        public TechnicianWithUserAndReviewsSpec(int userId) : base(t => t.Id == userId)
        {
            {
               
                AddInclude(t => t.ApplicationUser);
               
                
                
            }
        }
    }
}
