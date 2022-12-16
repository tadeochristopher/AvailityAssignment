namespace GatewayServices.ControlModels
{
    public class Warning
    {
        public string Message { get; set; }
        public string Code { get; set; }

        public Warning(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
