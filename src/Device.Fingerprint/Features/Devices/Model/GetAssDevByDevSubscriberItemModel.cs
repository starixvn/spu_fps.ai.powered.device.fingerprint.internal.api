namespace Device.Fingerprint.Features.Devices.Model
{
    public class GetAssDevByDevSubscriberItemModel
    {
        public ulong DeviceID { get; set; }

        public string DeviceCode { get; set; }

        public DateTime? FirstSeenDate { get; set; }

        public int AssociatedAccounts { get; set; }

        public int AssociatedDevices { get; set; }
    }
}
