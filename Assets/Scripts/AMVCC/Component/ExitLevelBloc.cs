using System.Collections;
using UnityEngine;

namespace AMVCC.View
{
    public class ExitLevelBloc : MonoBehaviour
    {
        public Animator SphereAnimator;
        // Update is called once per frame
        void Update()
        {
         
        }

        private void OnTriggerEnter(Collider other)
        {
            SphereAnimator.SetBool("go",true);
            StartCoroutine(NextLevel());
        }

        IEnumerator NextLevel()
        {
            yield return new WaitForSeconds(1f);
            AppNotification.LevelSucceed();

        }
        
    }
}
