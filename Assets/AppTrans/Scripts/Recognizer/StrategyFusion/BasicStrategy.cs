using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Recognizer;
using Recognizer.StrategyFusion;

namespace AppTrans.Scripts.Recognizer.StrategyFusion
{
    /// <summary>
    /// Will return a complete ResultStrategy
    /// The histograms are calculated using the Eric Anquetil's AMRG course, and the decision is taken considering its
    /// NOT Compatible with the old version of the RecognizerServer
    /// </summary>
    public class BasicStrategy : StrategyRecognizer,ISpecificPsiTheta
    {
        private Dictionary<string, Dictionary<string, double>> histogrammes;
        private List<string> _appClassesLearned;
        private bool _resetedHisto;

        public double THETA = 5;
        public double PSI = 10;
        private RecoManager.StateCurrentReco _status;

        public BasicStrategy()
        {
            histogrammes = new Dictionary<string, Dictionary<string, double>>();
        }



        /// <summary>
        /// Called each time the classifier answer
        /// </summary>
        /// <param name="brutResult">the brut result given by  the classifier</param>
        /// <returns>A complete ResultStrategy</returns>
        public override ResultStrategy OnFrameRecognitionResult(Dictionary<string, double> brutResult)
        {
            if (_appClassesLearned == null || RecoManager.GetInstance().State != _status)
            {
                _appClassesLearned = RecoManager.GetInstance().ParamRecoManager.GetClassesLearnt();
                _status = RecoManager.GetInstance().State; //to prevent change of classifier
                ResetHistogram();
            }

            Dictionary<string, Dictionary<string, double>>
                scores = new Dictionary<string, Dictionary<string, double>>();

            
            bool haveResult = false;
            string patternAll = @"^all_C(\d+)\.(.+)";
            //loop to fill the "scores" variable
            foreach (KeyValuePair<string, double> p in brutResult)
            {
                Regex regex = new Regex(patternAll);
                MatchCollection matchesAll = regex.Matches(p.Key);
                foreach (Match matched in matchesAll)
                {
                 
                    haveResult = true;
                    int cid = int.Parse(matched.Groups[1].Value) - 1;
                    if (!scores.ContainsKey(_appClassesLearned[cid]))
                        scores.Add(_appClassesLearned[cid], new Dictionary<string, double>());

                    string classScored = matched.Groups[2].Value;
                    double score = p.Value;
                    scores[_appClassesLearned[cid]].Add(classScored, score);
                }
            }

            if (!haveResult) //pas de valeurs renvoyés -> pas encore de score ou perte de tracking du leap
            {
                ResetHistogram();
                _resetedHisto = true;
            }

            bool decisionMade = false; //To update if the decision is taken
            string bestClass = null; //To update if the decision is taken

 
            //TODO : complete the code here
            //You will have to loop over the "scores" variable (which is already filled) in the code above
            //to fill the "histogrammes variables"
            //You will have to calculate beta and gamma to fill the histograms variable
            //You will also have to check if the histograms values exceed the treshold Psi or Theta,
            //and if it's the case, update the 'decisionMade' and 'bestClass' variable

            // END TODO


            Dictionary<string, Dictionary<string, double>> histograms = histogrammes;

            if (decisionMade)
            {
                //We ask to the classifier to reset the curvilinear distance because we have taken a decision
                RecoManager.GetInstance().ResetCurDi();
                histograms = new Dictionary<string, Dictionary<string, double>>(histogrammes); //make a copy
                ResetHistogram();
                _resetedHisto = true;
            }

            return new ResultStrategy(decisionMade, bestClass, scores, histograms);
        }

        public override void NewClassesLearnt()
        {
            _appClassesLearned = null;
        }


        /// <summary>
        /// Initialize the dataStructure which contains the histograms values
        /// </summary>
        private void ResetHistogram()
        {
            if(_resetedHisto)
                return;
            histogrammes.Clear();
            foreach (string g1 in _appClassesLearned)
            {
                histogrammes.Add(g1, new Dictionary<string, double>());
                foreach (string g2 in _appClassesLearned)
                {
                    histogrammes[g1].Add(g2, 0);
                }
            }
        }

        public double GetTheta()
        {
            return THETA;
        }

        public double GetPsi()
        {
            return PSI;
        }
    }
}