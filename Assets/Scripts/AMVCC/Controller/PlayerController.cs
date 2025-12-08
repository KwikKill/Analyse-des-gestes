using System;
using System.Collections.Generic;
using AMVCC.Model.PlayerPkg;
using Leap.Unity;
using UnityEngine;

namespace AMVCC.Controller
{
    public class PlayerController : AppElement
    {
        public float Speed = 5f;
        private CharacController _currentCharacterController;

        public List<CharacController> CharacControllers;
        
   
        public LayerMask JumpMask;
        
        //FOR DEBUG
        private Vector3 _boxPosition;
        private Vector3 _boxScale;


        private void Awake()
        {
            // Adding local methods the following events so that they are called after a notification.
            AppNotification.MovePlayer += MovePlayer;
            AppNotification.Jump += Jump;
            AppNotification.SwitchClass += SwitchClass;
            AppNotification.Ability += Ability1;
        }

        private void OnDestroy()
        {
            AppNotification.MovePlayer -= MovePlayer;
            AppNotification.Jump -= Jump;
            AppNotification.SwitchClass -= SwitchClass;
            AppNotification.Ability -= Ability1;
        }

        private void Start()
        {
           
            
            if(CharacControllers.Count==0)
                throw new Exception("Need at least 1 character controller");
            if (App.View.Player == null)
                return;

            App.Model.Game.Player.Destination = App.View.Player.transform.position;
            _currentCharacterController = CharacControllers[0];
            App.View.Player.SetColorPlayer(0);
        }

        private void Update()
        {
            App.Model.Game.Player.Speed = Speed; // Update the model with Speed displayed in the inspector.
        }

        public void Jump()
        {
            Vector3 desiredDestination = App.Model.Game.Player.Destination
                                         +new Vector3(Seuil(App.View.Player.transform.forward.x),
                                             App.View.Player.transform.forward.y,
                                             Seuil(App.View.Player.transform.forward.z))
                                         * App.Model.Game.Player.CharacterClass.JumpDistance;

            if (IsJumpPossible(desiredDestination, App.View.Player.transform.forward))
            {
                App.Model.Game.Player.Destination = desiredDestination;
                App.View.Player.Jump();
            }
        }

        private int Seuil(float f)
        {
            return f < -0.5f ? -1:(f>0.5?1:0);
        }

        /// <summary>
        /// Method that asks for a move in a direction. Its result on the game depends on the move legitimacy.
        /// </summary>
        /// <param name="direction">Ex: Vector3.forward/left/right...</param>
        public void MovePlayer(Vector3 direction)
        {
            Player p = App.Model.Game.Player;

            Vector3 desiredDestination = p.Destination + direction;
            if (IsMovePossible(desiredDestination))
            {
                 p.Destination = desiredDestination;
            }
            else
            {
                if (p.CharacterClass is ICanGrab && ((ICanGrab) p.CharacterClass).IsGrabbing)
                    return;
                App.View.Player.transform.LookAt(direction * 2f + App.View.Player.transform.position);
            }
        }

        /// <summary>
        /// Verify if the player can move in the specified direction.
        /// A.k.a if there is a bloc he can walk on and no obstacle on his path.
        /// </summary>
        /// <param name="desiredNewPosition"></param>
        /// <returns></returns>
        private bool IsMovePossible(Vector3 desiredNewPosition)
        {
            // In case the current character class someone who can grab, we need to check whether he is grabbing a block or not.
            bool isFighterGrabbing = App.Model.Game.Player.CharacterClass is ICanGrab &&
                                     ((ICanGrab) App.Model.Game.Player.CharacterClass).IsGrabbing;

            int nbBlockGroundUnderPlayer = Physics.OverlapSphere(desiredNewPosition + Vector3.down * 0.5f, 0.25f).Length;

            // If the the player has grabbed a block then the moving conditions change. (We need to considerate the block)
            if (isFighterGrabbing)
            {
                // The position of the block is in front of the player
                Vector3 grabbedBlockFuturePos = desiredNewPosition + App.View.Player.transform.forward;
                int nbBlockInFrontBlock = Physics.OverlapSphere(grabbedBlockFuturePos + Vector3.up * 0.5f, 0.25f, JumpMask)
                    .Length;
                int nbBlockGroundBlock = Physics.OverlapSphere(grabbedBlockFuturePos   + Vector3.down * 0.5f, 0.25f)
                    .Length;
                return nbBlockInFrontBlock == 0 && nbBlockGroundBlock >= 1 && nbBlockGroundUnderPlayer>=1;
            }
            
            int nbBlockInFront = Physics.OverlapSphere(desiredNewPosition + Vector3.up * 0.5f, 0.25f).Length;
            return nbBlockInFront == 0 && nbBlockGroundUnderPlayer >= 1;
        }

        private bool IsJumpPossible(Vector3 desiredNewPosition, Vector3 moveDirection)
        {
            bool jumpPossible = IsMovePossible(desiredNewPosition);

            // Signed distance between the jumping destination and the player 
            Vector3 distance = desiredNewPosition - App.View.Player.transform.position;
            Vector3 boxPosition = desiredNewPosition - distance / 2 + Vector3.up * 0.5f;
            Vector3 boxScale = new Vector3(Math.Abs(distance.x / 2), 0.25f, Math.Abs(distance.z / 2));
            _boxPosition = boxPosition;
            _boxScale = boxScale;
            int nbBlockInFront = Physics.OverlapBox(boxPosition, boxScale, Quaternion.identity, JumpMask).Length;
            return jumpPossible && nbBlockInFront == 0;
        }

        public void SwitchClass()
        {
            Log("SwitchClass");
            int indexOfNextClass = (CharacControllers.IndexOf(_currentCharacterController) + 1) % (CharacControllers.Count);
            _currentCharacterController=CharacControllers[indexOfNextClass];
            App.Model.Game.Player.CharacterClass = _currentCharacterController.GetCharactClass();
            App.View.Player.SetColorPlayer(indexOfNextClass);
            App.View.UpdateKindPlayerAndAbility();

        }

        public void Ability1()
        {
            _currentCharacterController.Ability();
        }

      

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(_boxPosition,_boxScale*2);
        }
    }
}