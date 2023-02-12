// Stop Vehicle generation in city skylines
using UnityEngine;
using ColossalFramework;
using ICities;

namespace NoTraffic.Source
{
    public class NoTrafficInformation : IUserMod
    {
        public string Name => "No Traffic";
        public string Description => "Stop Vehicle generation in city skylines";
    }

    public class NoTrafficLoader : LoadingExtensionBase
    {
        public NoTraffic noTraffic;

        /**
         * @brief
         * Overrides the default behaviour of the LoadingExtensionBase function.
         * Allows the mod to be accessed when a game is loaded or created.
         * @param mode Mode in which the game is accessed.
         */
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                noTraffic = new NoTraffic();
            }

            base.OnLevelLoaded(mode);
        }

        /**
         * @brief
         * Overrides the default behaviour of the OnLevelUnloading function.
         * Deletes the mod when the game is closed.
         */
        public override void OnLevelUnloading()
        {
            noTraffic = null;
            base.OnLevelUnloading();
        }
    }

    public class NoTraffic : ThreadingExtensionBase
    {
        /**
         * @brief
         * Overrides the default behavious of the OnUpade Function associated 
         * with the ThreadingExtensionBase class.
         * @param realTimeDelta The time since the last update.
         * @param simulationTimeDelta The time since the last simulation update.
         */
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            DestroyVehicles();
            base.OnUpdate(realTimeDelta, simulationTimeDelta);
        }
        
        /**
         * @brief
         * Gets the instance of the VehicleManager and the number of vehicles in the game.
         * Then, it loops through all the vehicles and sets their flags to 0.
         * @return Returns if the vehicle array size is 0.
         */
        private void DestroyVehicles()
        {
            VehicleManager vehicleInstance = Singleton<VehicleManager>.instance;
            uint numVehicles = vehicleInstance.m_vehicles.m_size;

            if (numVehicles == 0)
            {
                return;
            }

            for (int i = 0; i < numVehicles; i++)
            {
                Vehicle vehicles = vehicleInstance.m_vehicles.m_buffer[i];

                // If array is empty
                if (vehicleInstance.m_vehicles.m_buffer[i].m_flags == 0)
                {
                    continue;
                }

                if (i >= numVehicles)
                {
                    break;
                }

                // Set flags to 0 and remove vehicle
                vehicleInstance.m_vehicles.m_buffer[i].m_flags = 0;
                vehicleInstance.m_vehicles.m_buffer[i].m_flags = Vehicle.Flags.WaitingPath;
            }
        }
    }
}