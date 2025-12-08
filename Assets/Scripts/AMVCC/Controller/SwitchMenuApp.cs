using Menus;
using Recognizer;
using UnityEngine;

namespace AMVCC.Controller
{
    public class SwitchMenuApp : AppElement
    {
        public AppMenuItem Menu;
        public Application App;

        private bool _menuOpen;

     //   private Vector3 _initPosProvider;
        private AppNotificationForLeapReco _initNotifier;

        private void Start()
        {
            AppNotification.OpenMenu += OpenCloseMenu;

            
            _menuOpen = true;//will be change by the call
            if (RecoManager.GetInstance() != null)
                _initNotifier = RecoManager.GetInstance().notifier;
            OpenCloseMenu();
        }

        private void OnDestroy()
        {
            AppNotification.OpenMenu -= OpenCloseMenu;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                AppNotification.OpenMenu();
            }
        }

        private void OpenCloseMenu()
        {
            _menuOpen = !_menuOpen;
            if (_menuOpen)
            {
                Menu.gameObject.SetActive(true);
                App.gameObject.SetActive(false);
                if(RecoManager.GetInstance()!=null)
                	RecoManager.GetInstance().SetNotifier(Menu.gameObject.transform.GetComponentInChildren<AppNotificationForLeapReco>());
            }
            else
            {
                Menu.gameObject.SetActive(false);
                App.gameObject.SetActive(true);
                if (RecoManager.GetInstance() != null)
                {
                	if (RecoManager.GetInstance().State != RecoManager.StateCurrentReco.AppWorking)
                	{
                    	RecoManager.GetInstance().StopRecognizer();   
                    	RecoManager.GetInstance().StartAppRecognizer();   
                	}
                	RecoManager.GetInstance().SetNotifier(_initNotifier);
                }
            }
                
        }
    }
}