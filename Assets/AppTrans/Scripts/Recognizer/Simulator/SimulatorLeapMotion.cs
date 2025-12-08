using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Menus;
using Recognizer;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AppTrans.Scripts.Recognizer.Simulator
{
    public class SimulatorLeapMotion : MonoBehaviour
    {
        
        static SimulatorLeapMotion _singleton = null;
        
        public static SimulatorLeapMotion GetInstance() { return _singleton; }

        public List<Vector3> currentPositions = new List<Vector3>();
        public bool DoCheckInput=true;

        public RectTransform textForAssociationKeyGesture;
        public TextMeshProUGUI prefabLine;
        
        private List<string> _gestesAvaible;
        private AnimationFileReader _fileReader;
        private List<string> _learntOnly;
        
        private List<KeyCode> keycodesForGesture = new List<KeyCode>()
        {
            KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3,KeyCode.Alpha4,KeyCode.Alpha5,KeyCode.Alpha6,KeyCode.Alpha7,
            KeyCode.Alpha8,KeyCode.Alpha9,KeyCode.Alpha0,
            KeyCode.E, KeyCode.R,KeyCode.T,KeyCode.Y,KeyCode.U,KeyCode.I,KeyCode.O,KeyCode.P,
            KeyCode.S,KeyCode.D,KeyCode.F,KeyCode.G,KeyCode.H,KeyCode.J,KeyCode.K,KeyCode.L,
            KeyCode.V,KeyCode.B,KeyCode.N
        };
        
        void Awake()
        {
            if (_singleton != null)
            {
                Destroy(this);
                // throw new System.Exception("An instance of this singleton already exists.");
            }
            else
            {
                _singleton = this;
                DoCheckInput = true;
            }

        }
        
        
        
   

        private void Start()
        {
            _gestesAvaible = DataManager.GetInstance().GetAllDataClasseName();
            _learntOnly = RecoManager.GetInstance().ParamRecoManager.GetClassesLearnt();
            _fileReader = FindObjectOfType<SimulatorProvider>().GetComponentInChildren<AnimationFileReader>();
            
            if(textForAssociationKeyGesture!=null)
                for (int i = 0; i < _gestesAvaible.Count; i++)
                {
                    TextMeshProUGUI text = Instantiate(prefabLine, textForAssociationKeyGesture.transform);
                    Color color = Color.black;
                    if(_learntOnly.Contains(_gestesAvaible[i]))
                        color= Color.red;
                    
                    text.text += keycodesForGesture[i].ToString().Replace("Alpha","")+" ; "+_gestesAvaible[i]+"\n";
                    text.color = color;
                    text.rectTransform.anchoredPosition = new Vector2(text.rectTransform.localPosition.x,i*-10);
                }
        }

        private void Update()
        {
            if(!DoCheckInput)
                return;
          
            for (var index = 0; index < keycodesForGesture.Count; index++)
            {
                KeyCode code = keycodesForGesture[index];
                if (Input.GetKeyDown(code))
                {
                    if (index < _gestesAvaible.Count)
                    {
                        Simulate(DataManager.GetInstance().GetGestureDataFromClassName(_gestesAvaible[index]));
                    }
                }
            }
        }


        public IEnumerator DoSimulate(GestureData data)
        {
            yield return new WaitForSeconds(0.5f);
            Simulate(data);
        }

        private void Simulate(GestureData data)
        {
            if (MenuNotificationGeneral.SimulationPressedLabel != null)
                MenuNotificationGeneral.SimulationPressedLabel(data.Classe);
            
            _fileReader.notifyUpdatePos = receivePLayCoordinates;
            _fileReader.LoadNewGeste(data.Path);
            _fileReader.Play();
        }

        private void receivePLayCoordinates(List<Vector3> coordinates)
        {
            currentPositions = coordinates;
        }
    }
}