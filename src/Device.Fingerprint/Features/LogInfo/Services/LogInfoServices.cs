using Device.Fingerprint.Features.LogInfo.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Device.Fingerprint.Features.LogInfo.Services
{
    public interface ILogInfoServices
    {
        Task InsertDCSConsumerLogAsync(LogInfoModel logInfo);
    }

    public class LogInfoServices : ILogInfoServices
    {
        private readonly ILogInfoDataAccess logInfoDataAccess;

        public LogInfoServices(ILogInfoDataAccess logInfoDataAccess)
        {
            this.logInfoDataAccess = logInfoDataAccess ?? throw new ArgumentNullException(nameof(logInfoDataAccess));
        }

        public async Task InsertDCSConsumerLogAsync(LogInfoModel logInfo)
        {
            var logInfoJson = JsonConvert.SerializeObject(logInfo);
            logInfoJson = $"[{logInfoJson}]";
            await logInfoDataAccess.InsertDCSConsumerLogAsync(logInfoJson);
        }
    }
}
