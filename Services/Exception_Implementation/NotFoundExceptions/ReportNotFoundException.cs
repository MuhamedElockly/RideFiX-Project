using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Exception_Implementation.NotFoundExceptions
{
    public class ReportNotFoundException:Exception
    {
        public ReportNotFoundException(string message):base(message) { }
    }
}
