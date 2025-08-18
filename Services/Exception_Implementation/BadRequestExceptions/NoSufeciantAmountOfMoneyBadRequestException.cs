using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Exception_Implementation.BadRequestExceptions
{
    public class NoSufeciantAmountOfMoneyBadRequestException : BadRequestException
    {
        public NoSufeciantAmountOfMoneyBadRequestException()
            : base("You don't have sufficient amount of money to complete this transaction.")
        { }
    }
}
