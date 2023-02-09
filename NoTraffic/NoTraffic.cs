// Stop Vehicle generation in city skylines
using UnityEngine;
using ColossalFramework;
using ICities;
using System;

namespace NoTraffic.Source
{
    public class NoTrafficInformation : IUserMod
    {
        public string Name => "No Traffic";
        public string Description => "Stop Vehicle generation in city skylines";
    }

    public class NoTraffic : ThreadingExtensionBase
    {
        /**
         * @brief
         * Gets the instance of the VehicleManager and the number of vehicles in the
         * game. Then it loops through all the vehicles and sets their flags to 0
         * @param realTimeDelta The time since the last update.
         * @param simulationTimeDelta The time since the last simulation update.
         */
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            uint numVehicles = instance.m_vehicles.m_size;

            for (int i = 0; i < numVehicles; i++)
            {
                Vehicle vehicle = instance.m_vehicles.m_buffer[i];

                if (vehicle.m_flags != Vehicle.Flags.Spawned)
                {
                    instance.ReleaseVehicle((ushort)i);
                }
            }

            if (numVehicles == 0)
            {
                numVehicles = (uint)instance.m_vehicles.m_buffer.Length;

                for (int i = 0; i < numVehicles; i++)
                {
                    Vehicle vehicle = instance.m_vehicles.m_buffer[i];

                    if (vehicle.m_flags != Vehicle.Flags.Spawned)
                    {
                        instance.ReleaseVehicle((ushort)i);
                    }
                }
            }
        }
    }
}