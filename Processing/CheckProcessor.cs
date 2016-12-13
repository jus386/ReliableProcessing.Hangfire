using System.Linq;
using DataAccess;
using Hangfire;
using Processing.Events;

namespace Processing
{
    public class CheckProcessor
    {
        private readonly ICalculationJobRepo _calculationJobRepo;

        public CheckProcessor(ICalculationJobRepo calculationJobRepo)
        {
            _calculationJobRepo = calculationJobRepo;
        }

        public void CheckIfCountedTo20()
        {
            RecurringJob.AddOrUpdate(() => CheckCountedTo20(), Cron.Minutely);
        }

        public void CheckIfCountedTo42()
        {
            RecurringJob.AddOrUpdate(() => CheckCountedTo42(), Cron.Minutely);
        }

        public void CheckCountedTo20()
        {
            var jobsFor = _calculationJobRepo.GetJobsByParam(20);
            var jobsForResult = jobsFor.ToList();
            if (jobsForResult.Any())
            {
                var foundJob = jobsForResult.OrderByDescending(x => x.Id).First();
                var jobStat = new JobStatus
                {
                    Id = foundJob.Id.ToString(),
                    JobType = foundJob.JobType,
                    Name = foundJob.JobResult,
                    Progress = 100
                };

                EventAggregator.OnJobFound(this, jobStat);
            }
        }

        public void CheckCountedTo42()
        {
            var jobsFor = _calculationJobRepo.GetJobsByParam(42);
            var jobsForResult = jobsFor.ToList();
            if (jobsForResult.Any())
            {
                var foundJob = jobsForResult.OrderByDescending(x => x.Id).First();
                var jobStat = new JobStatus
                {
                    Id = foundJob.Id .ToString(),
                    JobType = foundJob.JobType,
                    Name = foundJob.JobResult,
                    Progress = 100
                };

                EventAggregator.OnJobFound(this, jobStat);
            }
        }
    }
}
