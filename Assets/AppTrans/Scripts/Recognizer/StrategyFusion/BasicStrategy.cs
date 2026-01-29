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
                _resetedHisto = false;
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
            // Parcourir les scores pour chaque classifieur
            foreach (var classifierScores in scores)
            {
                string classifierId = classifierScores.Key; // L'ID du classifieur

                // Trier les scores par ordre décroissant
                var sortedScores = classifierScores.Value.OrderByDescending(x => x.Value).ToList();

                if (sortedScores.Count < 2)
                    continue; // Il faut au moins 2 classes pour calculer β

                // Classe actuellement prédite (meilleur score)
                string predictedClass = sortedScores[0].Key;  // Predicted_i
                double predictedScore = sortedScores[0].Value;
                double secondBestScore = sortedScores[1].Value;

                // Calcul de β (différence entre Predicted_i et le second meilleur)
                double beta = predictedScore - secondBestScore;

                // Mise à jour de chaque entrée j de l'histogramme
                foreach (var scoreEntry in classifierScores.Value)
                {
                    string jthClass = scoreEntry.Key;  // jème classe
                    double jthScore = scoreEntry.Value; // score de la jème classe

                    // Calcul de γ pour la jème entrée
                    // γ = score(Predicted_i) - score(jème classe)
                    double gamma = predictedScore - jthScore;

                    // Mise à jour de l'histogramme His_i[j] += γ
                    if (histogrammes.ContainsKey(classifierId) &&
                        histogrammes[classifierId].ContainsKey(jthClass))
                    {
                        histogrammes[classifierId][jthClass] += gamma;
                    }
                }

                // Vérification des seuils pour la décision
                // Si β ≥ THETA → décision immédiate (forte confiance)
                if (beta >= THETA)
                {
                    decisionMade = true;
                    bestClass = predictedClass;
                    break;
                }

                // Si histogramme de la classe prédite ≥ PSI → décision par accumulation
                if (histogrammes.ContainsKey(classifierId) &&
                    histogrammes[classifierId].ContainsKey(predictedClass))
                {
                    if (histogrammes[classifierId][predictedClass] >= PSI)
                    {
                        decisionMade = true;
                        bestClass = predictedClass;
                        break;
                    }
                }
            }
            /*foreach (var classScores in scores)
            {
                string className = classScores.Key;

                // Trier les scores pour trouver les deux meilleurs
                var sortedScores = classScores.Value.OrderByDescending(x => x.Value).ToList();
                if (sortedScores.Count < 2)
                    continue; // S'assurer qu'il y a au moins deux classes pour calculer β

                string predictedClass = sortedScores[0].Key; // Classe prédite (Predicted_i)
                double predictedScore = sortedScores[0].Value;
                double secondBestScore = sortedScores[1].Value; // Deuxième meilleur score

                // Calcul de β
                double beta = predictedScore - secondBestScore;

                // Mettre à jour les histogrammes pour chaque classe
                foreach (var entry in classScores.Value)
                {
                    string targetClass = entry.Key;
                    double targetScore = entry.Value;

                    // Calcul de γ
                    double gamma = predictedScore - targetScore;

                    // Mise à jour de l'histogramme
                    histogrammes[className][targetClass] += gamma;

                    // Vérification des seuils
                    if (histogrammes[className][targetClass] >= PSI || beta >= THETA)
                    {
                        decisionMade = true;
                        bestClass = className;
                    }
                }
            }*/

            Dictionary<string, Dictionary<string, double>> histograms = histogrammes;

            if (decisionMade)
            {
                //We ask to the classifier to reset the curvilinear distance because we have taken a decision
                RecoManager.GetInstance().ResetCurDi();
                histograms = new Dictionary<string, Dictionary<string, double>>(histogrammes); //make a copy
                _resetedHisto = false;
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