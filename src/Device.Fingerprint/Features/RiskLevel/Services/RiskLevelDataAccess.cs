using Device.Fingerprint.Features.RiskLevel.Criterias;
using Device.Fingerprint.Features.RiskLevel.Model;
using Device.Fingerprint.Features.Transaction.Criterias;
using Device.Fingerprint.Features.Transaction.Model;
using Device.Fingerprint.Model;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Device.Fingerprint.Features.RiskLevel.Services
{
    public interface IRiskLevelDataAccess
    {
        IEnumerable<RiskLevelEntity> GetCustomerClassRiskLevel(int subscriberId, long lastId, int batchSize);
        IEnumerable<RiskLevelByAccountEntity> GetCustomerClassRiskLevelByAccount(string accountIDs);
    }

    public class RiskLevelDataAccess : IRiskLevelDataAccess
    {
        private readonly IDynamicRepository repository;

        public RiskLevelDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RiskLevelEntity> GetCustomerClassRiskLevel(int subscriberId, long lastId, int batchSize)
        {
            var criteria = new GetLatestRiskLevelCriteria
            {
                 SubscriberId = subscriberId,
                 LastId = lastId,
                 BatchSize = batchSize
            };

            return repository.Fetch<RiskLevelEntity>(criteria);
        }

        public IEnumerable<RiskLevelByAccountEntity> GetCustomerClassRiskLevelByAccount(string accountIDs)
        {
            var criteria = new GetRiskLevelByAccountCriteria
            {
                AccountIDs = accountIDs
            };

            return repository.Fetch<RiskLevelByAccountEntity>(criteria);
        }
    }
}
