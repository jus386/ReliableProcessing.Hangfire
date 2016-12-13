using System;

namespace DataAccess
{
    public class CalculationJob
    {
        public int Id { get; set; }

        public string JobType { get; set; }

        public int JobParameter { get; set; }

        public string JobResult { get; set; }
    }
}