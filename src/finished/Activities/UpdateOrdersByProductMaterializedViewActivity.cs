namespace MaterializedViewProcessor.Activities;

[DurableTask(nameof(UpdateOrdersByProductMaterializedViewActivity))]
public class UpdateOrdersByProductMaterializedViewActivity
                    (CosmosClient client, ILogger<UpdateOrdersByProductMaterializedViewActivity> logger)
                           : TaskActivity<CreateOrderRequest, bool>
{
    private readonly CosmosClient client = client;
    private readonly ILogger<UpdateOrdersByProductMaterializedViewActivity> logger = logger;

    public override async Task<bool> RunAsync(TaskActivityContext context, CreateOrderRequest input)
    {
        if (input is null)
        {
            return false;
        }

        Database database = await client.CreateDatabaseIfNotExistsAsync("db");
        Container container = await database.CreateContainerIfNotExistsAsync("OrdersByProduct", "/productId", 400);

        foreach (var item in input.OrderItems)
        {  

            // find in container if the product exists
            var query = new QueryDefinition($"SELECT * FROM c WHERE c.productId = @productId")
                            .WithParameter("@productId", item.ProductId);
            var requestOptions = new QueryRequestOptions()
            {
                MaxItemCount = 1
            };
            var iterator = container.GetItemQueryIterator<OrdersByProduct>(query, null, requestOptions);
            var result = await iterator.ReadNextAsync();

            if (result?.FirstOrDefault() is null)
            {
                OrdersByProduct view = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = item.ProductId,
                    Product = item.Product,
                    Quantity = item.Quantity,
                    Total = item.Total
                };

                // add to container 
                await container.CreateItemAsync(view, new PartitionKey(view.ProductId));
            }
            else
            {
                OrdersByProduct view = result.First();
                view.Quantity += item.Quantity;
                view.Total += input.OrderItems.Sum(x => x.Total);

                // update in container  
                await container.ReplaceItemAsync(view, view.Id, new PartitionKey(view.ProductId));
            }
        }

        

        return true;
    }
}
