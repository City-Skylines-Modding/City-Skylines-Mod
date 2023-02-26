using System;
using ICities;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;
using System.Collections.Generic;

namespace TerrainHeight.Source
{
    public class TerrainHeightInformation : IUserMod
    {
        public string Name => "Terrain Height";
        public string Description => "Gets the terrain height in meters.";
    }

    public class TerrainHeightLoader : LoadingExtensionBase
    {
        private TerrainHeight terrainHeight;

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
                terrainHeight = new TerrainHeight();
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
            if (terrainHeight != null)
            {
                terrainHeight = null;
            }

            base.OnLevelUnloading();
        }


    }

    public class TerrainHeight : ThreadingExtensionBase
    {
        private TerrainManager terrainManager;
        private UILabel terrainHeightLabel;
        private UILabel terrainHeightValue;
        private UILabel terrainHeightUnit;
        private float terrainHeight;

        /**
         * @brief
         * Overrides the default behaviour of the OnCreated function.
         * Creates the mod when the game is loaded or created.
         */
        public override void OnCreated(IThreading threading)
        {
            terrainManager = Singleton<TerrainManager>.instance;

            terrainHeightLabel = UIView.GetAView().AddUIComponent(typeof(UILabel)) as UILabel;
            terrainHeightLabel.text = "Terrain Height";
            terrainHeightLabel.relativePosition = new Vector3(10, 10);
            terrainHeightLabel.textScale = 0.8f;

            terrainHeightValue = UIView.GetAView().AddUIComponent(typeof(UILabel)) as UILabel;
            terrainHeightValue.relativePosition = new Vector3(10, 30);
            terrainHeightValue.textScale = 0.8f;

            terrainHeightUnit = UIView.GetAView().AddUIComponent(typeof(UILabel)) as UILabel;
            terrainHeightUnit.text = "m";
            terrainHeightUnit.relativePosition = new Vector3(10, 50);
            terrainHeightUnit.textScale = 0.8f;

            base.OnCreated(threading);
        }

        /**
         * @brief
         * Overrides the default behaviour of the OnReleased function.
         * Deletes the mod when the game is closed.
         */
        public override void OnReleased()
        {
            if (terrainHeightLabel != null)
            {
                UnityEngine.Object.Destroy(terrainHeightLabel);
            }

            if (terrainHeightValue != null)
            {
                UnityEngine.Object.Destroy(terrainHeightValue);
            }

            if (terrainHeightUnit != null)
            {
                UnityEngine.Object.Destroy(terrainHeightUnit);
            }

            base.OnReleased();
        }

        /**
         * @brief
         * Overrides the default behaviour of the OnUpdate function.
         * Updates the mod when the game is loaded or created.
         */
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            terrainHeight = terrainManager.GetSurfaceHeight();
            

            base.OnUpdate(realTimeDelta, simulationTimeDelta);
        }
    }
}