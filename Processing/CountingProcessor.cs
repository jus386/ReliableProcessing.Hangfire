using System;
using System.Threading;
using DataAccess;
using Hangfire;
using Processing.Events;

namespace Processing
{
    public class CountingProcessor
    {
        private readonly ICalculationJobRepo _calculationJobRepo;

        public CountingProcessor(ICalculationJobRepo calculationJobRepo)
        {
            _calculationJobRepo = calculationJobRepo;
        }

        public void CountTo10Once()
        {
            var job = _calculationJobRepo.CreateJob(JobType.CountTo10);
            BackgroundJob.Enqueue(() => CountTo(job.Id));
        }

        public void CountTo20Once()
        {
            var job = _calculationJobRepo.CreateJob(JobType.CountTo20);
            BackgroundJob.Enqueue(() => CountTo(job.Id));
        }

        public void CountToRandomOnce()
        {
            var job = _calculationJobRepo.CreateJob(JobType.CountToRandom);
            BackgroundJob.Enqueue(() => CountTo(job.Id));
        }

        public void CountTo10ThanTo20Twice()
        {
            var job10 = _calculationJobRepo.CreateJob(JobType.CountTo10);
            var job201 = _calculationJobRepo.CreateJob(JobType.CountTo20);
            var job202 = _calculationJobRepo.CreateJob(JobType.CountTo20);

            var job10Id = BackgroundJob.Enqueue(() => CountTo(job10.Id));
            var job201Id = BackgroundJob.ContinueWith(job10Id, () => CountTo(job201.Id));
            BackgroundJob.ContinueWith(job201Id, () => CountTo(job202.Id));
        }

        public void CountTo10Then20ThenRandom()
        {
            var to10 = _calculationJobRepo.CreateJob(JobType.CountTo10);
            var to20 = _calculationJobRepo.CreateJob(JobType.CountTo20);
            var toRandom = _calculationJobRepo.CreateJob(JobType.CountToRandom);
            var to10JobId = BackgroundJob.Enqueue(() => CountTo(to10.Id));
            var to20JobId = BackgroundJob.ContinueWith(to10JobId, () => CountTo(to20.Id));
            BackgroundJob.ContinueWith(to20JobId, () => CountTo(toRandom.Id));
        }

        public void CountTo10In5()
        {
            var to10 = _calculationJobRepo.CreateJob(JobType.CountTo10);
            BackgroundJob.Schedule(() => CountTo(to10.Id), TimeSpan.FromSeconds(5));
        }

        public void CountTo10In5ThanRandom()
        {
            var to10 = _calculationJobRepo.CreateJob(JobType.CountTo10);
            var toRandom = _calculationJobRepo.CreateJob(JobType.CountToRandom);
            var to10Id = BackgroundJob.Schedule(() => CountTo(to10.Id), TimeSpan.FromSeconds(5));
            BackgroundJob.ContinueWith(to10Id, () => CountTo(toRandom.Id));
        }

        public void CountTo(int jobId)
        {
            if (DateTime.Now.Second%6 == 0)
            {
                throw new Exception("Random error here and there.");
            }
            var started = DateTime.Now;

            var job = _calculationJobRepo.GetJob(jobId);
            if (job == null || job.JobResult != null)
            {
                return;
            }

            var status = NotifyStarted(job);

            var step = 100.0 / job.JobParameter;
            for (int i = 0; i < job.JobParameter; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                NotifyProgress(status, step);
            }
            NotifyFinished(job, status, started);
        }

        private void NotifyFinished(CalculationJob job, JobStatus status, DateTime started)
        {
            var stopped = DateTime.Now;
            job.JobResult = string.Format("Started at {0}, counted {1}, completed at {2}", started, job.JobParameter, stopped);
            _calculationJobRepo.SaveChanges();
            EventAggregator.OnJobCompleted(this, status);
        }

        private void NotifyProgress(JobStatus status, double step)
        {
            status.Progress += step;
            EventAggregator.OnJobStatusChanged(this, status);
        }

        private JobStatus NotifyStarted(CalculationJob job)
        {
            var jobStat = new JobStatus
            {
                Id = job.Id.ToString(),
                JobType = job.JobType,
                Name = string.Format("Job {0} counting to {1}", job.Id, job.JobParameter),
                Progress = 0
            };
            EventAggregator.OnJobStatusChanged(this, jobStat);
            return jobStat;
        }
    }
}