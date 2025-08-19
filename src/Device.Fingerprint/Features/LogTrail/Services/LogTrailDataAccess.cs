
namespace Device.Fingerprint.Features.LogTrail.Services
{
    using System.Threading.Tasks;
    using Device.Fingerprint.Features.LogTrail.Criterias;
    using Fanex.Data.Repository;

    public interface ILogTrailDataAccess
    {
        Task<int> InsertSummaryLogTrailAsync(string rawLogEvent);
    }
    public class LogTrailDataAccess : ILogTrailDataAccess
    {
        private readonly IDynamicRepository repository;

        public LogTrailDataAccess()
        {
            repository = new DynamicRepository();
        }

        public async Task<int> InsertSummaryLogTrailAsync(string rawLogEvent)
        {
            var criteria = new InsertSummaryLogTrailCriteria
            {
                RawLogEvent = rawLogEvent
            };

            return await repository.ExecuteAsync(criteria);
        }
    }
}
