using System;
using System.Collections;
using System.IO;
using Leap.Unity;
using RecognitionClient;
using Recognizer.EvaluationTools;
using Recognizer.StrategyFusion;
using UnityEngine;

namespace Recognizer
{
    public class RecoManager : MonoBehaviour
    {
        public enum StatusReco
        {
            Work,NotWorkingAndTrying
        }

        public static float FREQUENCY_RECORD_SEND_DATA = 0.025f;//data sent/captured each 0.025s, or 40 times per second
        
        static RecoManager _singleton = null;
        public static RecoManager GetInstance() { return _singleton; }


        public bool ShowServerCmd = true;

        public AppNotificationForLeapReco notifier;
        public StrategyRecognizer StrategyAppRecognizer;
        public StrategyRecognizer StrategyMenuRecognizer;
        public bool debug = false;


        public enum StateCurrentReco
        {
            NotWorking,AppWorking,MenuWorking
        }
        
        public StateCurrentReco State { get; private set; }
        
        private int _nbFrame = 0;
        
        private double[] _frame ;
        private RecognitionClient.RecognitionClientC _recoClient;

        private DevicesInfos.Device _previousDevice;
        public DevicesInfos.Device Device;

        public GameObject LeapProvider;
        public GameObject SimulatorProvider;

        [NonSerialized]
        public DevicesInfos.DeviceInfo DeviceInfo;

        
        private DateTime _lastReceived;
        private static float TIME_TO_RETRY_CONNECT_AFTER_LAST_RECEIVED=2f;
     
        [NonSerialized]
        public StatusReco LastStatus;
        private bool _pause = false;
        private System.Diagnostics.Process _serverExe;


        public ParamRecoManager ParamRecoManager { get; } = new ParamRecoManager();
        public IClassifierManager ClassifierManager { get; } = new RecoClassifierManager();

        private void Debug(string s)
        {
            if (debug)
                UnityEngine.Debug.Log(s);
        }

        private void OnValidate()
        {
            ChangeDeviceTo(Device);
        }

        public void ChangeDeviceTo(DevicesInfos.Device value)
        {
            if (_previousDevice != value)
            {
                try
                {
                    switch (value)
                    {
                        case DevicesInfos.Device.LeapMotion:
                            SimulatorProvider?.SetActive(false);
                            LeapProvider?.SetActive(true);
                            break;
                        case DevicesInfos.Device.SimulorLeapMotion:
                            LeapProvider?.SetActive(false);
                            SimulatorProvider?.SetActive(true);
                            break;
                        default:
                            UnityEngine.Debug.Log("Unsupported Device Information");
                            return;
                    }
                }
                catch (UnassignedReferenceException) { }

                _previousDevice = value;
                Device = value;
            }
        }

        void Awake()
        {
            if (_singleton != null)
            {
                Destroy(this);
               // throw new System.Exception("An instance of this singleton already exists.");
            }
            else
            {
                _singleton = this;
            }
            State = StateCurrentReco.NotWorking;
            DeviceInfo = DevicesInfos.Infos[Device];
            _frame = new double[DeviceInfo.FrameSize];
            _previousDevice = Device;
        }

        private void Start()
        {
            StartCoroutine(SendLeapInfo());
            _previousDevice = Device;
        }

     

        public void SetNotifier(AppNotificationForLeapReco notifier)
        {
            this.notifier = notifier;
        }

        public void StartAppRecognizer()
        {
            State = StateCurrentReco.AppWorking;
            string exePath = System.IO.Path.GetFullPath("./RecognitionServer/AppRecognizer/");
            StartServer(exePath);
            StartRecoClient();
        }
        
        public void StartMenuRecognizer()
        {
            State = StateCurrentReco.MenuWorking;
            string exePath = System.IO.Path.GetFullPath("./RecognitionServer/MenuRecognizer/");
            StartServer(exePath);
            StartRecoClient();
        }
        
     
      

        public void RelearnAndStart()
        {
            StopRecognizer();
            try{
                string directoryPath = System.IO.Path.GetFullPath("./RecognitionServer/AppRecognizer/"+DeviceInfo.PathModel);
                System.IO.Directory.Delete(directoryPath, true);
            }catch (DirectoryNotFoundException){}
            try{
                string directoryPath = System.IO.Path.GetFullPath("./RecognitionServer/AppRecognizer/HIF3D_LeapMotionDataFolder");
                System.IO.Directory.Delete(directoryPath,true);
             }catch (DirectoryNotFoundException){}
            try{
                string directoryPath = System.IO.Path.GetFullPath("./RecognitionServer/AppRecognizer/HIF3D_KinectDataFolder");
                System.IO.Directory.Delete(directoryPath,true);
            }catch (DirectoryNotFoundException){}
            StrategyAppRecognizer.NewClassesLearnt();
            StartAppRecognizer();
        }
     
      

        private void StartServer(string path)
        {
            
          
            string exePath = System.IO.Path.GetFullPath(path);

            _serverExe = new System.Diagnostics.Process();
            _serverExe.StartInfo.WorkingDirectory = exePath;
            _serverExe.StartInfo.FileName = "RecognitionServer.vs2015.exe";
            _serverExe.StartInfo.WindowStyle = (ShowServerCmd ? System.Diagnostics.ProcessWindowStyle.Normal : System.Diagnostics.ProcessWindowStyle.Hidden);
            _serverExe.StartInfo.Arguments = DeviceInfo.StartArguments;
            _serverExe.Start();
        }

