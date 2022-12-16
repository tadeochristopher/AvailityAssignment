using GatewayServices.Entities;
using GatewayServices.Handlers;
using GatewayServices.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.ComponentModel;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace Registration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        public async Task<ActionResult> RegisterClient([Bind(include: "LoginName, NPINumber, FullName, Password, BusinessAddress, Telephone" )] Register clientInfo)
        {
            var getRegisterationControl = new RegistrationClientService();

            var getResult = await getRegisterationControl.SendRegistration(new BindingList<Register>() { clientInfo });

            return Ok(getResult);
        }
    }
}
