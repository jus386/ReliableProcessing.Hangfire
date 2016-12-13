using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Processing.Events;

namespace WebApp.SignalR
{
    [HubName("processingJobsHub")]
    public class ProcessingJobsHub : Hub
    {
        private readonly ProcessingJobs _procJobs;

        public ProcessingJobsHub() : this(ProcessingJobs.Instance) { }

        public ProcessingJobsHub(ProcessingJobs procJobs)
        {
            _procJobs = procJobs;
        }

        public IEnumerable<JobStatus> GetAllStatuses()
        {
            return _procJobs.GetAllStatuses();
        }
    }
}