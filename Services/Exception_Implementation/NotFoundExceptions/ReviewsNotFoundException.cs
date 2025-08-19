using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Exception_Implementation.NotFoundExceptions
{
    public class ReviewsNotFoundException:Exception
    {
        public ReviewsNotFoundException(string message) : base(message) { }
    }
}
