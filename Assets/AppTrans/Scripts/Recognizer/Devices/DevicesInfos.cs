using System.Collections.Generic;
using Recognizer.DeviceInfoTaker;
using UnityEngine;

namespace Recognizer
{
    public class DevicesInfos
    {
        public enum Device
        {
            None, 
            LeapMotion,
            SimulorLeapMotion,
        }
        
        public class DeviceInfo
        {
            public Device Device;
            public IDeviceInfoTaker Filler;
            public int FrameSize;
            public string PathModel;
            public string PathRaw;
            public string StartArguments;

            public DeviceInfo( Device device, IDeviceInfoTaker filler,int frameSize, string pathModel, string pathRaw, string startArguments)
            {
                Device = device;
                Filler = filler;
                FrameSize = frameSize;
                PathModel = pathModel;
                PathRaw = pathRaw;
                StartArguments = startArguments;
            }
        }

        public static Dictionary<Device,DeviceInfo> Infos = new Dictionary<Device, DeviceInfo>()
        {
            {Device.None,new DeviceInfo(Device.None,null, 0,"","","")},
            {Device.LeapMotion,new DeviceInfo(Device.LeapMotion,new LeapInfoTaker(), 23*2*3,"ModelFixed_LeapMotionFolder","RAW_LeapMotionDataFolder","0")},
            {Device.SimulorLeapMotion,new DeviceInfo(Device.SimulorLeapMotion,new SimulatorInfoTaker(), 23*2*3,"ModelFixed_LeapMotionFolder","RAW_LeapMotionDataFolder","0")},

        };
    }
}