using AMVCC.Component;
using AMVCC.Model.PlayerPkg;
using UnityEngine;

namespace AMVCC.View
{
    public class PlayerView : AppElement
    {


        private Material _mat;
        private Player _player;
        
        public Animator Animator;

        public Color[] CharacterClassColor;
        
        private void Start()
        {
            _mat = GetComponentsInChildren<Renderer>()[1].sharedMaterial;
            _player = App.Model.Game.Player;
        }

        private void Update()
        {
            // Orient player as long as he is not grabbing an object
            if (!(_player.CharacterClass is Fighter && ((Fighter) _player.CharacterClass).IsGrabbing))
            {
                transform.LookAt(App.Model.Game.Player.Destination);    
            }
            
            transform.position = Vector3.MoveTowards(transform.position,
                _player.Destination, Time.deltaTime * _player.Speed);
        }

        /// <summary>
        /// Set the color of the character
        /// </summary>
        /// <param name="controllerTypeIndex">Index of the CharacterClass in the public array in the PlayerController</param>
        public void SetColorPlayer(int controllerTypeIndex)
        {
            if (controllerTypeIndex > CharacterClassColor.Length)
            {
                Debug.Log("Update the array so that a color is associated with this class");
            }
            else
            {
                _mat.color = CharacterClassColor[controllerTypeIndex];
            }
        }

        /// <summary>
        /// Do the jump animation
        /// </summary>
        public void Jump()
        {
        }

        /// <summary>
        /// Do the grabbing animation
        /// </summary>
        public void ActualiseGrab()
        {
            // Should always be true
            if (_player.CharacterClass is ICanGrab)
            {
                ICanGrab f = (ICanGrab) _player.CharacterClass;
                bool isGrabbing = f.IsGrabbing;

            }
            else
            {
                Log("ActualiseGrab() should not be called atm.", 4);
            }
        }
        
        
    }
}
