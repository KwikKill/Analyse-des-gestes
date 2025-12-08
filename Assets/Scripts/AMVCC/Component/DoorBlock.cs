using System.Collections;
using UnityEngine;

namespace AMVCC.Component
{
    public class DoorBlock : MonoBehaviour
    {
        public int SwitchId;
        private bool _open;
        private Vector3 _initPos;

        private void Awake()
        {
            AppNotification.ActivSwitch += DoOpenClose;
        }

        private void OnDestroy()
        {
            AppNotification.ActivSwitch -= DoOpenClose;
        }

        private void Start()
        {
            _open = false;
            _initPos = transform.position;
        }

        private void DoOpenClose(int switchId)
        {
            if (switchId == SwitchId)
            {
                _open = !_open;
                StartCoroutine(MoveFunction(_initPos + Vector3.up * (_open ? 2 : 0),_open));
            }
        }
        
        IEnumerator MoveFunction(Vector3 newPosition,bool state)
        {
            float timeSinceStarted = 0f;
            while (true)
            {
                if(_open!=state)
                    yield break;
                    
                timeSinceStarted += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, newPosition, timeSinceStarted);

                // If the object has arrived, stop the coroutine
                if (transform.position == newPosition)
                {
                    yield break;
                }

                // Otherwise, continue next frame
                yield return null;
            }
        }
    }
}