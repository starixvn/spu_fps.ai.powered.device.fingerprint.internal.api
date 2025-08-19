namespace Device.Fingerprint.Features.Account.Model
{
    public class SearchAccountItemModel
    {
        public ulong CTSCustID { get; set; }

        public string UserName { get; set; }

        public string RegisterName { get; set; } = string.Empty;

        public int SubscriberID { get; set; }

    }
}
