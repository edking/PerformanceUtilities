using System;

namespace PerformanceUtilities.ResultTypes.Formatters
{
    public abstract class FormatResultsBase
    {
        public virtual int Precision { get; set; }

        protected virtual string NumFormat
        {
            get { return String.Format("N{0}", Precision); }
        }

        public abstract string Format(ResultBase result);
    }
}