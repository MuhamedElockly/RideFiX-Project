using Domain.Entities.CoreEntites.EmergencyEntities;
using Services.Specification_Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specification_Implementation.ChatSessionsSpecifications
{
    public class ChatSessionBetweenParticipantsSpec : Specification<ChatSession, int>
    {
        public ChatSessionBetweenParticipantsSpec(int carOwnerId, int technicianId)
            : base(cs => cs.CarOwnerId == carOwnerId && cs.TechnicianId == technicianId)
        {
            AddInclude(c => c.massages);
        }
    }
}
