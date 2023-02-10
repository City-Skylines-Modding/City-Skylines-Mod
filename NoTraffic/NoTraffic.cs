﻿// Stop Vehicle generation in city skylines
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

                // If array is empty, skip
                if (vehicle.m_flags == 0)
                {
                    continue;
                }

                // If array index is out of bounds, skip
                if (i >= numVehicles)
                {
                    continue;
                }


                if (vehicle.m_flags == Vehicle.Flags.Created &&
                    vehicle.m_flags == Vehicle.Flags.Spawned)
                {
                    continue;
                }

                // Set the flags to 0
                instance.m_vehicles.m_buffer[i].m_flags = 0;

                // Set the vehicle to despawn
                instance.m_vehicles.m_buffer[i].m_flags = Vehicle.Flags.WaitingPath;
            }
        }
    }

    /** 
     * Stop the crime, fire, ambulance, police, and garbage trucks events from spawning
     */
    public class NoEvents : ThreadingExtensionBase
    {
        /**
         * @brief
         * Gets the instance of the EventManager and the number of events in the
         * game. Then it loops through all the events and sets their flags to 0
         * @param realTimeDelta The time since the last update.
         * @param simulationTimeDelta The time since the last simulation update.
         */
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            EventManager instance = Singleton<EventManager>.instance;
            int numEvents = instance.m_events.m_size;

            for (int i = 0; i < numEvents; i++)
            {
                EventData evt = instance.m_events.m_buffer[i];

                // If array is empty, skip
                if (evt.m_flags == 0)
                {
                    continue;
                }

                // If array index is out of bounds, skip
                if (i >= numEvents)
                {
                    continue;
                }

                // Set the flags to 0
                instance.m_events.m_buffer[i].m_flags = 0;
            }
        }
    }
}