using System;
using System.Threading;

using ICities;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;

namespace TerrainHeight.Source
{
    public class TerrainHeightInformation : IUserMod
    {
        public string Name => "Terrain Height Information";
        public string Description => "Displays the height of the terrain under the mouse cursor.";
    }

    public class TerrainHeightLoader : LoadingExtensionBase
    {
        private TerrainHeight terrainHeight;

        /**
         * @brief
         * Overrides the defualt behaviour of the OnLevelLoaded function.
         * Creates a new instance of the mod.
         * @param mode The mode the game is in.
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
         * Overrides the defualt behaviour of the OnLevelUnloading function.
         * Destroys the mod instance.
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

        /**
         * @brief
         * Gets the terrain height at the mouse position.
         * @return Terrain at the mouse position. 
         */
        private float GetTerrainHeight()
        {
            Vector3 mousePosition = Input.mousePosition;

            return Singleton<TerrainManager>.instance.SampleDetailHeight(mousePosition);
        }

        /**
         * @brief
         * Overrides the default behaviour of the OnUpdate Funcion.
         * @param realTimeDelta The time since the last update.
         * @param simulationTimeDelta The time since the last simulation update.
         */
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            OnKeysPressed();
            OnKeysReleased();

            base.OnUpdate(realTimeDelta, simulationTimeDelta);
        }

        /**
         * @brief
         * Checks if the keys Left Control + T are pressed.
         */
        private void OnKeysPressed()
        {
            // UI gets stuck when keys are pressed to fast.
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
            {
                ShowTerrainHeight();
            }

            base.OnUpdate(0f, 0f);
        }

        /**
         * @brief
         * Checks if the keys Left Control + T are released.
         */
        private void OnKeysReleased()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.T))
            {
                DeleteUIComponent();
            }

            if (Input.GetKeyUp(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.T))
            {
                DeleteUIComponent();
            }

            base.OnUpdate(0f, 0f);
        }

        /**
         * @brief
         * Creates a UI Component to display the terrain height.
         * The component can be accesed when the keys Left Control + T are pressed.
         */
        private void ShowTerrainHeight()
        {
            float terrainMeters = GetTerrainHeight() / 8f;

            UIView view = UIView.GetAView();
            UILabel label = view.AddUIComponent(typeof(UILabel)) as UILabel;

            label.name = "TerrainHeightLabel";
            label.text = "Terrain Height: " + terrainMeters.ToString("0.00") + "m";
            label.textScale = 0.8f;
            label.relativePosition = new Vector3(70f, 25f);
        }

        /**
         * @brief
         * Deletes the previous instance of the UI Component.
         */
        private void DeleteUIComponent()
        {
            UIView view = UIView.GetAView();
            UILabel label = view.FindUIComponent<UILabel>("TerrainHeightLabel");

            if (label != null)
            {
                UnityEngine.Object.Destroy(label);
            }
        }
    }
}