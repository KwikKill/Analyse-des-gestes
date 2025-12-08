
using AMVCC.Component;
using AMVCC.Model.PlayerPkg;
using UnityEngine;

namespace AMVCC.Controller
{
    public class NinjaController : CharacController
    {
        public ThrowableObject ThrowableObject;
        public Transform LaunchPoint;

        public override void Ability()
        {
            ThrowableObject throwableObject = Instantiate(ThrowableObject, LaunchPoint.position, App.View.Player.transform.rotation,App.View.transform);
            Destroy (throwableObject.gameObject, 7.0f);
        }

       

        public override CharacterClass GetCharactClass()
        {
            return new Ninja();
        }

        public override Color GetColorClass()
        {
            return Color.black;
        }

    }
}

