using NUnit.Framework;

namespace HFM.Proteins;

[TestFixture]
public class ProteinProductionExtensionsTests
{
    [Test]
    public void ProteinProductionExtensions_CalculateUnitsPerDay_Test()
    {
        var protein = new Protein();
        var frameTime = TimeSpan.FromMinutes(5);
        Assert.AreEqual(2.88, protein.CalculateUnitsPerDay(frameTime));
    }

    [Test]
    public void ProteinProductionExtensions_CalculateBonusMultiplier_Test()
    {
        var protein = new Protein { KFactor = 26.4, PreferredDays = 3.0, MaximumDays = 5.0 };
        var unitTime = TimeSpan.FromMinutes(5 * 100);
        Assert.AreEqual(19.5, protein.CalculateBonusMultiplier(unitTime), 0.01);
    }

    [Test]
    public void ProteinProductionExtensions_CalculateBonusCredit_ReturnsCreditWithBonus_Test()
    {
        var protein = new Protein { Credit = 700.0, KFactor = 26.4, PreferredDays = 3.0, MaximumDays = 5.0 };
        var unitTime = TimeSpan.FromMinutes(5 * 100);
        Assert.AreEqual(13648.383, protein.CalculateBonusCredit(unitTime));
    }

    [Test]
    public void ProteinProductionExtensions_CalculatePointsPerDay_ReturnsPPDWithNoBonus_Test()
    {
        var protein = new Protein { Credit = 500.0 };
        var frameTime = TimeSpan.FromMinutes(5);
        Assert.AreEqual(1440.0, protein.CalculatePointsPerDay(frameTime));
    }

    [Test]
    public void ProteinProductionExtensions_CalculateBonusPointsPerDay_ReturnsPPDWithBonus_Test()
    {
        var protein = new Protein { Credit = 700.0, KFactor = 26.4, PreferredDays = 3.0, MaximumDays = 5.0 };
        var frameTime = TimeSpan.FromMinutes(5);
        var unitTime = TimeSpan.FromMinutes(5 * 100);
        Assert.AreEqual(39307.35, protein.CalculateBonusPointsPerDay(frameTime, unitTime), 0.01);
    }

    [Test]
    public void ProteinProductionExtensions_CalculateProteinProduction_WithBonus_Test()
    {
        var protein = new Protein { Credit = 700.0, KFactor = 26.4, PreferredDays = 3.0, MaximumDays = 5.0 };
        var frameTime = TimeSpan.FromMinutes(5);
        var unitTime = TimeSpan.FromMinutes(5 * 100);
        var values = protein.CalculateProteinProduction(frameTime, unitTime);
        Assert.AreEqual(2.88, values.UnitsPerDay);
        Assert.AreEqual(19.5, values.BonusMultiplier, 0.01);
        Assert.AreEqual(13648.383, values.Credit);
        Assert.AreEqual(39307.35, values.PointsPerDay, 0.01);
    }
}
