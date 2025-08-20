namespace Patient.Contracts.Models
{
    public class MonthlyCasesBO
    {
        public long Month { get; set; }
        public long Year { get; set; }
        public long CasesCount { get; set; }
    }
    public class QEResultStatBO
    {
        public long QEResultId { get; set; }
        public long CasesCount { get; set; }
    }
    public class GoalCompletedBO
    {
        public long NumberCompleted { get; set; }
        public double PercentCompleted { get; set; }
        public long Goal { get; set; }
    }
}