using System.Collections.Generic;

using NUnit.Framework;

namespace HFM.Proteins
{
    [TestFixture]
    public class ProteinChangeTests
    {
        [Test]
        public void ProteinChange_EnumeratesPropertyChangesToReadOnlyList()
        {
            // Arrange
            var propertyChanges = EnumeratePropertyChanges();
            // Act
            var change = ProteinChange.Property(1, propertyChanges);
            // Assert
            Assert.AreEqual(1, change.PropertyChanges.Count);
        }

        private static IEnumerable<ProteinPropertyChange> EnumeratePropertyChanges()
        {
            yield return new ProteinPropertyChange("Foo", "Fizz", "Bizz");
        }
    }
}
