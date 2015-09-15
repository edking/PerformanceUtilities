namespace PerformanceUtilities.ResultTypes
{
    public class SampleInfo
    {
        public string Name { get; set; }
        public double Mean { get; set; }
        public double Count { get; set; }
        public double StdDev { get; set; }
        public PerformanceResult Details { get; set; }
    }
}