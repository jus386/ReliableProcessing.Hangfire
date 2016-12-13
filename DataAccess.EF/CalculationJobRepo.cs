using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.EF
{
    public class CalculationJobRepo : ICalculationJobRepo
    {
        private readonly ProcessingDbContext _db;

        public CalculationJobRepo(ProcessingDbContext db)
        {
            _db = db;
        }

        public CalculationJob CreateJob(JobType type)
        {
            var jobParam = 10;
            if (type == JobType.CountTo20)
            {
                jobParam = 20;
            }
            else if (type == JobType.CountToRandom)
            {
                Random r = new Random();
                jobParam = r.Next(1, 50);
            }
            var newJob = new CalculationJob
            {
                JobType = type.ToString(),
                JobParameter = jobParam
            };
            _db.Jobs.Add(newJob);
            _db.SaveChanges();

            return newJob;
        }

        public CalculationJob GetJob(int jobId)
        {
            return _db.Jobs.FirstOrDefault(x => x.Id == jobId);
        }

        public IEnumerable<CalculationJob> GetJobsByParam(int result)
        {
            return _db.Jobs
                .Where(x => x.JobParameter == result);
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}
