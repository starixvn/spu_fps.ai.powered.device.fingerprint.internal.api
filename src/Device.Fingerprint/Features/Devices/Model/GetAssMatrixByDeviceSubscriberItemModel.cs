namespace Device.Fingerprint.Features.Devices.Model
{
    public class GetAssMatrixByDeviceSubscriberItemModel
    {
        public ulong AccountID { get; set; }

        public string RegisterName { get; set; }

        public string UserName { get; set; }

        public ulong DeviceID { get; set; }

        public string FirstDeviceCode { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
