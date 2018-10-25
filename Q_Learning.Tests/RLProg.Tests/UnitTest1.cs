using System;
using Xunit;

namespace RLProg.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Func<double,AcountState, AcountState> Deposit = 
                (amount, s) => new AcountState { Amount = s.Amount + amount};

            Func<double, AcountState, AcountState> Withdraw =
                (amount, s) => new AcountState { Amount = s.Amount - amount };


            Func<double, AcountState, bool> Validate =
                (amount, s) => {
                    if (s.Amount - amount < 0) return false;
                    return true;
                };


            var myAccount = new AcountState
            {
                Amount = 100
            };

            // s = myAccount, a = Deposit
            // s' = (s, a)
            // s' = nextMyAcount
            var nextMyAcount = Deposit(200, myAccount);

            Assert.Equal(300, nextMyAcount.Amount);

            if(Validate(400, myAccount))
            {
                nextMyAcount = Withdraw(400, myAccount);
            }
        }
    }

    public struct AcountState
    {
        public double Amount { get; set; }
    }
}
