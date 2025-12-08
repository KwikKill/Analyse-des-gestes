using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AMVCC.Model.PlayerPkg
{
    public class Fighter : CharacterClass,ICanGrab
    {
        
        public GameObject GrabbedBloc { get; set; }
        public bool IsGrabbing { get; set; }

      

        public override ICollection<GameAction> GetAuthorizedActions()
        {
            //We can't change class if we are grabbing
            if (IsGrabbing)
            {
                List<GameAction> actions = new List<GameAction>(base.GetAuthorizedActions());
                actions.Remove(GameAction.SWITCH_CLASS);
                return actions;
            }

            return base.GetAuthorizedActions();
        }

        public override string GetName()
        {
            return "Fighter";
        }

        public override string GetDescriptionAbility()
        {
            return "Grab block";
        }
    }
}