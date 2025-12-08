using System;
using System.Collections.Generic;
using System.Linq;
using Recognizer.EvaluationTools.Untrimmed;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Menus.ClassifierEvaluation.View
{
    /// <summary>
    /// lance les events de la classe notification
    /// </summary>
    public class UntrimmedEvaluationView : UntrimmedEvaluationElement
    {
        public TextMeshProUGUI LabelsAndCount;
        public RectTransform Panel;
        public RectTransform AvailableMetrics;
        public RectTransform AvailableMetricsContent;
        public RectTransform AvailableAggregs;
        public RectTransform AvailableAggregsContent;
        public Text ListOfLabels;
        public Text ListOfLabelsPrediction;

        private float positionCounter;

        private string _currentTestClicked;
        public Selectable prefabMetricAggregLine;
        public Selectable prefabAvailability;
        private int _nbLines;
        private List<Selectable> Buttons = new List<Selectable>();
        private List<Selectable> _availabilityList = new List<Selectable>();


        /// <summary>
        /// Initialise la vue, affiche le nombre de donnée pour chaque classe et le nombre total
        /// </summary>
        public void InitView()
        {
            foreach (string classe in app.model.AppClassesLearned)
            {
                if(app.model.DataPerClass.ContainsKey(classe))
                    LabelsAndCount.text += classe + " : " + app.model.DataPerClass[classe].Count + " exemples \n";
            }

            LabelsAndCount.text += "\n Total examples : " +
                                   app.model.DataPerClass.Where((x) => app.model.AppClassesLearned.Contains(x.Key))
                                       .Aggregate(0, (acc, x) => acc + x.Value.Count);
            positionCounter = 0;
            _nbLines = 0;
        }

        /// <summary>
        /// Affiche le resultat de l'évaluation
        /// </summary>
        /// <param name="res"> le résultat</param>
        // public void DisplayResult()
        // {
        //     ResultView.SetActive(true);
        //     TitleTest.text = _currentTestClicked;
        //     EvaluationResult res = app.model.ResEval;
        //     FmesureValue.text = res.Fscore.ToString("0.00");
        //     PrecisionValue.text = res.Precision.ToString("0.00");
        //     RecallValue.text = res.Recall.ToString("0.00");
        //     ErrorRateValue.text = res.ErrorRate.ToString("0.00");
        //
        //
        //     //----the matrix----
        //     //clear the matrix
        //     foreach (Transform child in Matrix.transform)
        //     {
        //         GameObject.Destroy(child.gameObject);
        //     }
        //
        //     int count = res.ConfusionMatrix.Count + 1; //+1 for the label
        //     Matrix.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        //     Matrix.constraintCount = count;
        //     float width = Matrix.gameObject.GetComponent<RectTransform>().rect.width;
        //     float height = Matrix.gameObject.GetComponent<RectTransform>().rect.height;
        //     Matrix.cellSize = new Vector2((width - (count - 1) * Matrix.spacing.x) / count, (height - (count - 1) * Matrix.spacing.y) / count);
        //
        //     //float hei = 30 * count;
        //     MatrixHolder.rect.Set(MatrixHolder.rect.x, MatrixHolder.rect.y, MatrixHolder.rect.width,  MatrixHolder.rect.height);
        //
        //     Font arial;
        //     arial = (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        //     //labels : classes
        //     foreach (string classe in app.model.AppClassesLearned)
        //     {
        //         GameObject gm = new GameObject();
        //         Text text = gm.AddComponent<Text>();
        //         gm.transform.parent = Matrix.gameObject.transform;
        //         text.font = arial;
        //         text.text = classe;
        //         text.fontSize = 11;
        //         text.fontStyle = FontStyle.Bold;
        //         text.color = Color.black;
        //         text.alignment = TextAnchor.MiddleCenter;
        //         text.horizontalOverflow = HorizontalWrapMode.Overflow;
        //         text.verticalOverflow = VerticalWrapMode.Overflow;
        //         gm.transform.localPosition = Vector3.zero;
        //         gm.transform.localRotation = transform.rotation = Quaternion.identity;
        //         gm.transform.localScale = Vector3.one;
        //     }
        //
        //     //an empty one
        //     GameObject g = new GameObject();
        //     g.AddComponent<Text>();
        //     g.transform.parent = Matrix.gameObject.transform;
        //
        //     //fill teh matrix
        //     foreach (string classe in app.model.AppClassesLearned)
        //     {
        //         foreach (string c2 in app.model.AppClassesLearned)
        //         {
        //             float val = res.ConfusionMatrix[classe][c2];
        //
        //             GameObject gm = new GameObject();
        //             Text text = gm.AddComponent<Text>();
        //             text.font = arial;
        //             text.color = Color.black;
        //             text.fontSize = 13;
        //             text.alignment = TextAnchor.MiddleCenter;
        //             text.horizontalOverflow = HorizontalWrapMode.Overflow;
        //             text.verticalOverflow = VerticalWrapMode.Overflow;
        //             
        //             gm.transform.parent = Matrix.gameObject.transform;
        //             text.text = DoFormat(val);
        //             gm.transform.localPosition = Vector3.zero;
        //             gm.transform.localRotation = transform.rotation = Quaternion.identity;
        //             gm.transform.localScale = Vector3.one;
        //         }
        //
        //         //the label
        //         GameObject g2 = new GameObject();
        //         Text text2 = g2.AddComponent<Text>();
        //         text2.font = arial;
        //         text2.color = Color.black;
        //         text2.fontStyle = FontStyle.Bold;
        //         text2.alignment = TextAnchor.MiddleCenter;
        //         g2.transform.parent = Matrix.gameObject.transform;
        //         g2.transform.localPosition = Vector3.zero;
        //         g2.transform.localRotation = transform.rotation = Quaternion.identity;
        //         g2.transform.localScale = Vector3.one;
        //         text2.text = classe;
        //     }
        // }

        private static string DoFormat(float myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);

            if (s.EndsWith("00"))
            {
                return ((int) myNumber).ToString();
            }

            return s;
        }


      
        private void Update()
        {
            // if (app.model.NewValue)
            // {
            //     app.model.NewValue = false;
            //     // DisplayResult();
            // }
            HideIfClickedOutside();
        }
        private void HideIfClickedOutside() {
            if (Input.GetMouseButton(0) && AvailableMetrics.gameObject.activeSelf &&
                !RectTransformUtility.RectangleContainsScreenPoint(
                    AvailableMetrics,
                    Input.mousePosition,
                    null)) {
                HideScrollPanel(AvailableMetrics);
            }
            
            if (Input.GetMouseButton(0) && AvailableAggregs.gameObject.activeSelf &&
                !RectTransformUtility.RectangleContainsScreenPoint(
                    AvailableAggregs,
                    Input.mousePosition,
                    null)) {
                HideScrollPanel(AvailableAggregs);
            }
        }
        
        public void HideScrollPanel(RectTransform panel)
        {
            panel.gameObject.SetActive(false);
            //panel.GetComponent<CanvasGroup>().interactable = true;
            //panel.GetComponent<CanvasGroup>().alpha = 1.0f;
            foreach (Selectable gest in _availabilityList)
                Destroy(gest.gameObject);
            _availabilityList.Clear();
        }
        public void AddMetricAggregLine()
        {
            Selectable gestButton = Instantiate(prefabMetricAggregLine, Panel.transform);
            gestButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(Panel.anchoredPosition.x, Panel.anchoredPosition.y + positionCounter);
            positionCounter -=  35;
            
            /*Panel.GetComponent<RectTransform>().sizeDelta = new Vector2(Panel.GetComponent<RectTransform>().sizeDelta.x, 
                Panel.GetComponent<RectTransform>().sizeDelta.y + Panel.GetComponent<GridLayoutGroup>().cellSize.y);*/
                    
            gestButton.name = "";

            List<IUntrimmedMetric> metrics =  app.model.availableMetrics;
            List<IAggregationMetric> aggregations =  app.model.availableAggregations;

            UnityEngine.Assertions.Assert.IsTrue(metrics.Count>0,"No metrics available");
            UnityEngine.Assertions.Assert.IsTrue(aggregations.Count>0,"No aggregations available");
            
                
            Button[] buttons = gestButton.GetComponentsInChildren<Button>();
            buttons[1].GetComponentInChildren<Text>().text = metrics[0].GetType().Name; //metric text
            buttons[2].GetComponentInChildren<Text>().text = aggregations[0].GetType().Name; //aggreg text
            int theINdex = _nbLines;
            // These methods are for clicking with mouse
            buttons[1].onClick.AddListener(() => ShowAvaibleMetrics(theINdex)); // Replay button
            buttons[2].onClick.AddListener(() => ShowAvaibleAggregs(theINdex)); // Replay button
            Buttons.Add(gestButton);
            app.model.toDos.Add(new Tuple<IUntrimmedMetric, IAggregationMetric>(metrics[0],aggregations[0]));
            _nbLines += 1;
        }

        private void ShowAvaibleAggregs(int index)
        {
            float positionCounter = 0.0f;
            float counter = 0;
            AvailableAggregs.gameObject.SetActive(true);
            
            foreach (IAggregationMetric availability in app.model.availableAggregations)
            {
                counter++;
                Selectable availabilityButton = Instantiate(prefabAvailability, AvailableAggregsContent.transform);
                //availabilityButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(availableGestPanelContent.anchoredPosition.x, availableGestPanelContent.anchoredPosition.y+positionCounter);
                Text availabilityText = availabilityButton.GetComponentInChildren<Text>();
                availabilityText.text = availability.GetType().Name;
                IAggregationMetric theChoice = availability;
                availabilityButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ChangeAgreg(index, theChoice);
                });
                positionCounter -= 30.0f;
                _availabilityList.Add(availabilityButton);
            }
        }

        private void ChangeAgreg(int index, IAggregationMetric theChoice)
        {
            app.model.toDos[index] = new Tuple<IUntrimmedMetric, IAggregationMetric>(app.model.toDos[index].Item1,theChoice);
            Button[] buttons =  Buttons[index].GetComponentsInChildren<Button>();
            buttons[2].GetComponentInChildren<Text>().text = theChoice.GetType().Name;
        }

        private void ShowAvaibleMetrics(int index)
        {
            float positionCounter = 0.0f;
            float counter = 0;
            AvailableMetrics.gameObject.SetActive(true);
            
            foreach (IUntrimmedMetric availability in app.model.availableMetrics)
            {
                counter++;
                Selectable availabilityButton = Instantiate(prefabAvailability, AvailableMetricsContent.transform);
                //availabilityButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(availableGestPanelContent.anchoredPosition.x, availableGestPanelContent.anchoredPosition.y+positionCounter);
                Text availabilityText = availabilityButton.GetComponentInChildren<Text>();
                availabilityText.text = availability.GetType().Name;
                IUntrimmedMetric theChoice = availability;
                availabilityButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ChangeMetric(index, theChoice);
                });
                positionCounter -= 30.0f;
                _availabilityList.Add(availabilityButton);
            }
        }

        private void ChangeMetric(int index, IUntrimmedMetric theChoice)
        {
            app.model.toDos[index] = new Tuple<IUntrimmedMetric, IAggregationMetric>(theChoice,app.model.toDos[index].Item2);
            Button[] buttons =  Buttons[index].GetComponentsInChildren<Button>();
            buttons[1].GetComponentInChildren<Text>().text = theChoice.GetType().Name;
        }

        // private void RemoveLine(int index)
        // {
        //     print("remove at "+index);
        //     _nbLines-=1
        //     app.model.toDos.RemoveAt(index);
        //     Destroy(Buttons[index].gameObject);
        // }

        public void DisplayLabel(List<string> listsLabelsForSequence)
        {
            ListOfLabels.text = listsLabelsForSequence.Aggregate("", (x, y) => x + "__" + y);
        }

        public void AddLabelPrediction(List<string> s)
        {
            ListOfLabelsPrediction.text += s.Aggregate("",(x,y)=>x+"__"+y);
        }

        public void DisplayScores(List<float> currentScoreAggregatedForMetric)
        {
            for (var index = 0; index < currentScoreAggregatedForMetric.Count; index++)
            {
                float score = currentScoreAggregatedForMetric[index];
                Buttons[index].GetComponentsInChildren<Text>()[2].text = score.ToString("0.00") + "%";
            }
        }

        public void ClearLabels()
        {
            ListOfLabels.text = "";
            ListOfLabelsPrediction.text = "";
        }
    }
}