using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.DTOs.TechnicianDTOs
{
    public class ReadTechnicianReviewDTO
    {
        public int Rate { get; set; }
        public string? Comment { get; set; }
        public string? CarOwnerName { get; set; }

        public string? TechnicianName { get; set; }
        public DateTime DateTime { get; set; }
      
    }
}
