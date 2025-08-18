using Domain.Entities.CoreEntites.EmergencyEntities;
using Microsoft.Extensions.Primitives;
using Services.Specification_Implementation;
using SharedData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specification_Implementation.TechnicianSpecifications
{
    public class TechniciansSpecification : Specification<Technician, int>
    {
        public TechniciansSpecification
            (TimeOnly currentTime, Government state, int categoryId)
            : base(t => currentTime >= t.StartWorking
            && currentTime <= t.EndWorking
            && t.government == state
            && t.TCategories.Any(c => categoryId == c.Id))
        {
            AddInclude(t => t.TCategories);
            AddInclude(t => t.ApplicationUser);
            AddInclude(t => t.reviews);

        }
        public TechniciansSpecification() : base()
        {
            AddInclude(t => t.ApplicationUser);
        }

    }
}
