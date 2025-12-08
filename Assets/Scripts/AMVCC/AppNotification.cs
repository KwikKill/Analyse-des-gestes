using System.Collections.Generic;
using AMVCC.Model;
using UnityEngine;

namespace AMVCC
{
    public class AppNotification
    {
        //Actions
        public static System.Action<Vector3> MovePlayer;
        public static System.Action Jump;
        public static System.Action SwitchClass;
        public static System.Action Ability;
        
        //other events
        public static System.Action<int> ActivSwitch;
        public static System.Action LevelSucceed;
        public static System.Action OpenMenu;

        public static Dictionary<GameAction, System.Action> Actions = new Dictionary<GameAction, System.Action>
        {
            {GameAction.MOVE_LEFT, () => MovePlayer(Vector3.left)},
            {GameAction.MOVE_RIGHT, () => MovePlayer(Vector3.right)},
            {GameAction.MOVE_UP, () => MovePlayer(Vector3.forward)},
            {GameAction.MOVE_DOWN, () => MovePlayer(Vector3.back)},
            {GameAction.JUMP, () => Jump() },
            {GameAction.SWITCH_CLASS, () => SwitchClass() },
            {GameAction.ABILITY, () => Ability() }
        };

    }
}
