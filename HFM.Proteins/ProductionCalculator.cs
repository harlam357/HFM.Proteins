namespace HFM.Proteins;

/// <summary>
/// Measures work unit (protein) production based on frame time, work unit information, and the unit completion time.
/// </summary>
public static class ProductionCalculator
{
    private const int MaxDecimalPlaces = 5;

    /// <summary>
    /// Calculates the units per day measurement based the given frame time and number of frames.
    /// </summary>
    /// <param name="frameTime">The work unit frame time.</param>
    /// <param name="frames">The number of frames in the work unit.</param>
    /// <returns>The units per day for the work unit.</returns>
    public static double CalculateUnitsPerDay(TimeSpan frameTime, int frames)
    {
        double totalTime = frameTime.TotalSeconds * frames;
        if (totalTime <= 0.0)
        {
            return 0.0;
        }
        return frameTime.Equals(TimeSpan.Zero) ? 0.0 : 86400 / totalTime;
    }

    /// <summary>
    /// Calculates the production bonus multiplier.
    /// </summary>
    /// <param name="kFactor">The KFactor assigned to the work unit.</param>
    /// <param name="preferredDays">The preferred deadline (in decimal days).</param>
    /// <param name="maximumDays">The final deadline (in decimal days).</param>
    /// <param name="unitTime">The overall unit completion time.</param>
    /// <returns>The production bonus multiplier.</returns>
    public static double CalculateBonusMultiplier(double kFactor, double preferredDays, double maximumDays, TimeSpan unitTime)
    {
        if (kFactor > 0 && unitTime > TimeSpan.Zero)
        {
            if (unitTime <= TimeSpan.FromDays(preferredDays))
            {
                return Math.Round(Math.Sqrt(maximumDays * kFactor / unitTime.TotalDays), MaxDecimalPlaces);
            }
        }
        return 1.0;
    }

    /// <summary>
    /// Calculates the credit measurement based the given work unit information and the unit completion time.
    /// </summary>
    /// <param name="credit">The base credit assigned to the work unit.</param>
    /// <param name="kFactor">The KFactor assigned to the work unit.</param>
    /// <param name="preferredDays">The preferred deadline (in decimal days).</param>
    /// <param name="maximumDays">The final deadline (in decimal days).</param>
    /// <param name="unitTime">The overall unit completion time.</param>
    /// <returns>The credit for the work unit.</returns>
    public static double CalculateBonusCredit(double credit, double kFactor, double preferredDays, double maximumDays, TimeSpan unitTime)
    {
        double bonusMulti = CalculateBonusMultiplier(kFactor, preferredDays, maximumDays, unitTime);
        return Math.Round(credit * bonusMulti, MaxDecimalPlaces);
    }

    /// <summary>
    /// Calculates the points per day measurement based the given frame time and work unit credit.
    /// </summary>
    /// <param name="frameTime">The work unit frame time.</param>
    /// <param name="frames">The number of frames in the work unit.</param>
    /// <param name="credit">The base credit assigned to the work unit.</param>
    /// <returns>The points per day for the work unit.</returns>
    public static double CalculatePointsPerDay(TimeSpan frameTime, int frames, double credit)
    {
        if (frameTime.Equals(TimeSpan.Zero)) return 0;

        double basePpd = CalculateUnitsPerDay(frameTime, frames) * credit;
        return Math.Round(basePpd, MaxDecimalPlaces);
    }

    /// <summary>
    /// Calculates the points per day measurement based the given frame time, work unit information, and the unit completion time.
    /// </summary>
    /// <param name="frameTime">The work unit frame time.</param>
    /// <param name="frames">The number of frames in the work unit.</param>
    /// <param name="credit">The base credit assigned to the work unit.</param>
    /// <param name="kFactor">The KFactor assigned to the work unit.</param>
    /// <param name="preferredDays">The preferred deadline (in decimal days).</param>
    /// <param name="maximumDays">The final deadline (in decimal days).</param>
    /// <param name="unitTime">The overall unit completion time.</param>
    /// <returns>The points per day for the work unit.</returns>
    public static double CalculateBonusPointsPerDay(TimeSpan frameTime, int frames, double credit, double kFactor, double preferredDays, double maximumDays, TimeSpan unitTime)
    {
        if (frameTime.Equals(TimeSpan.Zero)) return 0;

        double basePpd = CalculateUnitsPerDay(frameTime, frames) * credit;
        double bonusMulti = CalculateBonusMultiplier(kFactor, preferredDays, maximumDays, unitTime);
        return Math.Round(basePpd * bonusMulti, MaxDecimalPlaces);
    }

    /// <summary>
    /// Calculates all protein production measurements based the given frame time, work unit information, and the unit completion time.
    /// </summary>
    /// <param name="frameTime">The work unit frame time.</param>
    /// <param name="frames">The number of frames in the work unit.</param>
    /// <param name="credit">The base credit assigned to the work unit.</param>
    /// <param name="kFactor">The KFactor assigned to the work unit.</param>
    /// <param name="preferredDays">The preferred deadline (in decimal days).</param>
    /// <param name="maximumDays">The final deadline (in decimal days).</param>
    /// <param name="unitTime">The overall unit completion time.</param>
    /// <returns>The production measurements for the work unit.</returns> 
    public static ProteinProduction CalculateProteinProduction(TimeSpan frameTime, int frames, double credit, double kFactor, double preferredDays, double maximumDays, TimeSpan unitTime) =>
        new(CalculateUnitsPerDay(frameTime, frames),
            CalculateBonusMultiplier(kFactor, preferredDays, maximumDays, unitTime),
            CalculateBonusCredit(credit, kFactor, preferredDays, maximumDays, unitTime),
            CalculateBonusPointsPerDay(frameTime, frames, credit, kFactor, preferredDays, maximumDays, unitTime));
}
