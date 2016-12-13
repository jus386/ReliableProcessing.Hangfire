using System.Collections.Generic;

namespace DataAccess
{
    public interface ICalculationJobRepo
    {
        CalculationJob CreateJob(JobType type);

        CalculationJob GetJob(int jobId);

        IEnumerable<CalculationJob> GetJobsByParam(int result);

        void SaveChanges();
    }
}
