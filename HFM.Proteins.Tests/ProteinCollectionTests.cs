using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using NUnit.Framework;

namespace HFM.Proteins
{
    [TestFixture]
    public class ProteinCollectionTests
    {
        [Test]
        public void ProteinCollection_Ctor_ThrowsWhenProteinsIsNull()
        {
            _ = Assert.Throws<ArgumentNullException>(() => _ = new ProteinCollection(null));
        }

        [Test]
        public void ProteinCollection_Add_ThrowsWhenProteinIsNull()
        {
            _ = Assert.Throws<ArgumentNullException>(() => new ProteinCollection().Add(null));
        }

        [Test]
        public void ProteinCollection_Contains_ReturnsFalseWhenTheCollectionIsEmpty()
        {
            // Arrange
            var collection = new ProteinCollection();
            // Act
            bool result = collection.Contains(1);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ProteinCollection_Contains_ReturnsTrueWhenTheKeyExists()
        {
            // Arrange
            var collection = new ProteinCollection
            {
                CreateValidProtein(1)
            };
            // Act
            bool result = collection.Contains(1);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ProteinCollection_TryGetValue_ReturnsFalseWhenTheCollectionIsEmpty()
        {
            // Arrange
            var collection = new ProteinCollection();
            // Act
            bool result = collection.TryGetValue(1, out var protein);
            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(protein);
        }

        [Test]
        public void ProteinCollection_TryGetValue_ReturnsTrueWhenTheKeyExists()
        {
            // Arrange
            var collection = new ProteinCollection
            {
                CreateValidProtein(1)
            };
            // Act
            bool result = collection.TryGetValue(1, out var protein);
            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(protein);
        }

        [Test]
        public void ProteinCollection_Ctor_AddsValidProteins()
        {
            // Arrange
            var proteins = new List<Protein>
            {
                CreateValidProtein(1),
                CreateValidProtein(2),
                new Protein { ProjectNumber = 3 }
            };
            // Act
            var collection = new ProteinCollection(proteins);
            // Assert
            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(1, collection[1].ProjectNumber);
            Assert.AreEqual(2, collection[2].ProjectNumber);
        }

        [Test]
        public void ProteinCollection_Update_AddsValidProteins()
        {
            // Arrange
            var proteins = new List<Protein>
            {
                CreateValidProtein(1),
                CreateValidProtein(2),
                new Protein { ProjectNumber = 3 }
            };
            var collection = new ProteinCollection();
            // Act
            var changes = collection.Update(proteins);
            // Assert
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(1, changes[0].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.Add, changes[0].Action);
            Assert.IsNull(changes[0].PropertyChanges);
            Assert.AreEqual(2, changes[1].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.Add, changes[1].Action);
            Assert.IsNull(changes[1].PropertyChanges);
        }

        [Test]
        public void ProteinCollection_Update_ThrowsWhenProteinsIsNull()
        {
            _ = Assert.Throws<ArgumentNullException>(() => new ProteinCollection().Update(null));
        }

        [Test]
        public void ProteinCollection_Update_ReturnsCollectionChanges()
        {
            // Arrange
            var collection = CreateCollectionForUpdate();
            var proteins = CreateProteinsToUpdate();
            // Act
            var changes = collection.Update(proteins);
            // Assert
            Assert.AreEqual(4, changes.Count);

            // check index 0    
            Assert.AreEqual(1, changes[0].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.Property, changes[0].Action);

            var propertyChanges = changes[0].PropertyChanges;
            Assert.AreEqual(1, propertyChanges.Count);
            Assert.AreEqual("Credit", propertyChanges[0].PropertyName);
            Assert.AreEqual("1", propertyChanges[0].Previous);
            Assert.AreEqual("100", propertyChanges[0].Current);

            // check index 1
            Assert.AreEqual(2, changes[1].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.Property, changes[1].Action);

            propertyChanges = changes[1].PropertyChanges;
            Assert.AreEqual(2, propertyChanges.Count);
            Assert.AreEqual("MaximumDays", propertyChanges[0].PropertyName);
            Assert.AreEqual("1", propertyChanges[0].Previous);
            Assert.AreEqual("3", propertyChanges[0].Current);
            Assert.AreEqual("KFactor", propertyChanges[1].PropertyName);
            Assert.AreEqual("0", propertyChanges[1].Previous);
            Assert.AreEqual("26.4", propertyChanges[1].Current);

            // check index 2
            Assert.AreEqual(3, changes[2].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.None, changes[2].Action);
            Assert.IsNull(changes[2].PropertyChanges);

            // check index 3
            Assert.AreEqual(4, changes[3].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.Add, changes[3].Action);
            Assert.IsNull(changes[3].PropertyChanges);
        }

        [Test]
        public void ProteinCollection_Update_AltersCollectionContents()
        {
            // Arrange
            var collection = CreateCollectionForUpdate();
            var proteins = CreateProteinsToUpdate();
            // Act
            _ = collection.Update(proteins);
            // Assert
            Assert.AreEqual(4, collection.Count);

            // check project 1
            Assert.AreEqual(1, collection[1].ProjectNumber);
            Assert.AreEqual(100.0, collection[1].Credit);

            // check project 2
            Assert.AreEqual(2, collection[2].ProjectNumber);
            Assert.AreEqual(3, collection[2].MaximumDays);
            Assert.AreEqual(26.4, collection[2].KFactor);

            // check project 3
            Assert.AreEqual(3, collection[3].ProjectNumber);

            // check project 4
            Assert.AreEqual(4, collection[4].ProjectNumber);
        }

        [Test]
        public void ProteinCollection_Update_ReplacesProteinObjects()
        {
            // Arrange
            var collection = CreateCollectionForUpdate();
            var proteins = CreateProteinsToUpdate();
            // Act
            _ = collection.Update(proteins);
            // Assert
            Assert.AreEqual(4, collection.Count);
            // this is a reference equality check
            CollectionAssert.AreEqual(proteins, collection.ToList());
        }

        private static ProteinCollection CreateCollectionForUpdate()
        {
            return new ProteinCollection
            {
                CreateValidProtein(1),
                CreateValidProtein(2),
                CreateValidProtein(3)
            };
        }

        private static List<Protein> CreateProteinsToUpdate()
        {
            var proteins = new List<Protein>();

            var protein = CreateValidProtein(1);
            protein.Credit = 100;
            proteins.Add(protein);

            protein = CreateValidProtein(2);
            protein.MaximumDays = 3;
            protein.KFactor = 26.4;
            proteins.Add(protein);

            proteins.Add(CreateValidProtein(3));
            proteins.Add(CreateValidProtein(4));

            return proteins;
        }

        private static Protein CreateValidProtein(int projectNumber)
        {
            return new Protein { ProjectNumber = projectNumber, PreferredDays = 1, MaximumDays = 1, Credit = 1, Frames = 100 };
        }

        [Test]
        [Category(TestCategoryNames.Integration)]
        public void ProteinCollection_Ctor_FromProjectSummary()
        {
            // Arrange
            var client = new WebClient();
            using (var stream = new MemoryStream(client.DownloadData(ProjectSummaryUrl.Json)))
            {
                stream.Position = 0;
                var deserializer = new ProjectSummaryJsonDeserializer();
                var proteins = deserializer.Deserialize(stream);
                var collection = new ProteinCollection(proteins);
                Assert.IsTrue(collection.Count > 0);
            }
        }

        [Test]
        [Category(TestCategoryNames.Integration)]
        public async Task ProteinCollection_Ctor_FromProjectSummaryAsync()
        {
            // Arrange
            var client = new WebClient();
            using (var stream = new MemoryStream(await client.DownloadDataTaskAsync(ProjectSummaryUrl.Json)))
            {
                stream.Position = 0;
                var deserializer = new ProjectSummaryJsonDeserializer();
                var proteins = await deserializer.DeserializeAsync(stream);
                var collection = new ProteinCollection(proteins);
                Assert.IsTrue(collection.Count > 0);
            }
        }
    }
}
