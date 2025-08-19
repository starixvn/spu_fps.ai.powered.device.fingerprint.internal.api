using Fanex.Data.Repository;
using Device.Fingerprint.Features.ClientTransaction.Criterias;

namespace Device.Fingerprint.Features.ClientTransaction.Services
{
    public interface IClientTransactionDataAccess
    {
        void InsertClientTransactionException(string exceptionJson);
    }

    public class ClientTransactionDataAccess : IClientTransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public ClientTransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void InsertClientTransactionException(string exceptionJson)
        {
            var criteria = new InsertClientTransactionExceptionCriteria
            {
                ExceptionJson = exceptionJson
            };

            repository.Execute(criteria);
        }
    }
}
