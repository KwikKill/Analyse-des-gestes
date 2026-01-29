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

        public GameObject SwitchClassParticles;

        public Color[] CharacterClassColor;
        
        private float _jumpPreparationTimer = 0f;
        private const float JUMP_PREPARATION_DURATION = 1f;
        
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

            if (Animator.GetBool("IsJumping")) {
                Animator.SetBool("IsJumping", transform.position != _player.Destination);

                if (_jumpPreparationTimer > 0) {
                    _jumpPreparationTimer -= Time.deltaTime;
                } else {
                    transform.position = Vector3.MoveTowards(transform.position,
                    _player.Destination, Time.deltaTime * _player.JumpSpeed);
                }  
            } else {
                Animator.SetBool("IsWalking", transform.position != _player.Destination);
                Animator.SetBool("IsJumping", false);

                transform.position = Vector3.MoveTowards(transform.position,
                _player.Destination, Time.deltaTime * _player.Speed);
            }
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

        public void PlaySwitchClassEffect()
        {
            if (SwitchClassParticles != null)
            {
                Vector3 spawnPosition = transform.position + new Vector3(0, 1f, 0);
                GameObject particles = Instantiate(SwitchClassParticles, spawnPosition, Quaternion.identity);
                Destroy(particles, 2f);
            }
        }

        /// <summary>
        /// Do the jump animation
        /// </summary>
        public void Jump()
        {
            Animator.SetBool("IsJumping", true);
            _jumpPreparationTimer = JUMP_PREPARATION_DURATION;
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
                int grabbingLayerIndex = Animator.GetLayerIndex("Grabbing");
                if (grabbingLayerIndex != -1)
                {
                    Animator.SetLayerWeight(grabbingLayerIndex, isGrabbing ? 1.0f : 0.0f);
                }
            }
            else
            {
                Log("ActualiseGrab() should not be called atm.", 4);
            }
        }
        public void Fireball()
        {
            Animator.SetTrigger("Destroy");
        }

        public void ShurikenThrow()
        {
            Animator.SetTrigger("Shuriken");
        }
    }
}
