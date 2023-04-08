using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HFM.Proteins;

/// <summary>
/// Represents a collection of <see cref="Protein"/> values keyed by project number.
/// </summary>
public class ProteinCollection : KeyedCollection<int, Protein>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProteinCollection"/> class.
    /// </summary>
    public ProteinCollection()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProteinCollection"/> class from valid proteins copied from the <paramref name="proteins"/> collection.
    /// </summary>
    public ProteinCollection(IEnumerable<Protein> proteins)
    {
        ArgumentNullException.ThrowIfNull(proteins);

        foreach (Protein protein in proteins.Where(Protein.IsValid))
        {
            Add(protein);
        }
    }

    protected override int GetKeyForItem(Protein item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        return item.ProjectNumber;
    }

    /// <summary>
    /// Gets the <see cref="Protein"/> associated with the specified <paramref name="projectNumber"/>.
    /// </summary>
    public bool TryGetValue(int projectNumber, out Protein? protein)
    {
        if (Dictionary is null)
        {
            protein = null;
            return false;
        }
        return Dictionary.TryGetValue(projectNumber, out protein);
    }

    /// <summary>
    /// Updates the <see cref="ProteinCollection"/> from the collection of <see cref="Protein"/> objects.
    /// </summary>
    /// <param name="proteins">The collection of <see cref="Protein"/> objects used to apply changes to this collection.</param>
    public IReadOnlyList<ProteinChange> Update(IEnumerable<Protein> proteins)
    {
        ArgumentNullException.ThrowIfNull(proteins);

        var changes = new List<ProteinChange>();
        foreach (Protein protein in proteins.Where(Protein.IsValid))
        {
            if (Contains(protein.ProjectNumber))
            {
                Protein previous = this[protein.ProjectNumber];
                _ = Remove(previous);

                var propertyChanges = GetChangedProperties(previous, protein);
                if (propertyChanges.Count > 0)
                {
                    changes.Add(ProteinChange.Property(protein.ProjectNumber, propertyChanges));
                }
                else
                {
                    changes.Add(ProteinChange.None(protein.ProjectNumber));
                }
            }
            else
            {
                changes.Add(ProteinChange.Add(protein.ProjectNumber));
            }

            Add(protein);
        }
        return changes;
    }

    private static List<ProteinPropertyChange> GetChangedProperties(Protein previous, Protein current)
    {
        var changes = new List<ProteinPropertyChange>();
        var properties = TypeDescriptor.GetProperties(previous);
        foreach (PropertyDescriptor prop in properties)
        {
            object? value1 = prop.GetValue(previous);
            object? value2 = prop.GetValue(current);
            if (value1 is null || value2 is null)
            {
                continue;
            }
            if (!value1.Equals(value2))
            {
                changes.Add(new(prop.Name, value1.ToString()!, value2.ToString()!));
            }
        }
        return changes;
    }
}
