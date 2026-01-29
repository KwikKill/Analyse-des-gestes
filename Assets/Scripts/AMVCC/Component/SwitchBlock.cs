using UnityEngine;

namespace AMVCC.Component
{
    public class SwitchBlock : MonoBehaviour
    {
        public int IdSwitch;
        private Renderer _renderer;
        private Color _initMaterialColor;

        private void Start()
        {
            _renderer = gameObject.GetComponent<Renderer>();
            _initMaterialColor = _renderer.material.color;
        }

        /// <summary>
        /// Called when something walk on
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            _renderer.material.SetColor("_Color",Color.gray);
 
            //TODO : call the event AppNotification.ActivSwitch
            AppNotification.ActivSwitch(IdSwitch);

            // END TODO

        }
        
        /// <summary>
        /// Called when something walk off
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
 
            //TODO
            AppNotification.ActivSwitch(IdSwitch);
            
            _renderer.material.SetColor("_Color",_initMaterialColor);
        }
    }
}