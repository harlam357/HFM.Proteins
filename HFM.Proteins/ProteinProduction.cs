namespace HFM.Proteins;

/// <summary>
/// Represents all protein production measurements.
/// </summary>
public readonly struct ProteinProduction : IEquatable<ProteinProduction>
{
    public ProteinProduction(double unitsPerDay, double bonusMultiplier, double credit, double pointsPerDay)
    {
        UnitsPerDay = unitsPerDay;
        BonusMultiplier = bonusMultiplier;
        Credit = credit;
        PointsPerDay = pointsPerDay;
    }

    /// <summary>
    /// Gets or sets the units per day (UPD) measurement.
    /// </summary>
    public double UnitsPerDay { get; }

    /// <summary>
    /// Gets or sets the bonus multiplier measurement.
    /// </summary>
    public double BonusMultiplier { get; }

    /// <summary>
    /// Gets or sets the work unit credit measurement.
    /// </summary>
    public double Credit { get; }

    /// <summary>
    /// Gets or sets the points per day (PPD) measurement.
    /// </summary>
    public double PointsPerDay { get; }

    public bool Equals(ProteinProduction other) => UnitsPerDay.Equals(other.UnitsPerDay) && BonusMultiplier.Equals(other.BonusMultiplier) && Credit.Equals(other.Credit) && PointsPerDay.Equals(other.PointsPerDay);

    public override bool Equals(object? obj) => obj is ProteinProduction other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = UnitsPerDay.GetHashCode();
            hashCode = (hashCode * 397) ^ BonusMultiplier.GetHashCode();
            hashCode = (hashCode * 397) ^ Credit.GetHashCode();
            hashCode = (hashCode * 397) ^ PointsPerDay.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(ProteinProduction left, ProteinProduction right) => left.Equals(right);

    public static bool operator !=(ProteinProduction left, ProteinProduction right) => !left.Equals(right);
}
