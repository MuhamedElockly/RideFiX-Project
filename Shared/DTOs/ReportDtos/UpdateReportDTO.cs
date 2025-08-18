using SharedData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.DTOs.ReportDtos
{
    public class UpdateReportDTO
    {
        public int ReportId { get; set; }
        public ReportState ReportState { get; set; }
    }
}
