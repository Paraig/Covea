namespace Calulator
{
    /// <summary>
    /// Enum of calculation status returned - one extension might be to 
    /// make them into bit fields so they could be combined if there
    /// were multiple validation issues
    /// </summary>
    public enum CalculationSatus
    {
        Success,
        TooYoung,
        TooOld,
        AmountTooSmall,
        AmountTooBig
    }
}
