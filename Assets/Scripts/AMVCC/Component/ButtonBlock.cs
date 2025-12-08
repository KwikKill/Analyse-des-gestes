using UnityEngine;

namespace AMVCC.Component
{
    public class ButtonBlock : MonoBehaviour
    {
        public int IdSwitch;
        private Renderer _renderer;
        private Color _initMaterialColor;
        private bool _grey;

        private void Start()
        {
            _renderer = gameObject.GetComponent<Renderer>();
            _initMaterialColor = _renderer.material.color;
            _grey = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            _grey = !_grey;
            _renderer.material.SetColor("_Color",_grey?Color.gray:_initMaterialColor);
            if (AppNotification.ActivSwitch != null)
                AppNotification.ActivSwitch(IdSwitch);
            else
                Debug.Log("Warning ; no script subscribed to any switch");
        }

     
    }
}