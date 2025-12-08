using UnityEngine;

namespace Recognizer
{
    //Just to be detected as provider
    public class SimulatorProvider : IDeviceProvider
    {
        public override Vector3 CameraPosRelativ { get; } = new Vector3(7,3,-9);
    }
}