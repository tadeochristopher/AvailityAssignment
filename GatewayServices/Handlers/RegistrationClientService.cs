using GatewayServices.ControlModels;
using GatewayServices.Entities;
using GatewayServices.Models;
using GatewayServices.Utilities;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace GatewayServices.Handlers
{
    public class RegistrationClientService
    {
        private readonly UsermanagementConfig _usermanagementServiceConfig;

        public RegistrationClientService(IOptions<UsermanagementConfig> usermanagementServiceConfig)
        {
            _usermanagementServiceConfig = usermanagementServiceConfig.Value;
        }

        public RegistrationClientService()
        {

        }

        //Channeling out registration data...TCDW
        public async Task<bool> SendRegistration(BindingList<Register> reg)
        {
            var response = new ControlModels.ApiResponse<bool>();

            using (var _client = new HttpClient())
            {
                response = await _client.PutAsync<BindingList<Register>, bool>(
                uri: _usermanagementServiceConfig.RegisterClient,
                input: reg
                );

                if (response.IsSuccessful)
                {
                    return response.Result;
                }
                else
                {
                    var apiErrors = new List<ApiError>();

                    response.Errors.ForEach(e =>
                    {
                        apiErrors.Add(new ApiError
                        {
                            ErrorCode = e.ErrorCode,
                            Message = e.Message
                        });
                    });

                    throw new ApiException(apiErrors, response.StatusCode);
                }
            }
        }
    }
}
