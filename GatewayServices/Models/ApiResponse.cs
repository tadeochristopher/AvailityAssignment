using System.Net;

namespace GatewayServices.Models
{
    public class ApiResponse<V>
    {
        public V? Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccessful { get; set; }
        public List<ApiError> Errors { get; set; } = new List<ApiError>();
    }
}
