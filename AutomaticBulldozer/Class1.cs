using System;
using System.Collections;
using System.Collections.Generic;

using ColossalFramework;
using ICities;


namespace AutomaticBulldoze.Source
{
    public class AutomaticBulldozeInformation : IUserMod
    {
        /**
         * @brief
         * Fills the information about the mod.
         * @return The name of the mod
         */
        public string Name
        {
            get { return "Automatic Bulldoze"; }
        }

        /**
         * @brief
         * Fills the description about the mod.
         * @return The description of the mod
         */
        public string Description
        {
            get { return "Automatically bulldoze abandoned buildings"; }
        }
    }

    public class AutomaticBulldozeLoader : LoadingExtensionBase
    {
        private AutomaticBulldozer _automaticBulldozer;

        /**
         * @brief
         * Overrides the default behaviour of the OnLevelLoaded function. 
         * Creates an instance of the Automatic Bulldozer.
         * @param mode Mode in which the level is accesed (created or loaded)
         * @return void
         */
        public override void OnLevelLoaded(LoadMode mode)
        {
            _automaticBulldozer = new AutomaticBulldozer();

            base.OnLevelLoaded(mode);
        }

        /**
         * @brief
         * Overrides the default behaviour of the OnLevelUnloaded function. 
         * Deletes the created instance of the Automatic Bulldozer created
         * when the level is loaded.
         * @return void
         */
        public override void OnLevelUnloading()
        {
            if (_automaticBulldozer != null)
            {
                _automaticBulldozer.Destroy();
                _automaticBulldozer = null;
            }

            base.OnLevelUnloading();
        }
    }

    public class AutomaticBulldozer
    {
        // Private read only attributes
        private readonly BuildingManager _buildingManager;
        private readonly BuildingObjectObserver _buildingObjectObserver;
        private readonly SimulationManager _simulationManager;

        /**
         * @brief
         * Construct a new instance of the Automatic Bulldozer.
         */
        public AutomaticBulldozer()
        {
            _buildingManager = Singleton<BuildingManager>.instance;
            _simulationManager = Singleton<SimulationManager>.instance;

            _buildingObjectObserver = new BuildingObjectObserver();
            _buildingObjectObserver.BuildingID.UnionWith(FindExistingBuildings());

            BindEvents();
        }

        /**
         * @brief
         * Binds the simulation event to the OnSimulationTick function.
         * @return void
         */
        private void BindEvents()
        {
            Timer.SimluationTicks += OnSimulationTick;
        }

        /**
         * @brief
         * Handles the simulation tick event. Finds the buildings with
         * "abandoned" associated to it. 
         * @param source The source of the event
         * @param args The arguments of the event
         * @return void
         */
        private void OnSimulationTick(object source, EventArgs args)
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
        public void DestroyBuildings(IEnumerable<ushort> abandonedBuildings)
        {
            foreach (var ID in abandonedBuildings)
            {
                if (_buildingObjectObserver.BuildingID.Contains(ID))
                {
                    _simulationManager.AddAction(BulldozeBuildings(ID));
                    _buildingObjectObserver.BuildingID.Remove(ID);
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
        private IEnumerator BulldozeBuildings(ushort ID)
        {
            if (_buildingManager.m_buildings.m_buffer[ID].m_flags != 0)
            {
                var information = _buildingManager.m_buildings.m_buffer[ID].Info;

                if (information.m_buildingAI.CheckBulldozing(
                    ID, ref _buildingManager.m_buildings.m_buffer[ID]) == ToolBase.ToolErrors.None)
                    _buildingManager.ReleaseBuilding(ID);
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
            var buildingID = new HashSet<ushort>(_buildingObjectObserver.BuildingID);
            var abandoned = new HashSet<ushort>();

            foreach (var ID in buildingID)
            {
                var building = _buildingManager.m_buildings.m_buffer[ID];

                if (building.m_flags == Building.Flags.Abandoned)
                    abandoned.Add(ID);
            }

            return abandoned;
        }

        /**
         * @brief
         * Handles the search of existing buildings in the game.
         * @return buildingID A HashSet (ushort type) with all the buildings's ID.
         */
        private HashSet<ushort> FindExistingBuildings()
        {
            var buildingID = new HashSet<ushort>();

            for (var i = 0; i < _buildingManager.m_buildings.m_buffer.Length; i++)
            {
                if (_buildingManager.m_buildings.m_buffer[i].m_flags != Building.Flags.None
                    && !Building.Flags.Original.IsFlagSet(_buildingManager.m_buildings.m_buffer[i].m_flags))
                    buildingID.Add((ushort)i);
            }

            return buildingID;
        }


        /**
         * @brief
         * Destroys the instance of the Automatic Bulldozer.
         * @return void
         */
        public void Destroy()
        {
            Timer.SimluationTicks -= OnSimulationTick;
            _buildingObjectObserver.Destroy();
        }

    }

    public class BuildingObjectObserver
    {
        // private readonly attributes
        private readonly BuildingManager _buildingManager;

        // public attributes
        public HashSet<ushort> BuildingID { get; private set; }

        /**
         * @brief
         * Construct a new instance of the Building Object Observer.
         */
        public BuildingObjectObserver()
        {
            BuildingID = new HashSet<ushort>();
            _buildingManager = Singleton<BuildingManager>.instance;

            BindEvents();
        }

        /**
         * @brief
         * Binds the building event to the OnBuildingCreated function.
         * @return void
         */
        private void BindEvents()
        {
            _buildingManager.EventBuildingCreated += OnBuildingCreated;
            _buildingManager.EventBuildingReleased += OnBuildingReleased;
        }

        /**
         * @brief
         * Handles the building created event. Adds the building ID to the
         * building.
         * @param id The ID of the building
         * @return void
         */
        public void OnBuildingCreated(ushort id)
        {
            BuildingID.Add(id);
        }

        /**
         * Handles the building released event. Removes the building ID from the
         * building.
         * @param id The ID of the building
         * @return void
         */
        public void OnBuildingReleased(ushort id)
        {
            BuildingID.Remove(id);
        }

        /**
         * @brief
         * Destroys the instance of the Building Object Observer.
         * @return void
         */
        public void Destroy()
        {
            _buildingManager.EventBuildingCreated -= OnBuildingCreated;
            _buildingManager.EventBuildingReleased -= OnBuildingReleased;
        }
    }

    public class Timer : ThreadingExtensionBase
    {

        public delegate void SimulationTickEventHandler(object source, EventArgs args);
        public static event SimulationTickEventHandler SimluationTicks;

        private float _counter;

        /**
         * @brief
         * Overrides the default behaviour of the OnUpdate function
         * @param realTimeDelta The real time delta
         * @param simulationTimeDelta The simulation time delta
         */
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (_counter >= 1)
            {
                if (SimluationTicks != null)
                    SimluationTicks(this, EventArgs.Empty);
            }

            else
            {
                _counter += simulationTimeDelta;
            }

            base.OnUpdate(realTimeDelta, simulationTimeDelta);
        }
    }
}