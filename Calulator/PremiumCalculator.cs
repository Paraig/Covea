using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calulator
{
    public class PremiumCalculator
    {
        private IReferenceData _referenceData;

        /// <summary>
        /// ctor - the reference data is injected so these values acan be changed
        /// without needing to modify the code
        /// </summary>
        /// <param name="referenceData"></param>
        public PremiumCalculator(IReferenceData referenceData)
        {
            _referenceData = referenceData;
            _referenceData.AgeRisks = _referenceData.AgeRisks.OrderBy(x => x.MaxAge);
            foreach(var ageRisk in _referenceData.AgeRisks)
            {
                ageRisk.SumAssuredRisk = ageRisk.SumAssuredRisk.OrderBy(x => x.Key);
            }
        }

        /// <summary>
        /// The calculation function
        /// </summary>
        /// <param name="age"></param>
        /// <param name="amount"></param>
        /// <param name="premium"></param>
        /// <returns>calculation status</returns>
        public CalculationSatus CalculatePremium(int age, ref int amount, ref decimal premium)
        {
            //validate the age/amount are within the supported limits 
            var  status = ValidateInput(age, amount);
            if (CalculationSatus.Success != status)
                return status;

            //calculate the risk rate
            var riskRate = GetRiskRate(age, amount);

            var riskPremium = riskRate * (decimal)(amount / 1000);

            var commission = riskPremium * (decimal)_referenceData.CommissionPercent / 100;

            var netPremium = riskPremium + commission;

            var initialCommission = netPremium * _referenceData.InitialCommissionPercent / 100;

            premium = netPremium + initialCommission;

            premium = Math.Round(premium, 2);

            // if the premium is below the threshold add 5000 to the amount until the premium reaches the threshold 
            if(premium < 2)
            {
                amount += 5000;
                CalculatePremium(age, ref amount, ref premium);
            }

            return CalculationSatus.Success;
        }

        private CalculationSatus ValidateInput(int age, int amount)
        {
            if (age < _referenceData.MinAge)
                return CalculationSatus.TooYoung;

            if (age > _referenceData.MaxAge)
                return CalculationSatus.TooOld;

            if (amount < _referenceData.MinSum)
                return CalculationSatus.AmountTooSmall;

            if (amount > _referenceData.MaxSum)
                return CalculationSatus.AmountTooBig;

            return CalculationSatus.Success;
        }

        private decimal GetRiskRate(int age, int amount)
        {
            decimal risk = -1.0M;

            //get the correct set of risk settings based on age
            var ageRisk = _referenceData.AgeRisks.FirstOrDefault(x => x.MaxAge >= age);
            if (ageRisk == null)
                return risk;

            //if we find an exact match between the amount and a risk sum assured use it 
            if(ageRisk.SumAssuredRisk.Any(x => x.Key == amount))
                risk = ageRisk.SumAssuredRisk.FirstOrDefault(x => x.Key == amount).Value;
            else // we need to inpolate
            {
                var lower = ageRisk.SumAssuredRisk.LastOrDefault(x => x.Key < amount);
                var upper = ageRisk.SumAssuredRisk.FirstOrDefault(x => x.Key > amount);

                risk = (((decimal)(amount - lower.Key)) * upper.Value + (decimal)(upper.Key - amount)  * lower.Value)
                    / (decimal)(upper.Key - lower.Key);
            }
            
            return risk;
            
        }
    }
}
