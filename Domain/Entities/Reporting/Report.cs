using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.CoreEntites.EmergencyEntities;
using Domain.Entities.IdentityEntities;
using SharedData.Enums;

namespace Domain.Entities.Reporting
{
    public class Report : BaseEntity<int>
    {
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        #region Navigation Properties
        public string ReportingUserId { get; set; }
        public ApplicationUser ReportingUser { get; set; }

        public string ReportedUserId { get; set; }
        public ApplicationUser ReportedUser { get; set; }

        public int RequestId { get; set; }
        public EmergencyRequest Request { get; set; }
        public ReportState reportState { get; set; } = ReportState.Waiting;
        #endregion
    }
}
