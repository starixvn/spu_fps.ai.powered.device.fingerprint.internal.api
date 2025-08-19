using System;

namespace Device.Fingerprint.Features.FingerPrint.Entities
{
    public class FPFingerPrintEntity
    {
        public ulong ID { get; set; }

        public string Code { get; set; }

        public string Attribute { get; set; }

        public DateTime InsertedTime { get; set; }
    }
}
