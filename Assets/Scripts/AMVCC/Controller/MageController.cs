using AMVCC.Model.PlayerPkg;
using UnityEngine;

namespace AMVCC.Controller
{
    public class MageController :  CharacController
    {
        /// <summary>
        /// Breaks an obstacle block if there is one in front of the player.
        /// </summary>
        public override void Ability()
        {
            var playerTransform = App.View.Player.transform;
            Vector3 forwardPos = playerTransform.position + playerTransform.forward;
            Log(forwardPos);

            // Retrieve the colliders with the Obstacle mask that are in front of the player.
            Collider[] colliders = Physics.OverlapSphere(forwardPos + Vector3.up * 0.5f, 0.25f, LayerMask.GetMask("Obstacle"));
            foreach (Collider col in colliders)
            {
                Destroy(col.gameObject);
            }
        }

      

        public override CharacterClass GetCharactClass()
        {
            return new Mage();
        }

        public override Color GetColorClass()
        {
            return Color.blue;

        }


    }
}

