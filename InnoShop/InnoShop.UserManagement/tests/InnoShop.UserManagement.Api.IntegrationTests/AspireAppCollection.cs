using Xunit;

namespace InnoShop.UserManagement.Api.IntegrationTests;

/// <summary>
/// xUnit collection definition to share the AspireAppFixture across all test classes.
/// This ensures the AppHost is started only once for all tests in the collection.
/// </summary>
[CollectionDefinition(nameof(AspireAppCollection))]
public class AspireAppCollection : ICollectionFixture<AspireAppFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
