using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.Fingerprint.Features.ClientTransaction.Criterias
{
    public class InsertClientTransactionExceptionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ExceptionJson", AllowNull = true)]
        public string ExceptionJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Log_FPSJavascriptException_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
