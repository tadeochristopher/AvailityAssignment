using GatewayServices.Models;

namespace GatewayServices.ControlModels
{
    public class ApiException : Exception
    {
        public List<ApiError> Errors { get; }
        public int StatusCode { get; set; }

        public ApiException()
            : base("One or more failures have occurred.")
        {
            Errors = new List<ApiError>();
        }

        public ApiException(List<ApiError> errors, int statusCode)
            : this()
        {
            Errors = errors;
            StatusCode = statusCode;
        }
    }
}
