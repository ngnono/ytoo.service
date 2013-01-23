namespace Yintai.Hangzhou.Contract.DTO.Request.Device
{
    public class DeviceRegisterRequest : CoordinateRequest
    {
        public string DeviceToken { get; set; }
        public string Uid { get; set; }
    }
}
