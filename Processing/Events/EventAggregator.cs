using System;

namespace Processing.Events
{
    public class EventAggregator
    {
        public static event EventHandler<JobStatus> JobStatusChanged;

        public static event EventHandler<JobStatus> JobCompleted;

        public static event EventHandler<JobStatus> JobCountFound;

        public static void OnJobStatusChanged(object sender, JobStatus e)
        {
            JobStatusChanged?.Invoke(sender, e);
        }

        public static void OnJobCompleted(object sender, JobStatus e)
        {
            JobCompleted?.Invoke(sender, e);
        }

        public static void OnJobFound(object sender, JobStatus e)
        {
            JobCountFound?.Invoke(sender, e);
        }
    }
}
