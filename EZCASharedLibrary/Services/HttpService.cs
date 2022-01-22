using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace EZCASharedLibrary.Services;

public interface IHttpService
{
    Task<HttpResponseMessage> PostAPIAsync(string url, string jsonPayload, string token);
    Task<HttpResponseMessage> GetAPIAsync(string url, string token);
}

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        HttpStatusCode[] httpStatusCodesWorthRetrying = {
               HttpStatusCode.RequestTimeout, // 408
               HttpStatusCode.InternalServerError, // 500
               HttpStatusCode.BadGateway, // 502
               HttpStatusCode.ServiceUnavailable, // 503
               HttpStatusCode.GatewayTimeout // 504
            };
        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrInner<TaskCanceledException>()
            .OrResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode))
              .WaitAndRetryAsync(new[]
              {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4),
                    TimeSpan.FromSeconds(8)
              });
    }


    public async Task<HttpResponseMessage> PostAPIAsync(string url, string jsonPayload, string token)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentNullException("URL is empty or null ", nameof(url));
        }
        HttpResponseMessage responseMessage;
        HttpRequestMessage requestMessage = new(HttpMethod.Post, url);
        if (!string.IsNullOrWhiteSpace(token))
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        requestMessage.Content = new StringContent(jsonPayload,
                Encoding.UTF8, "application/json");
        responseMessage = await _retryPolicy.ExecuteAsync(async () =>
                  await SendMessageAsync(requestMessage));
        return responseMessage;
    }

    public async Task<HttpResponseMessage> GetAPIAsync(string url, string token)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentNullException("URL is empty or null", nameof(url));
        }
        HttpRequestMessage requestMessage = new(HttpMethod.Get, url);
        if (!string.IsNullOrWhiteSpace(token))
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        HttpResponseMessage response;
        response = await _retryPolicy.ExecuteAsync(async () =>
                 await SendMessageAsync(requestMessage));
        return response;
    }


    private async Task<HttpResponseMessage> SendMessageAsync(HttpRequestMessage requestMessage)
    {
        HttpResponseMessage response;
        response = await _httpClient.SendAsync(requestMessage);
        return response;
    }
}
