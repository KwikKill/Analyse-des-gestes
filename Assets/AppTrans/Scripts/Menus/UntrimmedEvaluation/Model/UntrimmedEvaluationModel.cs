using System;
using System.Collections.Generic;
using Recognizer;
using Recognizer.EvaluationTools;
using Recognizer.EvaluationTools.Untrimmed;

namespace Menus.ClassifierEvaluation.Model
{
    public class UntrimmedEvaluationModel
    {
        public List<string> AppClassesLearned { get; set; }
        public Dictionary<string, List<GestureData>> DataPerClass{ get; set; } = new Dictionary<string, List<GestureData>>();

        public bool NewValue { get; set; } = false;
        private EvaluationResult _resEval;
        public List<IUntrimmedMetric> availableMetrics;
        public List<IAggregationMetric> availableAggregations;
        public List<Tuple<IUntrimmedMetric,IAggregationMetric>> toDos = new List<Tuple<IUntrimmedMetric, IAggregationMetric>>();

        public EvaluationResult ResEval
        {
            get { return _resEval;}
            set
            {
                _resEval = value;
                NewValue = true;
            }
        }
    }
}