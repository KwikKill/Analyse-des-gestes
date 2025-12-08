using System.Collections.Generic;
using System.Linq;

namespace Recognizer.EvaluationTools.Untrimmed
{
    public class PrecisionMatchIndex:IUntrimmedMetric
    {
        public override float Compute(List<string> groundTruth, List<string> predictions)
        {
            int nb = predictions.Count;
            if (nb == 0)
                return 0;
            
            float score = 0;
            for (var index = 0; index < groundTruth.Count && index<predictions.Count; index++)
            {
                string gt = groundTruth[index];
                string pred = predictions[index];
                if (gt == pred)
                    score += 1f;
            }

            return score/nb*100;
        }
    }
}