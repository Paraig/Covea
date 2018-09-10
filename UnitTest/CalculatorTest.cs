using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Calulator;
using Moq;

namespace UnitTest
{
    [TestFixture]
    public class CalculatorTest
    {
        // private POCO to use a minimal implementation of the reference age risk data
        private class AgeRisk : IAgeRisk
        {
            public int MaxAge { get; set; }

            public IEnumerable<KeyValuePair<int, decimal>> SumAssuredRisk { get; set; }
        }

        private IReferenceData _referenceData;
        [SetUp]
        public void TestSetup()
        {
            //create mock refernce data object
            var referenceData = new Mock<IReferenceData>();
            referenceData.Setup(x => x.MinAge).Returns(18);
            referenceData.Setup(x => x.MaxAge).Returns(65);
            referenceData.Setup(x => x.MinSum).Returns(25000);
            referenceData.Setup(x => x.MaxSum).Returns(500000);
            referenceData.Setup(x => x.CommissionPercent).Returns(3);
            referenceData.Setup(x => x.InitialCommissionPercent).Returns(205);

            var list = new List<AgeRisk>
            {
                new AgeRisk()
                {
                    MaxAge = 30,
                    SumAssuredRisk = new List<KeyValuePair<int, decimal>>
                    {
                        new KeyValuePair<int, decimal>(25000, .0172M),
                        new KeyValuePair<int, decimal>(50000, .0165M),
                        new KeyValuePair<int, decimal>(100000, .0154M),
                        new KeyValuePair<int, decimal>(200000, .0147M),
                        new KeyValuePair<int, decimal>(300000, .0144M),
                        new KeyValuePair<int, decimal>(500000, .0146M)
                    }
                },
                new AgeRisk()
                {
                    MaxAge = 50,
                    SumAssuredRisk = new List<KeyValuePair<int, decimal>>
                    {
                        new KeyValuePair<int, decimal>(25000, .1043M),
                        new KeyValuePair<int, decimal>(50000, .0999M),
                        new KeyValuePair<int, decimal>(100000, .0932M),
                        new KeyValuePair<int, decimal>(200000, .0887M),
                        new KeyValuePair<int, decimal>(300000, .0872M)
                    }
                },
                new AgeRisk()
                {
                    MaxAge = 65,
                    SumAssuredRisk = new List<KeyValuePair<int, decimal>>
                    {
                        new KeyValuePair<int, decimal>(25000, .2677M),
                        new KeyValuePair<int, decimal>(50000, .2565M),
                        new KeyValuePair<int, decimal>(100000, .2393M),
                        new KeyValuePair<int, decimal>(200000, .2285M)
                    }
                }
            };

            referenceData.Setup(x => x.AgeRisks).Returns(list); 

            _referenceData = referenceData.Object;
        }

        [TestCase(17, 25000, CalculationSatus.TooYoung, 0)]
        [TestCase(67, 25000, CalculationSatus.TooOld, 0)]
        [TestCase(65, 25000, CalculationSatus.Success, 21.02)]
        [TestCase(65, 24999, CalculationSatus.AmountTooSmall, 0)]
        [TestCase(65, 500001, CalculationSatus.AmountTooBig, 0)]
        [TestCase(18, 25000, CalculationSatus.Success, 2.11)]
        [TestCase(30, 50000, CalculationSatus.Success, 2.59)]
        [TestCase(49, 60000, CalculationSatus.Success, 18.58)]
        public void TestCalculation(int age, int amount, CalculationSatus expectedStatus, decimal expectedPremium)
        {
            var calculator = new PremiumCalculator(_referenceData);

            decimal premium = 0M;

            var result = calculator.CalculatePremium(age, ref amount, ref premium);
            Assert.That(result == expectedStatus);
            Assert.That(premium == expectedPremium);
        }
    }
}
