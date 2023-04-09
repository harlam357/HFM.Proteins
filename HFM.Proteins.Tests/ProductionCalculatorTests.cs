using NUnit.Framework;

namespace HFM.Proteins;

[TestFixture]
public class ProductionCalculatorTests
{
    [Test]
    public void ProductionCalculator_CalculateUnitsPerDay_ReturnsZeroWhenFrameTimeIsZero_Test()
    {
        var frameTime = TimeSpan.Zero;
        Assert.AreEqual(0.0, ProductionCalculator.CalculateUnitsPerDay(frameTime, 100));
    }

    [Test]
    public void ProductionCalculator_CalculateUnitsPerDay_Test()
    {
        var frameTime = TimeSpan.FromMinutes(5);
        Assert.AreEqual(2.88, ProductionCalculator.CalculateUnitsPerDay(frameTime, 100));
    }

    [Test]
    public void ProductionCalculator_CalculateBonusMultiplier_ReturnsOneWhenKFactorIsZero_Test()
    {
        var unitTime = TimeSpan.FromMinutes(5 * 100);
        Assert.AreEqual(1.0, ProductionCalculator.CalculateBonusMultiplier(0, 3.0, 5.0, unitTime), 0.01);
    }

    [Test]
    public void ProductionCalculator_CalculateBonusMultiplier_ReturnsOneWhenUnitTimeIsZero_Test()
    {
        var unitTime = TimeSpan.Zero;
        Assert.AreEqual(1.0, ProductionCalculator.CalculateBonusMultiplier(26.4, 3.0, 5.0, unitTime), 0.01);
    }

    [Test]
    public void ProductionCalculator_CalculateBonusMultiplier_ReturnsOneWhenUnitTimeGreaterThanPreferredTime_Test()
    {
        var unitTime = TimeSpan.FromDays(4);
        Assert.AreEqual(1.0, ProductionCalculator.CalculateBonusMultiplier(26.4, 3.0, 5.0, unitTime), 0.01);
    }

    [Test]
    public void ProductionCalculator_CalculateBonusMultiplier_Test()
    {
        var unitTime = TimeSpan.FromMinutes(5 * 100);
        Assert.AreEqual(19.5, ProductionCalculator.CalculateBonusMultiplier(26.4, 3.0, 5.0, unitTime), 0.01);
    }

    [Test]
    public void ProductionCalculator_CalculateBonusCredit_ReturnsCreditWithNoBonusWhenUnitTimeIsZero_Test()
    {
        var unitTime = TimeSpan.Zero;
        Assert.AreEqual(700, ProductionCalculator.CalculateBonusCredit(700, 26.4, 3.0, 5.0, unitTime));
    }

    [Test]
    public void ProductionCalculator_CalculateBonusCredit_ReturnsCreditWithBonus_Test()
    {
        var unitTime = TimeSpan.FromMinutes(5 * 100);
        Assert.AreEqual(13648.383, ProductionCalculator.CalculateBonusCredit(700, 26.4, 3.0, 5.0, unitTime));
    }

    [Test]
    public void ProductionCalculator_CalculatePointsPerDay_ReturnsZeroWhenFrameTimeIsZero_Test()
    {
        var frameTime = TimeSpan.Zero;
        Assert.AreEqual(0.0, ProductionCalculator.CalculatePointsPerDay(frameTime, 0, 0.0));
    }

    [Test]
    public void ProductionCalculator_CalculatePointsPerDay_ReturnsPPDWithNoBonus_Test()
    {
        var frameTime = TimeSpan.FromMinutes(5);
        Assert.AreEqual(1440.0, ProductionCalculator.CalculatePointsPerDay(frameTime, 100, 500.0));
    }

    [Test]
    public void ProductionCalculator_CalculateBonusPointsPerDay_ReturnsZeroWhenFrameTimeIsZero_Test()
    {
        var frameTime = TimeSpan.Zero;
        var unitTime = TimeSpan.Zero;
        Assert.AreEqual(0.0, ProductionCalculator.CalculateBonusPointsPerDay(frameTime, 0, 0.0, 0.0, 0.0, 0.0, unitTime));
    }

    [Test]
    public void ProductionCalculator_CalculateBonusPointsPerDay_ReturnsPPDWithNoBonusWhenUnitTimeIsZero_Test()
    {
        var frameTime = TimeSpan.FromMinutes(5);
        var unitTime = TimeSpan.Zero;
        Assert.AreEqual(1440.0, ProductionCalculator.CalculateBonusPointsPerDay(frameTime, 100, 500.0, 0.0, 0.0, 0.0, unitTime));
    }

    [Test]
    public void ProductionCalculator_CalculateBonusPointsPerDay_ReturnsPPDWithBonus_Test()
    {
        var frameTime = TimeSpan.FromMinutes(5);
        var unitTime = TimeSpan.FromMinutes(5 * 100);
        Assert.AreEqual(39307.35, ProductionCalculator.CalculateBonusPointsPerDay(frameTime, 100, 700.0, 26.4, 3.0, 5.0, unitTime), 0.01);
    }

    [Test]
    public void ProductionCalculator_CalculateProteinProduction_NoBonus_Test()
    {
        var frameTime = TimeSpan.FromMinutes(5);
        var unitTime = TimeSpan.Zero;
        var values = ProductionCalculator.CalculateProteinProduction(frameTime, 100, 700.0, 26.4, 3.0, 5.0, unitTime);
        Assert.AreEqual(2.88, values.UnitsPerDay);
        Assert.AreEqual(1.0, values.BonusMultiplier);
        Assert.AreEqual(700.0, values.Credit);
        Assert.AreEqual(2016.0, values.PointsPerDay);
    }

    [Test]
    public void ProductionCalculator_CalculateProteinProduction_WithBonus_Test()
    {
        var frameTime = TimeSpan.FromMinutes(5);
        var unitTime = TimeSpan.FromMinutes(5 * 100);
        var values = ProductionCalculator.CalculateProteinProduction(frameTime, 100, 700.0, 26.4, 3.0, 5.0, unitTime);
        Assert.AreEqual(2.88, values.UnitsPerDay);
        Assert.AreEqual(19.5, values.BonusMultiplier, 0.01);
        Assert.AreEqual(13648.383, values.Credit);
        Assert.AreEqual(39307.35, values.PointsPerDay, 0.01);
    }
}
