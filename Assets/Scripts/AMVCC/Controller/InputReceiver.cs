using System;
using System.Collections.Generic;
using System.ComponentModel;
using AMVCC.APIGesture;
using AMVCC.Model;
using Recognizer;
using UnityEngine;

namespace AMVCC.Controller
{
    public class InputReceiver : AppElement
    {
        private ActionProvider _provider;
        private Queue<string> _actionsReceived;
        private float _lastMotorUpdate = 0f;

        private void Awake()
        {
            AppNotifForLeapReco.MotorInput += ReceivedMotorMessage;
        }

        private void OnDestroy()
        {
            AppNotifForLeapReco.MotorInput -= ReceivedMotorMessage;
        }

        private void Start()
        {
            _provider = FindObjectOfType<ActionProvider>();
            _actionsReceived = new Queue<string>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ActionNotify(GameAction.MOVE_LEFT);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ActionNotify(GameAction.MOVE_RIGHT);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ActionNotify(GameAction.MOVE_UP);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ActionNotify(GameAction.MOVE_DOWN);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ActionNotify(GameAction.JUMP);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                ActionNotify(GameAction.ABILITY);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                ActionNotify(GameAction.SWITCH_CLASS);
            }
            
         

            if (_actionsReceived.Count > 0)
            {
                MotorInput(_actionsReceived.Dequeue());
            }
        }

        public void MotorInput(string gesture)
        {
            App.Model.Game.ClassRecognized = gesture;
            GameAction action = _provider.GetActionFromGesture(gesture);
            ActionNotify(action);
        }

        public void ReceivedMotorMessage(string msg)
        {
            _actionsReceived.Enqueue(msg);
        }

        /// <summary>
        /// Notify the system that an action is requested.
        /// </summary>
        /// <param name="action"></param>
        private void ActionNotify(GameAction action)
        {
            if (action == GameAction.NONE)
            {
                Log("Unauthorized GameAction");
                throw new InvalidEnumArgumentException();
            }

            if (IsActionAccepted(action))
            {
                AppNotification.Actions[action](); // Call the Event linked to the GameAction.
            }
            else
            {
                Log($"GameAction {action.ToString()} not authorized in the current state of the game.");
            }
        }

        /// <summary>
        /// Method to be called when an Action is requested. Will check the model if it's a currently authorized action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool IsActionAccepted(GameAction action)
        {
            return App.Model.Game.Player.CharacterClass.GetAuthorizedActions().Contains(action);
        }
    }
}