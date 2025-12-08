using System.Collections.Generic;
using UnityEngine;

namespace Recognizer.EvaluationTools.Untrimmed
{
    public abstract class IUntrimmedMetric : MonoBehaviour
    {
        public abstract float Compute(List<string> groundTruth, List<string> predictions);
    }
}