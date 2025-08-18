using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.DTOs.Account
{
    public class LoginResultDto
    {
        public string Token { get; set; }
        public bool RequiresProfilePic { get; set; }
        public bool IsBanned { get; set; }
    }
}
