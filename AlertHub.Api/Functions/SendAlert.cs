using AlertHub.Api.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

public class SendAlertFunction
{
    [Function("SendAlert")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        var alert = await req.ReadFromJsonAsync<AlertMessage>();
        var response = req.CreateResponse(HttpStatusCode.OK);

        await response.WriteAsJsonAsync(alert);

        return response;
    }
}