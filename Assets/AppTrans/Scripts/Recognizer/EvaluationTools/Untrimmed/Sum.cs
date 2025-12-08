using System.Collections.Generic;
using System.Linq;

namespace Recognizer.EvaluationTools.Untrimmed
{
    public class Sum:IAggregationMetric
    {
        public override float Aggregate(List<float> computedMetrics)
        {
            return computedMetrics.Sum();
        }
    }
}