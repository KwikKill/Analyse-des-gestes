using System;
using AppTrans.Scripts.Recognizer.Simulator;
using UnityEngine;

namespace Recognizer.DeviceInfoTaker
{
    public class SimulatorInfoTaker : IDeviceInfoTaker
    {
        private SimulatorLeapMotion _manager;
        public SimulatorInfoTaker()
        {
            this._manager = SimulatorLeapMotion.GetInstance();
        }
        

        public void Fill(ref double[] data)
        {
            if (_manager==null)
            {
                if (SimulatorLeapMotion.GetInstance() == null)
                    return;
                _manager = SimulatorLeapMotion.GetInstance();
            }

            int startId = 0;
            foreach (Vector3 currentPosition in _manager.currentPositions)
            {
                data[startId] = currentPosition.x;
                data[startId+1] = currentPosition.y;
                data[startId+2] = currentPosition.z;
                startId += 3;
            }
            
        } 
        
        
        static private void SetFramePosition(ref double[] leapData, Vector3 v, ref int startIdx)
        {
            leapData[startIdx] = v.x;
            leapData[startIdx + 1] = v.y;
            leapData[startIdx + 2] = v.z;
            startIdx += 3;
        }
    }
}