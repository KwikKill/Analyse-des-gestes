using System.Linq;
using AMVCC.Model.PlayerPkg;
using UnityEngine;

namespace AMVCC.Controller
{
    public class FighterController : CharacController
    {
        

        /// <summary>
        /// Grab a block if there is one in front of the player.
        /// </summary>
        public override void Ability()
        {
            Fighter fighter = (Fighter) App.Model.Game.Player.CharacterClass;
            if (!fighter.IsGrabbing)
            {
                var playerTransform = App.View.Player.transform;
                Vector3 forwardPos = playerTransform.position + playerTransform.forward;
                Log(forwardPos);

                // Retrieve the colliders with the Obstacle mask that are in front of the player.
                Collider[] colliders = Physics.OverlapSphere(forwardPos + Vector3.up * 0.5f, 0.25f, LayerMask.GetMask("GraspableBlock"));
                
                if (colliders.Any())
                {
                    Collider blockCollider = colliders.First();
                    blockCollider.transform.parent = playerTransform;
                    fighter.GrabbedBloc = blockCollider.gameObject;
                    fighter.IsGrabbing = true;
                }
            }
            else
            {
                if(fighter.GrabbedBloc != null)
                {
                    fighter.GrabbedBloc.transform.parent = App.View.Environment.transform;
                    fighter.GrabbedBloc = null;
                    fighter.IsGrabbing = false;
                }
            }
            App.View.Player.ActualiseGrab();
        }

        public override CharacterClass GetCharactClass()
        {
            return new Fighter();
        }

        public override Color GetColorClass()
        {
            return Color.gray;
        }


    }
}

