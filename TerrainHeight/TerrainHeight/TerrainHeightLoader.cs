/**
 * @file TerrainHeightLoader.cs
 * @brief Terrain height loader class. Loads the mod.
 * @author Carlos Salguero
 * @date 2023-08-28
 * @version 4.0.0
 * 
 * @copyright Copyright (c) - Tec de Monterrey
 * 
 */
using ICities;
using ColossalFramework;


namespace TerrainHeight.Source
{
    /**
     * @class TerrainHeightLoader
     * @implements LoadingExtensionBase
     * @brief Terrain height loader class. Loads the mod.
     */
    public class TerrainHeightLoader : LoadingExtensionBase
    {
        private TerrainHeight terrainHeight;
        public static TerrainHeightLoader Instance { get; private set; }

        // Constructor
        /**
         * @brief
         * Construct a new TerrainHeightLoader :: TerrainHeightLoader object
         */
        public TerrainHeightLoader()
        {
            if (Instance == null)
                Instance = this;
        }

        // Methods
        /**
         * @brief
         * Overload of the onLevelLoaded method. It is called when the level is loaded 
         * or a new one is created.
         * @param mode LoadMode mode of the load
         */
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame
                || mode == LoadMode.NewGameFromScenario || mode == LoadMode.LoadScenario)
                terrainHeight = new TerrainHeight();

            base.OnLevelLoaded(mode);
        }

        /**
         * @brief
         * Overload the on level unloading method. It is called when the level is unloaded.
         */
        public override void OnLevelUnloading()
        {
            if (terrainHeight != null)
            {
                terrainHeight.Dispose();
                terrainHeight = null;
            }

            base.OnLevelUnloading();
        }
    }
}
