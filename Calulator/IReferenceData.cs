using System.Collections.Generic;

namespace Calulator
{
    //These interfaces define the reference data, in reality this data is likely to come from a database
    //or configuration settings - there is no concrete type defined apart from a very simple definition in the 
    //test - this is intentional

    /// <summary>
    /// Interface for the refence data
    /// </summary>
    public interface IReferenceData
    {
        /// <summary>
        /// The min supported age (18 in the spec)
        /// </summary>
        int MinAge { get; set; }

        /// <summary>
        /// The max supported age (65 in the spec)
        /// </summary>
        int MaxAge { get; set; }

        /// <summary>
        /// The min supported sum assured (25000 in the spec)
        /// </summary>
        int MinSum { get; set; }

        /// <summary>
        /// The max supported sum assured (500000 in the spec)
        /// </summary>
        int MaxSum { get; set; }

        /// <summary>
        /// The commission percent (3% in the spec)
        /// </summary>
        int CommissionPercent { get; set; }

        /// <summary>
        /// The initial commission percent (205% in the spec)
        /// </summary>
        int InitialCommissionPercent { get; set; }

        /// <summary>
        /// The collection of risk data points (the data in the risk rate table in the spec)
        /// </summary>
        IEnumerable<IAgeRisk> AgeRisks { get; set; }  
    }

    /// <summary>
    /// The risk data - each object implementing this interface has the max age for the band and 
    /// under it a collection of KVP with the sum assured as key and the key rate as value
    /// </summary>
    public interface IAgeRisk
    {
        /// <summary>
        /// the max age for each segment 30,50, 65 (the max age)
        /// </summary>
        int MaxAge { get; set; }

        /// <summary>
        /// KVP sum assured as key, rate as value
        /// </summary>
        IEnumerable<KeyValuePair<int, decimal>>SumAssuredRisk { get; set; } 
    } 
}
