using System.Collections.Generic;
using System.Linq;

namespace AMVCC.Model.PlayerPkg
{
    public abstract class CharacterClass
    {
        public virtual int JumpDistance => 2;
        
        private readonly ICollection<GameAction> _defaultActions;

        protected CharacterClass()
        {
            // Must list here all the actions that are authorized for every player class.
            _defaultActions = new List<GameAction>(new [] { GameAction.MOVE_UP, GameAction.MOVE_DOWN, GameAction.MOVE_LEFT, GameAction.MOVE_RIGHT, GameAction.SWITCH_CLASS, GameAction.JUMP,GameAction.ABILITY });
        }

        public virtual ICollection<GameAction> GetAuthorizedActions()
        {
            return _defaultActions;
        }

        public abstract string GetName();
        public abstract string GetDescriptionAbility();

    }
}
    