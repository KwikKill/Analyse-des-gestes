using System.Collections.Generic;
using System.Linq;
using AMVCC.Component;
using AMVCC.Model;
using AMVCC.View;
using Menus;
using Recognizer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AMVCC.Controller
{
    public class GameController : AppElement
    {
        public List<Environment> Levels;
        private int _level = 0;

        public bool UseRecoManager = true;
        
        // Start is called before the first frame update
        private void Start()
        {
            AppNotification.LevelSucceed += NextLevel;

            if (UseRecoManager)
            {
                if (RecoManager.GetInstance()!=null && RecoManager.GetInstance().State != RecoManager.StateCurrentReco.AppWorking)
                {
                    RecoManager.GetInstance().StopRecognizer();   
                    RecoManager.GetInstance().StartAppRecognizer();   
                }
            }

            _level = 0;
            App.View.ShowNewLevel(Levels[_level]);
            App.Model.Game.NumberOfLevels = Levels.Count;
            App.Model.Game.CurrentLevel = _level+1;
            if (Levels.Count == 0)
                App.Model.Game.CurrentLevel = 0;
            else
                App.View.ShowNewLevel(Levels[_level]);

            App.View.UpdateNumberLevel();
        }

        private void OnDestroy()
        {
            AppNotification.LevelSucceed -= NextLevel;
        }

        private void NextLevel()
        {
            if (_level + 1 < Levels.Count)
            {
                _level++;
                App.View.ShowNewLevel(Levels[_level]);
                App.Model.Game.Player.Destination = Levels[_level].PlayerInitialPosition;
                App.Model.Game.CurrentLevel = _level+1;
                App.View.UpdateNumberLevel();
            } 
        }


        public void RestartGame()
        {
            if (RecoManager.GetInstance() != null)
                RecoManager.GetInstance().StopRecognizer();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
