using UnityEngine;

namespace AMVCC.Component
{
    public class FallingBlock : MonoBehaviour
    {

        public bool IsFalling = false;
        public float Speed = 5f;
        // Delay in second
        public float Delay = 2f;

        private float _currentTime = 0f;
        private float _triggerTime = 0f;
        

        // Update is called once per frame
        void Update()
        {
            if (IsFalling)
            {
                if (_currentTime >= _triggerTime + Delay)
                {
                    transform.Translate(Vector3.down * Time.deltaTime * Speed);
                }
                else
                {
                    _currentTime = Time.time;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            _triggerTime = _currentTime = Time.time;
            IsFalling = true;
            transform.GetComponent<Collider>().enabled = false;
            Destroy(this,5);
        }
    }
}
