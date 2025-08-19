using Device.Fingerprint.Features.FPDevices.Criteria;
using Device.Fingerprint.Features.FPDevices.Criterias;
using Device.Fingerprint.Features.FPDevices.Entities;
using Device.Fingerprint.Model;
using Fanex.Data;
using Fanex.Data.Repository;
using System.Collections.Generic;

namespace Device.Fingerprint.Features.FPDevices.Services
{

    public interface IFPDeviceDataAccess
    {
        IEnumerable<FPStagingTransIdEntity> GetFPStagingTransIdForDevice(int size, int noOfBatch);

        DataResult UpdateDevice(string transIdJson);
    }

    public class FPDeviceDataAccess : IFPDeviceDataAccess
    {
        public IEnumerable<FPStagingTransIdEntity> GetFPStagingTransIdForDevice(int size, int noOfBatch)
        {
            var criteria = new GetFPDeviceStagingTransactionPackageCriteria
            {
                Size = size,
                NoOfBatch = noOfBatch
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.Query<FPStagingTransIdEntity>(criteria);
        }

        public DataResult UpdateDevice(string transIdJson)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateFPDeviceIdCriteria
            {
                TransIdJson = transIdJson
            };

            var errCode = repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errCode
            };
        }
    }
}
