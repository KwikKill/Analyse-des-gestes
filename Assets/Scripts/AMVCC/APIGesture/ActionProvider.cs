using System;
using System.Collections.Generic;
using System.Linq;
using AMVCC.Model;
using UnityEngine;

namespace AMVCC.APIGesture
{
    public class ActionProvider : IActionProvider
    {
        private Dictionary<string, GameAction> _actionsMap;
        private List<GameAction> _allActions;

        private void Awake()
        {
            _allActions= new List<GameAction>()
            {
                GameAction.MOVE_LEFT, 
                GameAction.MOVE_RIGHT, 
                GameAction.MOVE_UP, 
                GameAction.MOVE_DOWN, 
                GameAction.JUMP,
                GameAction.SWITCH_CLASS,
                GameAction.ABILITY,
            };
            _actionsMap= new Dictionary<string, GameAction>();
            
        }

        private void OnEnable()
        {
            LoadConfig();
        }


        /// <inheritdoc/>
        public override void UpdateMap(List<Tuple<string, string>> tuples)
        {
            _actionsMap.Clear();
            foreach (Tuple<string,string> tuple in tuples)
            {
                if(tuple.Item1!="")
                    _actionsMap.Add(tuple.Item1, (GameAction)Enum.Parse(typeof(GameAction), tuple.Item2));
            }
            SaveConfig();
        }

        /// <summary>
        /// Return the GameAction associated with the provided gesture string if it exists. Return GameAction.NONE otherwise.
        /// </summary>
        /// <param name="gestureName">string sent by the motor</param>
        /// <returns></returns>
        public GameAction GetActionFromGesture(string gestureName)
        {
            if (_actionsMap.ContainsKey(gestureName)) 
                return _actionsMap[gestureName];
            throw new Exception("Gesture "+gestureName+" not associated with a game action");
        }

        public override List<Tuple<string, string>> GetMapAction()
        {
            List<Tuple<string, string>> map = new List<Tuple<string, string>>();
            
            foreach (GameAction action in _allActions)
            {
                String gest = "";
                if (_actionsMap.ContainsValue(action))
                {
                    gest = _actionsMap.First(x => x.Value == action).Key;
                }
                
                map.Add(new Tuple<string, string>(gest, action.ToString()));
            }
            return map;
        }

        public void SaveConfig()
        {
            foreach (GameAction action in _allActions)
            {
                String gest = "";
                if (_actionsMap.ContainsValue(action))
                {
                    gest = _actionsMap.First(x => x.Value == action).Key;
                }
                PlayerPrefs.SetString(action.ToString(),gest);//save in the other to avoid erasing when no gesture affected
            }
            PlayerPrefs.Save();
        }

        public void LoadConfig()
        {
            _actionsMap.Clear();
            foreach (GameAction action in _allActions)
            {
                string gest = PlayerPrefs.GetString(action.ToString(),null);
                if (gest != null && gest!="")
                {
                    _actionsMap.Add(gest,action);
                }
            }

            /*if (_actionsMap.Count == 0)
            {
                _actionsMap = new Dictionary<string, GameAction>//default value
                {
                    {"Gauche", GameAction.MOVE_LEFT},
                    {"Droite", GameAction.MOVE_RIGHT}
                };
            }*/
              
        }
    }
}
