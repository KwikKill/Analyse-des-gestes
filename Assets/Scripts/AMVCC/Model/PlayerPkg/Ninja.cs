using System.Collections.Generic;
using System.Linq;

namespace AMVCC.Model.PlayerPkg
{
    public class Ninja : CharacterClass
    {
        public override int JumpDistance => 3;
        public override string GetName()
        {
            return "Ninja";
        }

        public override string GetDescriptionAbility()
        {            
            return "Throw shuriken";
        }
    }
}

