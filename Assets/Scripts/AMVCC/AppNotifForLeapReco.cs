using System;
using Recognizer;
using Recognizer.StrategyFusion;

namespace AMVCC
{
    public class AppNotifForLeapReco : AppNotificationForLeapReco
    {
        public static Action<string> MotorInput;

        public override Action<string> GetGestureRecognizedAction()
        {
            return MotorInput;
        }

        public override Action<ResultStrategy> GetClassDetailedScoresAction()
        {
            return null;
        }

        public override Action<RecoManager.StatusReco> GetStatusRecoChangeAction()
        {
            return null;
        }
    }
}