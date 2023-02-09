using ColossalFramework;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;


namespace NoTraffic.Source
{
    public class NoTrafficInformation : IUserMod
    {
        /**
         * @brief
         * Shows the name of the mod in the mod list.
         * @return Name Mod's Name
         */
        public string Name => "Stop Traffic Generation";

        /**
         * @brief
         * Shows the mod's description in the City Skylines Content Manager
         * Window.
         * @return Description Mod's Description
         */
        public string Description => "Stops the generation of traffic in the game when the \'T\' key is pressed.";

        public void OnEnabled()
        {
            var go = new GameObject("NoTraffic");
            go.AddComponent<NoTraffic>();

            Object.DontDestroyOnLoad(go);
        }
    }

    public class NoTraffic : MonoBehaviour
    {
        private void Start()
        {
            var vehicles = Singleton<VehicleManager>.instance.m_vehicles.m_buffer;
            var vehicleCount = Singleton<VehicleManager>.instance.m_vehicles.m_size;

            for (ushort i = 0; i < vehicleCount; i++)
            {
                vehicles[i].Info.m_vehicleAI.SetVehicleName(i, ref vehicles[i], "NoTraffic");
                vehicles[i].Info.m_vehicleAI = null;

                vehicles[i].m_flags = Vehicle.Flags.Deleted;
            }

            VehicleManager.instance.m_vehicles.m_size = 0;
        }
    }
}
