namespace DAL.Entities
{
    [Serializable]
    public class TestResult
    {
        public string TestTitle { get; set; } = string.Empty;
        public string StudentName { get; set; } = "Student";
        public double Percent { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public override string ToString()
            => $"{Date.Day}/{Date.Month}/{Date.Year} | {StudentName} - {TestTitle}: {Percent:F2}%";
    }
}
