﻿using System.Diagnostics.CodeAnalysis;

namespace HFM.Proteins;

/// <summary>
/// Provides extensions to the <see cref="IProteinCollectionSerializer"/> interface to assist with read and writing to the file system and reading from resources described by a <see cref="Uri"/>.
/// </summary>
public static class ProteinCollectionSerializerExtensions
{
    private static readonly HttpClient _HttpClient = new();

    /// <summary>
    /// Reads a collection of <see cref="Protein"/> objects from a file.
    /// </summary>
    public static ICollection<Protein> ReadFile(this IProteinCollectionSerializer serializer, string path)
    {
        using var stream = File.OpenRead(path);
        return serializer.Deserialize(stream);
    }

    /// <summary>
    /// Asynchronously reads a collection of <see cref="Protein"/> objects from a file.
    /// </summary>
    public static async Task<ICollection<Protein>> ReadFileAsync(this IProteinCollectionSerializer serializer, string path)
    {
        using var stream = File.OpenRead(path);
        return await serializer.DeserializeAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads a collection of <see cref="Protein"/> objects from a resource described by a <see cref="Uri"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static async Task<ICollection<Protein>> ReadUriAsync(this IProteinCollectionSerializer serializer, Uri requestUri)
    {
        var response = await _HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        return await serializer.DeserializeAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Writes a collection of <see cref="Protein"/> objects to a file.
    /// </summary>
    public static void WriteFile(this IProteinCollectionSerializer serializer, string path, ICollection<Protein> values)
    {
        using var stream = File.Create(path);
        serializer.Serialize(stream, values);
    }

    /// <summary>
    /// Asynchronously writes a collection of <see cref="Protein"/> objects to a file.
    /// </summary>
    public static async Task WriteFileAsync(this IProteinCollectionSerializer serializer, string path, ICollection<Protein> values)
    {
        using var stream = File.Create(path);
        await serializer.SerializeAsync(stream, values).ConfigureAwait(false);
    }
}
