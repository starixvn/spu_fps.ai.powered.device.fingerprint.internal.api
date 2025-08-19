using System;

namespace Device.Fingerprint.Features.Transaction.Model
{
    public class AssociationAccountUsageEntity
    {
        public string LoginName { get; set; }

        public DateTime MnUsageDate { get; set; }

        public DateTime MaxUsageDate { get; set; }

        public int UsageCount { get; set; }
    }
}
