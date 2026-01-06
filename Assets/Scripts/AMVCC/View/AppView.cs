using AMVCC.Component;
using AMVCC.View;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AMVCC
{
    public class AppView : AppElement
    {
        public PlayerView Player => Assert<PlayerView>(_player);
        private PlayerView _player;
        public Environment Environment => Assert<Environment>(_environment);
        private Environment _environment;

        public TextMeshProUGUI LevelIndicator;
        public TextMeshProUGUI ClassPlayerIndicator;
        public TextMeshProUGUI AbilityPlayerIndicator;
        public Text text;


        private void Update()
        {
            UpdateGestureReco();
        }

        /// <summary>
        /// Update the view according to the recognized class
        /// </summary>
        public void UpdateGestureReco()
        {
 
            //TODO
            //you will have to use and update App.Model.Game.Reco (which is true when a class is recognized)
            //you will need the current recognized class : App.Model.Game.ClassRecognized
            if (App.Model.Game.Reco) {
                text.text = App.Model.Game.ClassRecognized;
                App.Model.Game.Reco = false;
            }

        }

        /// <summary>
        /// Load the new level passed in parameter, remove the last one
        /// </summary>
        /// <param name="envPrefab"></param>
        public void ShowNewLevel(Environment envPrefab)
        {
            if (_environment != null)
                Destroy(_environment.gameObject);

            _environment = Instantiate(envPrefab, gameObject.transform);
            Player.transform.position = _environment.PlayerInitialPosition;
        }

        /// <summary>
        /// Update the view by setting the current level number
        /// </summary>
        public void UpdateNumberLevel()
        {
            LevelIndicator.text = "Level " + App.Model.Game.CurrentLevel + "/" + App.Model.Game.NumberOfLevels;
        }
        
        /// <summary>
        /// Update the view by setting the name of the class (kind) of player and the ability according to the model
        /// </summary>
        public void UpdateKindPlayerAndAbility()
        {
            ClassPlayerIndicator.text =  App.Model.Game.Player.CharacterClass.GetName();
            AbilityPlayerIndicator.text =  App.Model.Game.Player.CharacterClass.GetDescriptionAbility();
        }
    }
}