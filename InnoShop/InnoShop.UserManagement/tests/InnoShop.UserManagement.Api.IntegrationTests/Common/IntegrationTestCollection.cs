namespace InnoShop.UserManagement.Api.IntegrationTests.Common;

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "IntegrationTestCollection";
}
