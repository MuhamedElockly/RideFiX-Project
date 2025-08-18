using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.DTOs.MessegeDTOs
{
    public class ReadMessageDTO
    {
        public int MessageId { get; set; }
        public string? Text { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSeen { get; set; }

        public string? SenderId { get; set; }
        public string? SenderName { get; set; }
    }
}
