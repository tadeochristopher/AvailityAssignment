namespace GatewayServices.Entities
{
    public class Register
    {
        public string LoginName { get; set; } = string.Empty;
        public int NPINumber { get; set; }
        public string FullName { get; set; } = string.Empty;
        public BinaryData? Password { get; set; }
        public string BusinessAddress { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
    }
}
