using MaterializedViewProcessor.Utilities;

namespace MaterializedViewProcessor.Extensions;

public static class HttpRequestDataExtensions
{

    public static async Task<T?> GetJsonBody<T>(this HttpRequestData request)
    {
        var requestBody = await request.ReadAsStringAsync();

        if(string.IsNullOrWhiteSpace(requestBody))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(requestBody, JsonUtil.CamelCaseSerializerSettings);
    }

    public static async Task<HttpResponseData> ToOkResponseAsync(this HttpRequestData request, string message)
    {
        var response = request.CreateResponse(System.Net.HttpStatusCode.OK);
        return await ToResponseAsync(message, response);
    }

    public static async Task<HttpResponseData> ToBadRequestResponseAsync(this HttpRequestData request, string message)
    {
        var response = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        return await ToResponseAsync(message, response);
    }

    private static async Task<HttpResponseData> ToResponseAsync(string message, HttpResponseData response)
    {
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(message);
        return response;
    }
}
