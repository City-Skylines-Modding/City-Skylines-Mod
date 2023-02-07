using System;
using System.Collections;
using System.Collections.Generic;

using ColossalFramework;
using ICities;

namespace AutomaticBulldoze.Source
{
    public class AutomaticBulldozeInfo : IUserMod
    {
        public string Name
        {
            get { return "Automatic Bulldozer"; }
        }

        public string Description
        {
            get { return "Automatic bulldozing for abandoned buildings."; }
        }
    }

    public class AutomaticBulldozerLoader : LoadingExtensionBase
    {
        private AutomaticBulldozer automaticBulldozer;
        
        /**
         * @brief
         * Overrides the default behaviour of the OnLevelLoaded Function.
         * Creates an instance of the automatic Bulldozer.
         * @param mode Mode in which the game is accessed. 
         */
        public override void OnLevelLoaded(LoadMode mode)
        {
            automaticBulldozer = new AutomaticBulldozer();

            base.OnLevelLoaded(mode);
        }

        /**
         * @brief
         * Overrides the default behaviour of the OnLevelUnloading() Function
         * Deletes the created instance of the Bulldozer.
         */
        public override void OnLevelUnloading()
        {
            if (automaticBulldozer != null)
            {
                automaticBulldozer.Dispose();
                automaticBulldozer = null;
            }

            base.OnLevelUnloading();
        }
    }

    public class AutomaticBulldozer
    {
        private readonly BuildingManager buildingManager;
        private readonly BuildingObjectObserver buildingObserver;
        private readonly SimulationManager simulationManager;

        /**
         * @brief
         * Construct a new instance of the Automatic Bulldozer.
         */
        public AutomaticBulldozer()
        {
            buildingManager = Singleton<BuildingManager>.instance;
            simulationManager = Singleton<SimulationManager>.instance;

            buildingObserver = new BuildingObjectObserver();
            buildingObserver.buildingID.UnionWith(FindExistingBuildings());

            BindEvents();
        }

        /**
         * @brief
         * Binds the simulation event to the OnSimulationTick function.
         * @return void
         */
        private void BindEvents()
        {
            Timer.SimulationStep += OnSimulationStep;
        }

        /**
         * @brief
         * Handles the simulation tick event. Finds the buildings with
         * "abandoned" associated to it. 
         * @param source The source of the event
         * @param args The arguments of the event
         * @return void
         */
        private void OnSimulationStep(object source, EventArgs args)
        {
            var abandonedBuildings = FindAbandonedBuildings();

            if (abandonedBuildings.Count > 0)
                DestroyBuildings(abandonedBuildings);
        }

        /**
         * @brief
         * Destroys the instance of buildings with "abandoned" associated to it.
         * @param abandonedBuildings The list of buildings with "abandoned" associated to it
         * @return void
         */
        private void DestroyBuildings(IEnumerable<ushort> abandonedBuildings)
        {
            foreach (var ID in abandonedBuildings)
            {
                if (buildingObserver.buildingID.Contains(ID))
                {
                    simulationManager.AddAction(BulldozeBuildings(ID));
                    buildingObserver.buildingID.Remove(ID);
                }
            }
        }

        /**
         * @brief
         * Handles the destruction of the buildings with "abandoned" associated to it.
         * Basically, activates the automatic bulldozer.
         * @param ID The ID of the building to be destroyed
         * @return iterator
         */
        public IEnumerator BulldozeBuildings(ushort buildingID)
        {
            if (buildingManager.m_buildings.m_buffer[buildingID].m_flags != 0)
            {
                var information = buildingManager.m_buildings.m_buffer[buildingID].Info;

                if (information.m_buildingAI.CheckBulldozing(buildingID, ref buildingManager.m_buildings.m_buffer[buildingID]) == 
                    ToolBase.ToolErrors.None)
                    buildingManager.ReleaseBuilding(buildingID);
            }

            yield return (object)0;
        }

        /**
         * @brief
         * Handles the search of buildings with "abandoned" associated to it.
         * @return abandoned HashSet (ushort type) with all the abandoned buildings's ID.
         */
        private HashSet<ushort> FindAbandonedBuildings()
        {
            var buildingIds = new HashSet<ushort>(buildingObserver.buildingID);
            var abandonedBuildings = new HashSet<ushort>();

            foreach (var buildingId in buildingIds)
            {
                var building = buildingManager.m_buildings.m_buffer[buildingId];

                if (building.m_flags.IsFlagSet(Building.Flags.Abandoned))
                    abandonedBuildings.Add(buildingId);
            }

            return abandonedBuildings;
        }

        /**
         * @brief
         * Handles the search of existing buildings in the game.
         * @return buildingID A HashSet (ushort type) with all the buildings's ID.
         */
        private HashSet<ushort> FindExistingBuildings()
        {
            var buildingIds = new HashSet<ushort>();

            for (var i = 0; i < buildingManager.m_buildings.m_buffer.Length; i++)
                if (buildingManager.m_buildings.m_buffer[i].m_flags != Building.Flags.None && 
                    !Building.Flags.Original.IsFlagSet(buildingManager.m_buildings.m_buffer[i].m_flags))
                    buildingIds.Add((ushort)i);

            return buildingIds;
        }

        /**
         * @brief
         * Destroys the instance of the Automatic Bulldozer.
         * @return void
         */
        public void Dispose()
        {
            Timer.SimulationStep -= OnSimulationStep;

            buildingObserver.Destroy();
        }
    }

    public class BuildingObjectObserver
    {
        private readonly BuildingManager buildingManager;
        public HashSet<ushort> buildingID { get; private set; }

        /**
         * @brief
         * Construct a new instance of the Building Object Observer.
         */
        public BuildingObjectObserver()
        {
            buildingID = new HashSet<ushort>();
            buildingManager = Singleton<BuildingManager>.instance;

            BindEvents();
        }

        /**
         * @brief
         * Binds the building event to the OnBuildingCreated function.
         * @return void
         */
        public void BindEvents()
        {
            buildingManager.EventBuildingCreated += OnBuildingCreated;
            buildingManager.EventBuildingReleased += OnBuildingReleased;
        }

        /**
         * @brief
         * Handles the building created event. Adds the building ID to the
         * building.
         * @param id The ID of the building
         * @return void
         */
        private void OnBuildingCreated(ushort ID)
        {
            buildingID.Add(ID);
        }

        /**
         * Handles the building released event. Removes the building ID from the
         * building.
         * @param id The ID of the building
         * @return void
         */
        private void OnBuildingReleased(ushort ID)
        {
            buildingID.Remove(ID);
        }

        /**
         * @brief
         * Destroys the instance of the Building Object Observer.
         * @return void
         */
        public void Destroy()
        {
            buildingManager.EventBuildingCreated -= OnBuildingCreated;
            buildingManager.EventBuildingReleased -= OnBuildingReleased;
        }

    }

    public class Timer : ThreadingExtensionBase
    {
        public delegate void SimulationStepEventHandler(object source, EventArgs args);
        public static event SimulationStepEventHandler SimulationStep;
        private float counter;

        /**
         * @brief
         * Overrides the default behaviour of the OnUpdate function
         * @param realTimeDelta The real time delta
         * @param simulationTimeDelta The simulation time delta
         */
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (counter >= 1)
            {
                if (SimulationStep != null)
                    SimulationStep(this, EventArgs.Empty);

                counter = 0;
            }

            else
            {
                counter += simulationTimeDelta;
            }

            base.OnUpdate(realTimeDelta, simulationTimeDelta);
        }
    }
}