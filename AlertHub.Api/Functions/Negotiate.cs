using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace AlertHub.Api.Functions;

internal sealed class NegotiateFunction
{
    [Function("negotiate")]
    public static async Task<HttpResponseData> Negotiate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [SignalRConnectionInfoInput(
            HubName = "alertsHub", 
            ConnectionStringSetting = "AzureSignalRConnectionString")]
        SignalRConnectionInfo connectionInfo)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);

        await response.WriteAsJsonAsync(new
        {
            url = connectionInfo.Url,
            accessToken = connectionInfo.AccessToken
        });

        return response;
    }
}
