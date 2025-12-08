using AMVCC.Model.PlayerPkg;
using UnityEngine;

namespace AMVCC.Controller
{
    public abstract class CharacController : AppElement
    {
        public abstract void Ability();

        public abstract CharacterClass GetCharactClass();

        public abstract Color GetColorClass();
    }
}
