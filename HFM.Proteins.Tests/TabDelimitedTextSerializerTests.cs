using System.Text;

using DeepEqual.Syntax;

using NUnit.Framework;

namespace HFM.Proteins;

[TestFixture]
public class TabDelimitedTextSerializerTests
{
    private TabDelimitedTextSerializer? _serializer;
    private ICollection<Protein>? _expected;

    [OneTimeSetUp]
    public void BeforeAll()
    {
        _serializer = new();
        using var stream = File.OpenRead("TestFiles\\ProjectInfo.tab");
        _expected = _serializer!.Deserialize(stream);
    }

    [Test]
    public void TabDelimitedTextSerializer_Deserialize_Test()
    {
        using var stream = File.OpenRead("TestFiles\\ProjectInfo.tab");
        var collection = _serializer!.Deserialize(stream);

        Assert.AreEqual(1409, collection.Count);
        _expected.ShouldDeepEqual(collection);
    }

    [Test]
    public async Task TabDelimitedTextSerializer_DeserializeAsync_Test()
    {
        await using var stream = File.OpenRead("TestFiles\\ProjectInfo.tab");
        var collection = await _serializer!.DeserializeAsync(stream);

        Assert.AreEqual(1409, collection.Count);
        _expected.ShouldDeepEqual(collection);
    }

    [Test]
    public void TabDelimitedTextSerializer_Deserialize_FromEmptyStream_Test()
    {
        using var stream = new MemoryStream();
        var proteins = _serializer!.Deserialize(stream);
        Assert.AreEqual(0, proteins.Count);
    }

    [Test]
    public void TabDelimitedTextSerializer_Serialize_Test()
    {
        var collection = CreateCollectionForSerialize();

        var buffer = new byte[256];
        using var stream = new MemoryStream(buffer);
        _serializer!.Serialize(stream, collection);

        string text = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        Assert.IsTrue(text.Length > 0 && text.Length < buffer.Length);
    }

    [Test]
    public async Task TabDelimitedTextSerializer_SerializeAsync_Test()
    {
        var collection = CreateCollectionForSerialize();

        var buffer = new byte[256];
        await using var stream = new MemoryStream(buffer);
        await _serializer!.SerializeAsync(stream, collection);

        string text = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        Assert.IsTrue(text.Length > 0 && text.Length < buffer.Length);
    }

    private static List<Protein> CreateCollectionForSerialize()
    {
        var collection = new List<Protein>
        {
            new()
            {
                ProjectNumber = 6900,
                ServerIP = "1.2.3.4",
                WorkUnitName = "Name of Work Unit",
                NumberOfAtoms = 10000,
                PreferredDays = 3,
                MaximumDays = 5,
                Credit = 500,
                Frames = 100,
                Core = "GRO-A5",
                Description = "http://something.com",
                Contact = "me",
                KFactor = 26.4
            },
            new()
            {
                ProjectNumber = 6901,
                ServerIP = "5.6.7.8",
                WorkUnitName = "Work Unit Name",
                NumberOfAtoms = 78910,
                PreferredDays = 4,
                MaximumDays = 5,
                Credit = 512,
                Frames = 100,
                Core = "GRO-A5",
                Description = "http://somethingelse.com",
                Contact = "you",
                KFactor = 2
            }
        };
        return collection;
    }
}
