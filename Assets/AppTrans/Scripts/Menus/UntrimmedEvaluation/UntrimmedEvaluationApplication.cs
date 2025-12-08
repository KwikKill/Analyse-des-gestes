using System;
using Menus.ClassifierEvaluation.Model;
using Menus.ClassifierEvaluation.View;

namespace Menus.ClassifierEvaluation
{
    public class UntrimmedEvaluationElement : MenuElement<UntrimmedEvaluationApplication>
    {
    }
    public class UntrimmedEvaluationApplication: AppMenuItem
    {
        public Recognizer.DevicesInfos.Device _previousDevice;
        public Recognizer.RecoManager recoManager;

        [NonSerialized]
        public UntrimmedEvaluationModel model;
        public UntrimmedEvaluationView view;

        protected override void Awake()
        {
            base.Awake();

            Recognizer.RecoManager[] recos = FindObjectsOfType<Recognizer.RecoManager>();
            for (int i = 0; i< recos.Length; ++i )
            {
                if (recos[i].LeapProvider != null)
                {
                    recoManager = recos[i];
                    break;
                }
            }            

            _previousDevice = recoManager.Device;
            recoManager.ChangeDeviceTo(Recognizer.DevicesInfos.Device.SimulorLeapMotion);
            model =new UntrimmedEvaluationModel();
        }

        private void Start()
        {
            Draw();
        }

        private void OnDisable()
        {
            recoManager.ChangeDeviceTo(_previousDevice);
        }

        public override string GetTitle()
        {
            return "Évaluation non segmentée";
        }
    }
}