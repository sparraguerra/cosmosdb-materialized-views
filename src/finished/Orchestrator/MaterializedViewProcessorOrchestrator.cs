namespace MaterializedViewProcessor.Orchestrator;

[DurableTask(nameof(MaterializedViewProcessorOrchestrator))]
public class MaterializedViewProcessorOrchestrator : TaskOrchestrator<CreateOrderRequest, bool>
{

    protected readonly TaskOptions defaultActivityRetryOptions = new()
    {
        Retry = new TaskRetryOptions(new Microsoft.DurableTask.RetryPolicy(maxNumberOfAttempts: 3, firstRetryInterval: TimeSpan.FromSeconds(5)))
    };

    public override async Task<bool> RunAsync(TaskOrchestrationContext context, CreateOrderRequest input)
    {
        try
        {
            var tasks = new List<Task>
            {
                context.CallActivityAsync<bool>(nameof(UpdateOrdersByCustomerMaterializedViewActivity), input, defaultActivityRetryOptions),
                context.CallActivityAsync<bool>(nameof(UpdateOrdersByProductMaterializedViewActivity), input, defaultActivityRetryOptions),
                context.CallActivityAsync<bool>(nameof(UpdateOrdersByQuarterMaterializedViewActivity), input, defaultActivityRetryOptions)
            };

            await Task.WhenAll(tasks);
        }
        catch (Exception)
        {
            return false;
        }
        context.SetCustomStatus(new { result = true });
        return true;

    }
}

