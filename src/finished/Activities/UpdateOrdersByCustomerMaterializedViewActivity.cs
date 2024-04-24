namespace MaterializedViewProcessor.Activities;

[DurableTask(nameof(UpdateOrdersByCustomerMaterializedViewActivity))]
public class UpdateOrdersByCustomerMaterializedViewActivity
                    (CosmosClient client, ILogger<UpdateOrdersByCustomerMaterializedViewActivity> logger) 
                           : TaskActivity<CreateOrderRequest, bool>
{
    private readonly CosmosClient client = client;
    private readonly ILogger<UpdateOrdersByCustomerMaterializedViewActivity> logger = logger;
    private const string containerName = "OrdersByCustomer";

    public override async Task<bool> RunAsync(TaskActivityContext context, CreateOrderRequest input)
    {
        if (input is null)
        {
            return false;
        }

        Database database = await client.CreateDatabaseIfNotExistsAsync("db");
        Container container = await database.CreateContainerIfNotExistsAsync("OrdersByCustomer", "/customerId", 400);

        // find in container if the customerId exists
        var query = new QueryDefinition($"SELECT * FROM c WHERE c.customerId = @customerId")
                        .WithParameter("@customerId", input.CustomerId);
        var requestOptions = new QueryRequestOptions()
        {
            MaxItemCount = 1
        };
        var iterator = container.GetItemQueryIterator<OrdersByCustomer>(query, null, requestOptions);
        var result = await iterator.ReadNextAsync();

        if (result?.FirstOrDefault() is null)
        {
            OrdersByCustomer view = new()
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = input.CustomerId,
                Customer = input.Customer,
                Total = input.OrderItems.Sum(x => x.Total)
            };

            // add to container 
            await container.CreateItemAsync(view, new PartitionKey(view.CustomerId));
        }
        else
        {
            OrdersByCustomer view = result.First();
            view.Total += input.OrderItems.Sum(x => x.Total);

            // update in container  
            await container.ReplaceItemAsync(view, view.Id, new PartitionKey(view.CustomerId));
        }

        return true;
    }
}
