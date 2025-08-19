using System;
using System.Collections.Generic;
using System.Text;

namespace Device.Fingerprint.Features.Transaction.Model
{
    public class AssociationByAccountAndSubscriberEntity
    {
        public IEnumerable<AssociationDeviceEntity> AssociationDevice { get; set; }

        public IEnumerable<AssociationAccountEntity> AssociationAccount { get; set; }
    }
}
