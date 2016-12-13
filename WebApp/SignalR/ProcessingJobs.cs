using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Processing.Events;

namespace WebApp.SignalR
{
    public class ProcessingJobs
    {
        private static readonly Lazy<ProcessingJobs> _instance =
            new Lazy<ProcessingJobs>(
                () => new ProcessingJobs(GlobalHost.ConnectionManager.GetHubContext<ProcessingJobsHub>().Clients));

        private readonly ConcurrentDictionary<string, JobStatus> _jobStatuses = new ConcurrentDictionary<string, JobStatus>();

        private readonly object _updateJobsLock = new object();
        private volatile bool _updateJobs = false;

        public static ProcessingJobs Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        private ProcessingJobs(IHubConnectionContext<object> clients)
        {
            Clients = clients;

            EventAggregator.JobStatusChanged += EventAggregator_JobStatusChanged;
            EventAggregator.JobCompleted += EventAggregator_JobCompleted;
            EventAggregator.JobCountFound += EventAggregator_JobCountFound;
        }

        private void EventAggregator_JobCountFound(object sender, JobStatus e)
        {
            Clients.All.foundJobCount(e);
        }

        private void EventAggregator_JobCompleted(object sender, JobStatus e)
        {
            UpdateJobCompleted(e);
        }

        private void EventAggregator_JobStatusChanged(object sender, JobStatus e)
        {
            UpdateJobStatus(e);
        }

        public IEnumerable<JobStatus> GetAllStatuses()
        {
            return _jobStatuses.Values;
        }

        private void UpdateJobCompleted(JobStatus stat)
        {
            lock (_updateJobsLock)
            {
                if (!_updateJobs)
                {
                    _updateJobs = true;

                    if (_jobStatuses.ContainsKey(stat.Id))
                    {
                        JobStatus removed;
                        if (_jobStatuses.TryRemove(stat.Id, out removed))
                        {
                            Clients.All.updateJobComplete(removed);
                        }
                    }
                    _updateJobs = false;
                }
            }
        }

        public void UpdateJobStatus(JobStatus stat)
        {
            lock (_updateJobsLock)
            {
                if (!_updateJobs)
                {
                    _updateJobs = true;

                    if (_jobStatuses.ContainsKey(stat.Id))
                    {
                        var jobStatus = _jobStatuses[stat.Id];
                        jobStatus.Progress = stat.Progress;
                        Clients.All.updateJobStatus(jobStatus);
                    }
                    else
                    {
                        if (_jobStatuses.TryAdd(stat.Id, stat))
                        {
                            Clients.All.updateJobStatus(stat);
                        }
                    }
                    _updateJobs = false;
                }
            }
        }
    }
}