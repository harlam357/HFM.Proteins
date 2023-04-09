namespace HFM.Proteins;

/// <summary>
/// Provides extensions to the <see cref="Protein"/> class to assist with measuring work unit (protein) production with the <see cref="ProductionCalculator"/>.
/// </summary>
public static class ProteinProductionExtensions
{
    /// <summary>
    /// Calculates the units per day measurement based the given frame time and number of frames.
    /// </summary>
    /// <param name="protein"></param>
    /// <param name="frameTime">The work unit frame time.</param>
    /// <returns>The units per day for the work unit.</returns>
    public static double CalculateUnitsPerDay(this Protein protein, TimeSpan frameTime) =>
        ProductionCalculator.CalculateUnitsPerDay(frameTime, protein.Frames);

    /// <summary>
    /// Calculates the production bonus multiplier.
    /// </summary>
    /// <param name="protein"></param>
    /// <param name="unitTime">The overall unit completion time.</param>
    /// <returns>The production bonus multiplier.</returns>
    public static double CalculateBonusMultiplier(this Protein protein, TimeSpan unitTime) =>
        ProductionCalculator.CalculateBonusMultiplier(protein.KFactor, protein.PreferredDays, protein.MaximumDays, unitTime);

    /// <summary>
    /// Calculates the credit measurement based the given work unit information and the unit completion time.
    /// </summary>
    /// <param name="protein"></param>
    /// <param name="unitTime">The overall unit completion time.</param>
    /// <returns>The credit for the work unit.</returns>
    public static double CalculateBonusCredit(this Protein protein, TimeSpan unitTime) =>
        ProductionCalculator.CalculateBonusCredit(protein.Credit, protein.KFactor, protein.PreferredDays, protein.MaximumDays, unitTime);

    /// <summary>
    /// Calculates the points per day measurement based the given frame time and work unit credit.
    /// </summary>
    /// <param name="protein"></param>
    /// <param name="frameTime">The work unit frame time.</param>
    /// <returns>The points per day for the work unit.</returns>
    public static double CalculatePointsPerDay(this Protein protein, TimeSpan frameTime) =>
        ProductionCalculator.CalculatePointsPerDay(frameTime, protein.Frames, protein.Credit);

    /// <summary>
    /// Calculates the points per day measurement based the given frame time, work unit information, and the unit completion time.
    /// </summary>
    /// <param name="protein"></param>
    /// <param name="frameTime">The work unit frame time.</param>
    /// <param name="unitTime">The overall unit completion time.</param>
    /// <returns>The points per day for the work unit.</returns>
    public static double CalculateBonusPointsPerDay(this Protein protein, TimeSpan frameTime, TimeSpan unitTime) =>
        ProductionCalculator.CalculateBonusPointsPerDay(frameTime, protein.Frames, protein.Credit, protein.KFactor, protein.PreferredDays, protein.MaximumDays, unitTime);

    /// <summary>
    /// Calculates all production measurements based the given frame time, work unit information, and the unit completion time.
    /// </summary>
    /// <param name="protein"></param>
    /// <param name="frameTime">The work unit frame time.</param>
    /// <param name="unitTime">The overall unit completion time.</param>
    /// <returns>The production measurements for the work unit.</returns> 
    public static ProteinProduction CalculateProteinProduction(this Protein protein, TimeSpan frameTime, TimeSpan unitTime) =>
        ProductionCalculator.CalculateProteinProduction(frameTime, protein.Frames, protein.Credit, protein.KFactor, protein.PreferredDays, protein.MaximumDays, unitTime);
}
