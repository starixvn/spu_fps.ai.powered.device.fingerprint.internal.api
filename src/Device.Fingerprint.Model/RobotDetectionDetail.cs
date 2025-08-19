using System;
using System.Collections.Generic;
using System.Text;

namespace Device.Fingerprint.Model
{
    public class RobotDetectionDetail
    {
        public float Score { get; set; }

        public int TestsRun { get; set; }

        public string TestsDetail { get; set; }
    }
}
