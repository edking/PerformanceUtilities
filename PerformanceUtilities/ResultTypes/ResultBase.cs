using PerformanceUtilities.ResultTypes.Formatters;

namespace PerformanceUtilities.ResultTypes
{
    public class ResultBase
    {
        protected ResultBase(FormatResultsBase f, int p)
        {
            Formatter = f;
            Formatter.Precision = p;
        }

        public FormatResultsBase Formatter { get; set; }

        public override string ToString()
        {
            return Formatter.Format(this);
        }
    }
}