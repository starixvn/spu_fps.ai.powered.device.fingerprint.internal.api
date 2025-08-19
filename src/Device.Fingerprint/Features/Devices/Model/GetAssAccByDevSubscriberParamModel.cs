namespace Device.Fingerprint.Features.Devices.Model
{
    public class GetAssAccByDevSubscriberParamModel
    {
        public int SubscriberID { get; set; }

        public ulong DeviceID { get; set; }

        public int NoOfRecord { get; set; }
    }
}
