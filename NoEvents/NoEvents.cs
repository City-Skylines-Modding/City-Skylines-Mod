using System;
using System.Collections;
using System.Collections.Generic;

using ColossalFramework;
using ICities;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace NoEvents.Source
{
    public class NoEventsInformation : IUserMod
    {
        public string Name => "No Events";
        public string Description => "Disables police, health care, garbage collection " +
                "and fire department events.";
    }

    public class NoEventsLoader : LoadingExtensionBase
    {
        public NoEvents noEvents;

        /**
         * @brief
         * Overrides the default behaviour of the OnLevelLoaded Function.
         * Allows the mod to run when level is created or loaded.
         * @param mode Mode in which the game is accessed.
         */
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                noEvents = new NoEvents();
            }

            base.OnLevelLoaded(mode);
        }

        /**
         * @brief
         * Overrides the default behaviour of the OnLevelUnloading Function.
         * Deletes the Mod object.
         */
        public override void OnLevelUnloading()
        {
            if (noEvents != null)
            {
                noEvents = null;
            }

            base.OnLevelUnloading();
        }
    }

    public class NoEvents : ThreadingExtensionBase
    {
        /**
         * @brief
         * Overrides the default behaviour of the OnUpdate Funcion associated
         * with the ThreadingExtensionBase class.
         * @param realTimeDelta The time since the last update.
         * @param simulationTimeDelta The time since the last simulation update.
         */
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            DisableEvents();
            base.OnUpdate(realTimeDelta, simulationTimeDelta);
        }

        /**
         * @brief
         * Disables the crime, healthcare, garbage collection and fire department
         * events from happning in the city.
         * @return Returns if the events instance size is empty
         */
        public static void DisableEvents()
        {
            BuildingManager buildingInstance = Singleton<BuildingManager>.instance;
            var buildingCount = buildingInstance.m_buildings.m_size;

            if (buildingCount == 0)
            {
                return;
            }

            for (int i = 0; i < buildingCount; ++i)
            { 
                if (buildingInstance.m_buildings.m_buffer[i].m_flags == Building.Flags.None)
                {
                    continue;
                }

                buildingInstance.m_buildings.m_buffer[i].m_crimeBuffer = 0;

                buildingInstance.m_buildings.m_buffer[i].m_healthProblemTimer = 0;
                buildingInstance.m_buildings.m_buffer[i].m_health = 0;
                buildingInstance.m_buildings.m_buffer[i].m_childHealth = 0;
                buildingInstance.m_buildings.m_buffer[i].m_seniorHealth = 0;

                buildingInstance.m_buildings.m_buffer[i].m_fireIntensity = 0;
                buildingInstance.m_buildings.m_buffer[i].m_fireHazard = 0;

                buildingInstance.m_buildings.m_buffer[i].m_garbageBuffer = 0;
                buildingInstance.m_buildings.m_buffer[i].m_garbageTrafficRate = 0;

                buildingInstance.m_buildings.m_buffer[i].m_deathProblemTimer = 0;
            }
        }
    }
} 