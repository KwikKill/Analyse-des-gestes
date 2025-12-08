using System.Collections.Generic;
using UnityEngine;

namespace Recognizer.EvaluationTools.Untrimmed
{
    public abstract class IAggregationMetric:MonoBehaviour
    {
        public abstract float Aggregate(List<float> computedMetrics);
    }
}