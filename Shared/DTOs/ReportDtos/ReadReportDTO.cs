using SharedData.DTOs.MessegeDTOs;
using SharedData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.DTOs.ReportDtos
{
    public class ReadReportDTO
    {
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public ReportState ReportState { get; set; }
        public string? ReportingUserId { get; set; }
        public string? ReportingUserRole { get; set; }
        public int? ReportingEntityId { get; set; }
        
        public string? ReportedUserId { get; set; }
        public string? ReportedUserRole { get; set; }
        public int? ReportedEntityId { get; set; }
        public int ReportId { get; set; }
        public string? TechnicianName { get; set; }
        public string? CarOwnerName { get; set; }

        public ICollection<ReadMessageDTO> Messages { get; set; } = new HashSet<ReadMessageDTO>();
       

    }
}
