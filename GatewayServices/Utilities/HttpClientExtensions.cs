using GatewayServices.ControlModels;
using GatewayServices.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace GatewayServices.Utilities
{
    public static class HttpClientExtensions
    {
        public static async Task<ControlModels.ApiResponse<VResult>> GetAsync<VResult>(this HttpClient client, string uri, string accessToken)
        {
            return await SendAsync<VResult>(
                client: client,
                httpMethod: HttpMethod.Get,
                uri: uri,
                accessToken: accessToken
                );
        }

        public static async Task<ControlModels.ApiResponse<VResult>> GetAsync<V, VResult>(this HttpClient client, string uri, V input, string accessToken)
        {
            return await SendAsync<V, VResult>(
                client: client,
                httpMethod: HttpMethod.Get,
                uri: uri,
                input: input,
                accessToken: accessToken
                );
        }

        public static async Task<ControlModels.ApiResponse<VResult>> PostAsync<V, VResult>(this HttpClient client, string uri, V input, string accessToken)
        {
            return await SendAsync<V, VResult>(
                client: client,
                httpMethod: HttpMethod.Post,
                uri: uri,
                input: input,
                accessToken: accessToken
                );
        }

        public static async Task<ControlModels.ApiResponse<VResult>> PutAsync<V, VResult>(this HttpClient client, string uri, V input, string accessToken)
        {
            return await SendAsync<V, VResult>(
                client: client,
                httpMethod: HttpMethod.Put,
                uri: uri,
                input: input,
                accessToken: accessToken
                );
        }

        public static async Task<ControlModels.ApiResponse<VResult>> PutAsync<V, VResult>(this HttpClient client, string uri, V input)
        {
            return await SendAsync<V, VResult>(
                client: client,
                httpMethod: HttpMethod.Put,
                uri: uri,
                input: input
                );
        }

        public static async Task<ControlModels.ApiResponse<VResult>> DeleteAsync<VResult>(this HttpClient client, string uri, string accessToken)
        {
            return await SendAsync<VResult>(
                client: client,
                httpMethod: HttpMethod.Delete,
                uri: uri,
                accessToken: accessToken
                );
        }

        private static async Task<ControlModels.ApiResponse<VResult>> SendAsync<VResult>(HttpClient client, HttpMethod httpMethod, string uri, string accessToken)
        {
            using var httpRequestMessage = new HttpRequestMessage(httpMethod, uri);
            using var httpResponseMessage = await SendAsync(
                client: client,
                httpRequestMessage: httpRequestMessage,
                accessToken: accessToken
                );

            return await HandleResponse<VResult>(httpResponseMessage);
        }

        private static async Task<ControlModels.ApiResponse<VResult>> SendAsync<V, VResult>(HttpClient client, HttpMethod httpMethod, string uri, V input, string accessToken)
        {
            using var httpRequestMessage = new HttpRequestMessage(httpMethod, uri);

            var httpContent = new StringContent(JsonConvert.SerializeObject(input));
            httpRequestMessage.Content = httpContent;
            httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using var httpResponseMessage = await SendAsync(
                client: client,
                httpRequestMessage: httpRequestMessage,
                accessToken: accessToken
                );

            return await HandleResponse<VResult>(httpResponseMessage);
        }

        private static async Task<ControlModels.ApiResponse<VResult>> SendAsync<V, VResult>(HttpClient client, HttpMethod httpMethod, string uri, V input)
        {
            using var httpRequestMessage = new HttpRequestMessage(httpMethod, uri);

            var httpContent = new StringContent(JsonConvert.SerializeObject(input));
            httpRequestMessage.Content = httpContent;
            httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using var httpResponseMessage = await SendAsync(
                client: client,
                httpRequestMessage: httpRequestMessage
                );

            return await HandleResponse<VResult>(httpResponseMessage);
        }

        private static async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage httpRequestMessage, string accessToken)
        {
            PrepareHttpRequestHeadersAsync(
                httpRequestMessage: httpRequestMessage,
                accessToken: accessToken
                );

            return await client.SendAsync(
                request: httpRequestMessage,
                completionOption: HttpCompletionOption.ResponseHeadersRead
                );
        }

        private static async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage httpRequestMessage)
        {
            return await client.SendAsync(
                request: httpRequestMessage,
                completionOption: HttpCompletionOption.ResponseHeadersRead
                );
        }

        private static void PrepareHttpRequestHeadersAsync(HttpRequestMessage httpRequestMessage, string accessToken)
        {
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        private static async Task<ControlModels.ApiResponse<VResult>> HandleResponse<VResult>(HttpResponseMessage response)
        {
            var apiResponse = new ControlModels.ApiResponse<VResult>();

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                apiResponse = JsonConvert.DeserializeObject<ControlModels.ApiResponse<VResult>>(content);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ControlModels.ApiResponse<VResult>>(error);

                if (errorResponse != null)
                {
                    apiResponse = errorResponse;
                }
                else
                {
                    apiResponse.AddError(new ApiError() { ErrorCode = response.StatusCode.ToString(), Message = $"{response.ReasonPhrase} {error}" });
                }
            }

            apiResponse.StatusCode = (int)response.StatusCode;

            return apiResponse;
        }
    }
}
