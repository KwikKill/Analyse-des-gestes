using System.Collections.Generic;

namespace Recognizer.EvaluationTools.Untrimmed
{
    public class ExactMatch:IUntrimmedMetric
    {
        public override float Compute(List<string> groundTruth, List<string> predictions)
        {
            if (groundTruth.Count != predictions.Count)
                return 0;
            for (var index = 0; index < groundTruth.Count; index++)
            {
                string gt = groundTruth[index];
                string pred = predictions[index];
                if (gt != pred)
                    return 0;
            }

            return 100;
        }
    }
}