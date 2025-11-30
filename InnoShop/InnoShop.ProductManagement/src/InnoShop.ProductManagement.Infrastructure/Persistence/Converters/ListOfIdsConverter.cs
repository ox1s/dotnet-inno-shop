using System.Collections;
using System.Data;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InnoShop.ProductManagement.Infrastructure.Persistence.Converters;

public class ListOfIdsConverter : ValueConverter<List<Guid>, string>
{
    public ListOfIdsConverter(ConverterMappingHints? mappingHints = null)
        : base(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList(),
            mappingHints)
    {
    }
}

public class ListOfIdsComparer : ValueComparer<List<Guid>>
{
    public ListOfIdsComparer() : base(
      (t1, t2) => t1!.SequenceEqual(t2!),
      t => t.Select(x => x!.GetHashCode()).Aggregate((x, y) => x ^ y),
      t => t)
    {
    }
}

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<List<Guid>> HasListOfIdsConverter(this PropertyBuilder<List<Guid>> propertyBuilder)
    {
        propertyBuilder
            .HasConversion(new ListOfIdsConverter())
            .Metadata.SetValueComparer(new ListOfIdsComparer());
        return propertyBuilder;
    }
}