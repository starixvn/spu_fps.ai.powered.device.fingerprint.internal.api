namespace Device.Fingerprint.Features.Account.Model
{
    public class GetAccountsPerDeviceItemModel
    {
        public ulong DeviceId { get; set; }

        public string DeviceNo { get; set; }

        public int NoOfAccount { get; set; }
    }
}
