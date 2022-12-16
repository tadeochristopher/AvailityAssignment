using GatewayServices.Models;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace GatewayServices.ControlModels
{
    public class ApiResponse<V>
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int StatusCode { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsSuccessful => Errors == null || Errors.Count == 0;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public V? Result { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Error>? Errors { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string>? Messages { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Warning>? Warnings { get; set; }

        public void AddError(ApiError error)
        {
            if (Errors == null)
            {
                Errors = new List<Error>();
            }

            Errors.Add(new Error(error.ErrorCode, error.Message));
        }

        public static implicit operator ApiResponse<V>(ApiResponse<bool> v)
        {
            throw new NotImplementedException();
        }
    }
}
