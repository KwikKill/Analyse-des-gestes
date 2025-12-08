using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AMVCC;
using AppTrans.Scripts.Recognizer.Simulator;
using Recognizer;
using Recognizer.EvaluationTools.Untrimmed;
using UnityEngine;

namespace Menus.ClassifierEvaluation.Controller
{
    
    
    
    /// <summary>
    /// </summary>
    public class UntrimmedEvaluationController : MenuController<UntrimmedEvaluationApplication>
    {
        public List<IUntrimmedMetric> Metrics;
        public List<IAggregationMetric> Aggregations;
        private List<string> queueDecision= new List<string>();
        private List<Action> sequenceToDo;
        private List<string> _currentGT;
        private List<string> _currentPred;
        private List<List<float>> _currentScoresForMetric;
        private List<float> _currentScoreAggregatedForMetric;
        private float _oldFrequence;
        private List<Action> endOfSequence;

        protected override void Start()
        {
            base.Start();
            RecoManager.GetInstance().StopRecognizer();
            if (!FindObjectOfType<IDeviceProvider>() is SimulatorProvider)
                throw new Exception("SIMULATOR not instancied, this menu is usable only with Simulator");
            StartCoroutine(StopSimulatorInput());
            app.model.AppClassesLearned = DataManager.GetInstance().GetAllDataClasseName();
            app.model.availableAggregations = Aggregations;
            app.model.availableMetrics = Metrics;
            
            Dictionary<string,Func<List<GestureData>>> dataClassesLazyAndHeaderDergOnly = DataManager.GetInstance().GetDataClassesLAZYAndHeaderDergOnly();
            foreach (KeyValuePair<string,Func<List<GestureData>>> valuePair in dataClassesLazyAndHeaderDergOnly)
            {
                if(valuePair.Key=="Sequence")
                    app.model.DataPerClass.Add(valuePair.Key,valuePair.Value());
            }
            
            
            _oldFrequence = RecoManager.FREQUENCY_RECORD_SEND_DATA;

            app.view.InitView();
            AddMetricField();
        }


        private IEnumerator StopSimulatorInput()
        {
            yield return new WaitForSeconds(0.5f);
            SimulatorLeapMotion.GetInstance().DoCheckInput = false;
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            MenuNotificationGeneral.GestureRecognized += PredictionMotor;
            try
            {
                RecoManager.GetInstance().StopRecognizer();
            }
            catch (Exception)
            {
            }
        }
        
        protected override void Update()
        {
            base.Update();


            if (queueDecision.Count > 0)
            {
                app.view.AddLabelPrediction(queueDecision);
                queueDecision.Clear();
            }
                
        }

        private void PredictionMotor(string obj)
        {
            queueDecision.Add(obj);
            _currentPred.Add(obj);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SimulatorLeapMotion.GetInstance().DoCheckInput = true;
            MenuNotificationGeneral.GestureRecognized -= PredictionMotor;

        }


        public void EvaluateUntrimmedSequences()
        {
            _currentScoresForMetric = new List<List<float>>();
            _currentScoreAggregatedForMetric = new List<float>();
            for (int i = 0; i < app.model.toDos.Count; i++)
            {
                _currentScoresForMetric.Add(new List<float>());
                _currentScoreAggregatedForMetric.Add(-1f);
            }
            _currentPred = new List<string>();
            _currentGT = new List<string>();
            
            RecoManager.FREQUENCY_RECORD_SEND_DATA /= 6; // réglable selon les performance de l'ordinateur, permet d'accelrer l'evaluation
                                                           // 6 = fast, 1 = normal speed
            Dictionary<string, List<GestureData>> dataClasses = app.model.DataPerClass;
            if(!dataClasses.ContainsKey("Sequence"))
                throw new Exception("No sequence to evaluate");
            List<GestureData> datas = dataClasses["Sequence"];
            sequenceToDo = new List<Action>();
            RecoManager.GetInstance().StartAppRecognizer();
            endOfSequence = new List<Action>();
            app.view.ClearLabels();

            for (var index = 0; index < datas.Count; index++)
            {
                GestureData data = datas[index];
                GestureData d = data;
                int id = index;
                Action f = () =>
                {
                    app.view.DisplayLabel(d.ListsLabelsForSequence());
                    StartCoroutine(SimulatorLeapMotion.GetInstance().DoSimulate(d));
                    _currentGT = d.ListsLabelsForSequence();
                    _currentPred.Clear();
                    endOfSequence.Add(() => EndOf(id));
                    MenuNotificationGeneral.EndSequence += endOfSequence[id];
                };
                sequenceToDo.Add(f);
            }

            sequenceToDo[0]();
        }

        private void EndOf(int id)
        {
            MenuNotificationGeneral.EndSequence -= endOfSequence[id];
            for (var index = 0; index < app.model.toDos.Count; index++)
            {
                Tuple<IUntrimmedMetric, IAggregationMetric> metric = app.model.toDos[index];
                float score = metric.Item1.Compute(_currentGT, _currentPred);
                _currentScoresForMetric[index].Add(score);
                print("Score for sequence "+id+" metric "+ metric.Item1.GetType().Name +" : "+score);
            }

            if (sequenceToDo.Count > id + 1)
            {
                app.view.ClearLabels();
                sequenceToDo[id + 1]();
            }
            else//the end of evaluation
            {
                RecoManager.FREQUENCY_RECORD_SEND_DATA = _oldFrequence;
                for (var index = 0; index < app.model.toDos.Count; index++)
                {
                    Tuple<IUntrimmedMetric, IAggregationMetric> metric = app.model.toDos[index];
                    float aggregation = metric.Item2.Aggregate(_currentScoresForMetric[index]);
                    _currentScoreAggregatedForMetric[index] = aggregation;
                }

                app.view.DisplayScores(_currentScoreAggregatedForMetric);
                RecoManager.GetInstance().StopRecognizer();
            }

        }

        public void AddMetricField()
        {
            app.view.AddMetricAggregLine();
        }

       
       
        
        protected override void MoveRight()
        {
        }

        protected override void MoveLeft()
        {
        }

        protected override void Enter()
        {
        }
        



      
    }
}