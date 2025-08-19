using SharedData.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.DTOs.ReportDtos
{
    public class UpdateReportDTO
    {
        public int ReportId { get; set; }
        [Range(2,3, ErrorMessage = "Status must be 2 ( Rejected) or 3 (Approved).")]
        public ReportState ReportState { get; set; }
    }
}
