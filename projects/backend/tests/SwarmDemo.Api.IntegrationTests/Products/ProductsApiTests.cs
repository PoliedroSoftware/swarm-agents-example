using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SwarmDemo.Api.Contracts.Products;
using SwarmDemo.Api.IntegrationTests.Fixtures;

namespace SwarmDemo.Api.IntegrationTests.Products;

public class ProductsApiTests(SwarmDemoApiFactory factory) : IClassFixture<SwarmDemoApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static string UniqueSku(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}"[..13].ToUpperInvariant();

    [Fact]
    public async Task Full_crud_flow()
    {
        var sku = UniqueSku("DEMO");
        var create = new CreateProductRequest(sku, "Demo", null, 9.99m, "USD", 5);
        var post = await _client.PostAsJsonAsync("/api/products", create);
        post.StatusCode.Should().Be(HttpStatusCode.Created);
        var posted = await post.Content.ReadFromJsonAsync<ProductResponse>();
        posted.Should().NotBeNull();
        posted!.Sku.Should().Be(sku);
        posted.PriceAmount.Should().Be(9.99m);

        var get = await _client.GetAsync($"/api/products/{posted.Id}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);

        var getAgain = await _client.GetAsync($"/api/products/{posted.Id}");
        getAgain.StatusCode.Should().Be(HttpStatusCode.OK);

        var update = new UpdateProductRequest("Demo Updated", "new desc", 19.99m, "EUR", 10);
        var put = await _client.PutAsJsonAsync($"/api/products/{posted.Id}", update);
        put.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getAfterUpdate = await _client.GetAsync($"/api/products/{posted.Id}");
        var fetched = await getAfterUpdate.Content.ReadFromJsonAsync<ProductResponse>();
        fetched!.Name.Should().Be("Demo Updated");
        fetched.PriceCurrency.Should().Be("EUR");
        fetched.PriceAmount.Should().Be(19.99m);

        var del = await _client.DeleteAsync($"/api/products/{posted.Id}");
        del.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var gone = await _client.GetAsync($"/api/products/{posted.Id}");
        gone.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_returns_409_on_duplicate_sku()
    {
        var create = new CreateProductRequest(UniqueSku("DUPE"), "Dupe", null, 1m, "USD", 1);

        var first = await _client.PostAsJsonAsync("/api/products", create);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await _client.PostAsJsonAsync("/api/products", create);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_returns_400_on_invalid_payload()
    {
        var invalid = new CreateProductRequest("", "", null, -1m, "X", -1);
        var resp = await _client.PostAsJsonAsync("/api/products", invalid);
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
