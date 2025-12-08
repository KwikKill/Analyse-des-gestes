using UnityEngine;

namespace AMVCC.Component
{
    public class ThrowableObject : MonoBehaviour
    {
        public float Speed = 8f;
    
        // Start is called before the first frame update
        void Start()
        {
            Speed = 8f;
        
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(transform.forward * Speed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("TRIGGER");
            Destroy(gameObject);
        }
    }
}
