using System.Diagnostics;
using System.Globalization;

using HFM.Proteins.Internal;

namespace HFM.Proteins;

/// <summary>
/// Represents a serializer capable of serializing and deserializing a collection of <see cref="Protein"/> objects to and from tab delimited text.
/// </summary>
public class TabDelimitedTextSerializer : IProteinCollectionSerializer
{
    internal ICollection<Protein> DeserializeOld(Stream stream)
    {
        var collection = new List<Protein>();
        using var reader = new StreamReader(stream);

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            var p = ParseProteinOld(line);
            if (Protein.IsValid(p))
            {
                collection.Add(p);
            }
        }

        return collection;
    }

    /// <summary>
    /// Deserializes a collection of <see cref="Protein"/> objects from a <see cref="Stream"/>.
    /// </summary>
    public ICollection<Protein> Deserialize(Stream stream)
    {
        var collection = new List<Protein>();
        using var reader = new StreamReader(stream);

        int bytesRead;
        char[]? unparsedChars = null;
        Span<char> buffer = stackalloc char[1024];

        while ((bytesRead = reader.Read(buffer)) != 0)
        {
            DeserializeProteins(buffer, bytesRead, ref unparsedChars, collection);
        }

        return collection;
    }

    internal async Task<ICollection<Protein>> DeserializeAsyncOld(Stream stream)
    {
        var collection = new List<Protein>();
        using var reader = new StreamReader(stream);

        string line;
        while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
        {
            var p = ParseProteinOld(line);
            if (Protein.IsValid(p))
            {
                collection.Add(p);
            }
        }

        return collection;
    }

    /// <summary>
    /// Asynchronously deserializes a collection of <see cref="Protein"/> objects from a <see cref="Stream"/>.
    /// </summary>
    public async Task<ICollection<Protein>> DeserializeAsync(Stream stream)
    {
        var collection = new List<Protein>();
        using var reader = new StreamReader(stream);

        int bytesRead;
        char[]? unparsedChars = null;
        var buffer = new Memory<char>(new char[1024]);

        while ((bytesRead = await reader.ReadAsync(buffer).ConfigureAwait(false)) != 0)
        {
            DeserializeProteins(buffer.Span, bytesRead, ref unparsedChars, collection);
        }

        return collection;
    }

    private static void DeserializeProteins(Span<char> buffer, int bytesRead, ref char[]? unparsedChars, ICollection<Protein> collection)
    {
        foreach (var line in buffer[..bytesRead].SplitLines())
        {
            if (line.HasSeparator)
            {
                ReadOnlySpan<char> lineToParse = line.Line;
                if (unparsedChars != null && unparsedChars.Length > 0)
                {
                    lineToParse = unparsedChars.Concat(lineToParse);
                    unparsedChars = null;
                }

                var p = ParseProtein(lineToParse);
                if (Protein.IsValid(p))
                {
                    collection.Add(p);
                }
            }
            else
            {
                unparsedChars = new char[line.Line.Length];
                for (int i = 0; i < line.Line.Length; i++)
                {
                    unparsedChars[i] = line.Line[i];
                }
            }
        }
    }

    private static Protein ParseProteinOld(string line)
    {
        try
        {
            var p = new Protein();
            string[] lineData = line.Split(new[] { '\t' }, StringSplitOptions.None);
            p.ProjectNumber = Int32.Parse(lineData[0], CultureInfo.InvariantCulture);
            p.ServerIP = lineData[1].Trim();
            p.WorkUnitName = lineData[2].Trim();
            p.NumberOfAtoms = Int32.Parse(lineData[3], CultureInfo.InvariantCulture);
            p.PreferredDays = Double.Parse(lineData[4], CultureInfo.InvariantCulture);
            p.MaximumDays = Double.Parse(lineData[5], CultureInfo.InvariantCulture);
            p.Credit = Double.Parse(lineData[6], CultureInfo.InvariantCulture);
            p.Frames = Int32.Parse(lineData[7], CultureInfo.InvariantCulture);
            p.Core = lineData[8];
            p.Description = lineData[9];
            p.Contact = lineData[10];
            p.KFactor = Double.Parse(lineData[11], CultureInfo.InvariantCulture);
            return p;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
        {
            Debug.Assert(false);
        }
        return null;
    }

    private static Protein ParseProtein(ReadOnlySpan<char> line)
    {
        try
        {
            int index = 0;
            var p = new Protein();
            foreach (var lineData in line.SplitTabs())
            {
                switch (index++)
                {
                    case 0:
                        p.ProjectNumber = Int32.Parse(lineData, NumberStyles.Integer, CultureInfo.InvariantCulture);
                        break;
                    case 1:
                        p.ServerIP = lineData.Trim().ToString();
                        break;
                    case 2:
                        p.WorkUnitName = lineData.Trim().ToString();
                        break;
                    case 3:
                        p.NumberOfAtoms = Int32.Parse(lineData, NumberStyles.Integer, CultureInfo.InvariantCulture);
                        break;
                    case 4:
                        p.PreferredDays = Double.Parse(lineData, NumberStyles.Float, CultureInfo.InvariantCulture);
                        break;
                    case 5:
                        p.MaximumDays = Double.Parse(lineData, NumberStyles.Float, CultureInfo.InvariantCulture);
                        break;
                    case 6:
                        p.Credit = Double.Parse(lineData, NumberStyles.Float, CultureInfo.InvariantCulture);
                        break;
                    case 7:
                        p.Frames = Int32.Parse(lineData, NumberStyles.Integer, CultureInfo.InvariantCulture);
                        break;
                    case 8:
                        p.Core = lineData.ToString();
                        break;
                    case 9:
                        p.Description = lineData.ToString();
                        break;
                    case 10:
                        p.Contact = lineData.ToString();
                        break;
                    case 11:
                        p.KFactor = Double.Parse(lineData, NumberStyles.Float, CultureInfo.InvariantCulture);
                        break;
                }
            }
            return p;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
        {
            Debug.Assert(false);
        }
        return null;
    }

    /// <summary>
    /// Serializes a collection of <see cref="Protein"/> objects to a <see cref="Stream"/>.
    /// </summary>
    public void Serialize(Stream stream, ICollection<Protein> collection)
    {
        using (var writer = new StreamWriter(stream))
        {
            foreach (Protein protein in collection.OrderBy(x => x.ProjectNumber))
            {
                string line = WriteProtein(protein);
                writer.WriteLine(line);
            }
        }
    }

    /// <summary>
    /// Asynchronously serializes a collection of <see cref="Protein"/> objects to a <see cref="Stream"/>.
    /// </summary>
    public async Task SerializeAsync(Stream stream, ICollection<Protein> collection)
    {
        using (var writer = new StreamWriter(stream))
        {
            foreach (Protein protein in collection.OrderBy(x => x.ProjectNumber))
            {
                string line = WriteProtein(protein);
                await writer.WriteLineAsync(line).ConfigureAwait(false);
            }
        }
    }

    private static string WriteProtein(Protein protein) =>
        String.Format(CultureInfo.InvariantCulture,
            "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}",
            /*  0 */ protein.ProjectNumber,
            /*  1 */ protein.ServerIP,
            /*  2 */ protein.WorkUnitName,
            /*  3 */ protein.NumberOfAtoms,
            /*  4 */ protein.PreferredDays,
            /*  5 */ protein.MaximumDays,
            /*  6 */ protein.Credit,
            /*  7 */ protein.Frames,
            /*  8 */ protein.Core,
            /*  9 */ protein.Description,
            /* 10 */ protein.Contact,
            /* 11 */ protein.KFactor);
}