        void StartRecoClient()
        {
            if(_recoClient!=null)
                _recoClient.Disconnect();
            else
            {
                _lastReceived = DateTime.Now;
                LastStatus = StatusReco.NotWorkingAndTrying;
                StartCoroutine( CheckConnectionToServer());
            }
            _recoClient = new RecognitionClientC("127.0.0.1", 65432);
            _recoClient.OnError += RecoClient_OnError;
            _recoClient.OnFrameRecognitionResult += RecoClient_OnFrameRecognitionResultStatic;
            _recoClient.Connect();
          
        }

        private IEnumerator CheckConnectionToServer()
        {
            while (true)
            {
                if (_pause)
                {
                    //do nothing
                }
                else if ((DateTime.Now - _lastReceived).TotalSeconds >TIME_TO_RETRY_CONNECT_AFTER_LAST_RECEIVED)
                {
                /*    if (LastStatus != StatusReco.NotWorkingAndTrying)
                    {*/
                        LastStatus = StatusReco.NotWorkingAndTrying;
                        if (notifier.GetStatusRecoChangeAction() != null)
                        {
                            notifier.GetStatusRecoChangeAction()(LastStatus);
                        }
                  //  }
                    if (State != StateCurrentReco.NotWorking)
                        StartRecoClient();
                }
                else
                {
                   /* if (LastStatus != StatusReco.Work)
                    {*/
                        LastStatus = StatusReco.Work;
                        if (notifier.GetStatusRecoChangeAction() != null)
                        {
                            notifier.GetStatusRecoChangeAction()(LastStatus);
                        }
                    //}
                }
                yield return new WaitForSeconds(2f);//check every 2 seconds
            }
            
        }

        public void StopRecognizer()
        {
            if (State != StateCurrentReco.NotWorking)
            {
                State = StateCurrentReco.NotWorking;
                _recoClient.Disconnect();
                Debug("Client disconnected");

                if (_serverExe != null && !_serverExe.HasExited)
                {
                    _serverExe.Kill();
                }        
            }
          
        }
        
        void OnApplicationQuit()
        {
            //Destroy(FindObjectOfType<LeapHandController>().gameObject);
            StopRecognizer();
            StopCoroutine(CheckConnectionToServer());
            StopCoroutine(SendLeapInfo());
            
        }

        /*void CheckConnection()
        {
            bool _newOnline = true;
            if (leapSP.GetLeapController() == null)
            {
                _newOnline = false;
                Debug("Leap Controller Not Created");
            }
            else if (!leapSP.IsConnected())
            {
                _newOnline = false;
                Debug("Hand Provider not connected");
            }

            if (online && !_newOnline)
                Debug("Now DISconnected");
            else if (!online && _newOnline)
                Debug("Now Connected");

            online = _newOnline;
        }*/

        void Update()
        {

          

       

        }
        
        private IEnumerator SendLeapInfo()
        {
            while (true)
            {
                if (State!=StateCurrentReco.NotWorking && !_pause)
                {
                    // 2 (gauche / droit) * 23 (Ordre) * 3 (x,y,z) = 138 valeurs doubles en entrée
                    DetectAndFillPositionDevice(ref _frame);
                    SendOneFrame();
                }
                yield return new WaitForSecondsRealtime(FREQUENCY_RECORD_SEND_DATA);
            }
        }

        public void DetectAndFillPositionDevice(ref Double[] frame)
        {
            DeviceInfo.Filler.Fill(ref frame);
        }
     

        private void SendOneFrame()
        {
            string id = "" + _nbFrame;
            _recoClient.Send(id, _frame);
            _nbFrame++;
        }

        /// <summary>
        /// Demande au moteur la réinitialisation de la distance curviligne (même effet que perte de tracking du leap)
        /// </summary>
        public void ResetCurDi()
        {
            _frame = new double[DeviceInfo.FrameSize];
            SendOneFrame();
        }

        private static void RecoClient_OnFrameRecognitionResultStatic(string id, System.Collections.Generic.Dictionary<string, double> allMap, bool scoreCanBeUsed)
        {
            RecoManager.GetInstance().RecoClient_OnFrameRecognitionResult(id, allMap, scoreCanBeUsed);
        }

        public void RecoClient_OnFrameRecognitionResult(string id, System.Collections.Generic.Dictionary<string, double> allMap, bool scoreCanBeUsed)
        {
            _lastReceived = DateTime.Now;
            ResultStrategy res;
            if (State == StateCurrentReco.AppWorking)
            {
                if (StrategyAppRecognizer == null)
                {
                    UnityEngine.Debug.LogWarning("Strategy is null, no result sent");
                    return;
                }
                res = StrategyAppRecognizer.OnFrameRecognitionResult(allMap);
            }
            else//should be MenuWorking
            {
                if (StrategyMenuRecognizer == null)
                {
                    UnityEngine.Debug.LogWarning("Strategy is null, no result sent");
                    return;
                }
                res = StrategyMenuRecognizer.OnFrameRecognitionResult(allMap);       
            }
        
            if (res.DecisionMade && notifier.GetGestureRecognizedAction() != null)
                notifier.GetGestureRecognizedAction()(res.CurrentRecognizedClass);
            if (notifier.GetClassDetailedScoresAction() != null)
                notifier.GetClassDetailedScoresAction()(res);    
        }

        private static void RecoClient_OnError(string errorMessage)
        {
            UnityEngine.Debug.Log("Erreur de connexion");
        }


        public void Pause(bool b)
        {
            this._pause = b;
        }
    }
}
