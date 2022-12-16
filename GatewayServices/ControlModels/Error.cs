namespace GatewayServices.ControlModels
{
    public class Error
    {
        public string Message { get; set; }
        public string ErrorCode { get; set; }

        public Error(string errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }
    }
}
