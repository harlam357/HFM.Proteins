using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace HFM.Proteins;

/// <summary>
/// Deserializes a collection of <see cref="Protein"/> objects from the Folding@Home project summary json.
/// This class does not support serialization through the <see cref="IProteinCollectionSerializer"/> interface. The serialize methods will throw <see cref="NotSupportedException"/>.
/// </summary>
public class ProjectSummaryJsonDeserializer : IProteinCollectionSerializer
{
    /// <summary>
    /// Deserializes a collection of <see cref="Protein"/> objects from a <see cref="Stream"/>.
    /// </summary>
    public ICollection<Protein> Deserialize(Stream stream)
    {
        string json;
        using (var reader = new StreamReader(stream))
        {
            json = reader.ReadToEnd();
        }
        return DeserializeInternal(json);
    }

    /// <summary>
    /// Asynchronously deserializes a collection of <see cref="Protein"/> objects from a <see cref="Stream"/>.
    /// </summary>
    public async Task<ICollection<Protein>> DeserializeAsync(Stream stream)
    {
        string json;
        using (var reader = new StreamReader(stream))
        {
            json = await reader.ReadToEndAsync().ConfigureAwait(false);
        }
        return DeserializeInternal(json);
    }

    private static ICollection<Protein> DeserializeInternal(string json)
    {
        const double secondsToDays = 86400.0;

        var collection = new List<Protein>();
        if (json.Length > 0)
        {
            foreach (var node in JsonNode.Parse(json)!.AsArray())
            {
                if (node is null)
                {
                    continue;
                }

                var p = new Protein();
                p.ProjectNumber = GetTokenValue<int>(node, "id");
                p.ServerIP = GetTokenValue<string>(node, "ws");
                p.WorkUnitName = @"p" + p.ProjectNumber;
                p.NumberOfAtoms = GetTokenValue<int>(node, "atoms");
                p.PreferredDays = Math.Round(GetTokenValue<int>(node, "timeout") / secondsToDays, 3, MidpointRounding.AwayFromZero);
                p.MaximumDays = Math.Round(GetTokenValue<int>(node, "deadline") / secondsToDays, 3, MidpointRounding.AwayFromZero);
                p.Credit = GetTokenValue<double>(node, "credit");
                p.Frames = 100;
                p.Core = GetTokenValue<string>(node, "type");
                p.Description = @"https://apps.foldingathome.org/project.py?p=" + p.ProjectNumber;
                p.Contact = GetTokenValue<string>(node, "contact");
                p.KFactor = GetTokenValue<double?>(node, "bonus") ?? 0.75;
                collection.Add(p);
            }
        }
        return collection;
    }

    private static T? GetTokenValue<T>(JsonNode node, string path)
    {
        var selected = node[path];
        return selected is not null
            ? selected.GetValue<T>()
            : default;
    }

    [ExcludeFromCodeCoverage]
    void IProteinCollectionSerializer.Serialize(Stream stream, ICollection<Protein> collection) =>
        throw new NotSupportedException("Serialization is not supported.");

    [ExcludeFromCodeCoverage]
    Task IProteinCollectionSerializer.SerializeAsync(Stream stream, ICollection<Protein> collection) =>
        throw new NotSupportedException("Serialization is not supported.");
}
