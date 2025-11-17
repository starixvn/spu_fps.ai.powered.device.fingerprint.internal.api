namespace Device.Fingerprint.Features.Account.Model
{
    public class GetAssAccountsByCustSubscriberItemModel
    {
        public ulong CTSCustID { get; set; }

        public string UserName { get; set; }

        public string RegisterName { get; set; } = string.Empty;

        public int DeviceInCommon { get; set; }

        public int OtherDeives { get; set; }

        public int AssociatedAccount { get; set; }
    }
}
