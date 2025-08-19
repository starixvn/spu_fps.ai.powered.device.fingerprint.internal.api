using System;

namespace Device.Fingerprint.Features.Transaction.Model
{
    public class AssociationDeviceEntity
    {
        public long DeviceID { get; set; }    
        
        public string Device { get; set; }

        public DateTime FirstSeenDate { get; set; }
    }
}
