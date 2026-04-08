//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Http;
//using StackExchange.Redis;
//using System.Net;
//using System.Text.Json;

//namespace AlertHub.Api.Functions;

//public class RedisHealthCheckFunction
//{
//    private readonly IConnectionMultiplexer _redis;

//    public RedisHealthCheckFunction(IConnectionMultiplexer multiplexer)
//    {
//        _redis = multiplexer;
//    }

//    [Function("HealthCheck")]
//    public async Task<HttpResponseData> Run(
//        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
//    {
//        var response = req.CreateResponse();

//        try
//        {
//            var db = _redis.GetDatabase();
//            var pong = await db.PingAsync();

//            response.StatusCode = HttpStatusCode.OK;
//            response.Headers.Add("Content-Type", "application/json");
//            response.WriteString(JsonSerializer.Serialize(new { status = "Healthy", ping = pong.TotalMilliseconds }));
//        }
//        catch (Exception ex)
//        {
//            response.StatusCode = HttpStatusCode.ServiceUnavailable;
//            response.Headers.Add("Content-Type", "application/json");
//            response.WriteString(JsonSerializer.Serialize(new { status = "Unhealthy", error = ex.Message }));
//        }

//        return response;
//    }
//}