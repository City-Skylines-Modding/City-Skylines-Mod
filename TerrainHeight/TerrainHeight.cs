using System;
using System.Threading;
using ICities;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;

namespace TerrainHeight.Source
{
    /**
     * @class TerrainHeightInformation
     * @extends IUserMod
     * @brief This class is responsible for displaying the information about the mod in the content manager
     */
    public class TerrainHeightInformation : IUserMod
    {
        public string Name => "Terrain Height Mod";
        public string Description => "Displays the terrain height at the mouse cursor position" +
            ". After the following keys are pressed: ctrl + shift + t";
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

    /**
     * @class TerrainHeight
     * @extends ThreadingExtensionBase
     * @brief This class is responsible for displaying the terrain height at the mouse cursor position
     */
    public class TerrainHeight : ThreadingExtensionBase
    {
        private bool isUpdating = false;
        private UILabel uiLabel;

        /**
         * @brief
         * Disposes of the UI label.
         */
        public void Dispose()
        {
            if (uiLabel != null)
            {
                UnityEngine.Object.Destroy(uiLabel.gameObject);
                uiLabel = null;
            }
        }

        /**
         * @brief
         * Overrides the default behaviour of the OnUpdate function.
         */
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
            {
                isUpdating = !isUpdating;

                if (isUpdating)
                {
                    showTerrainHeight();
                }
                else
                {
                    hideTerrainHeight();
                }
            }

            if (isUpdating)
            {
                updateTerrainHeight();
            }

            base.OnUpdate(realTimeDelta, simulationTimeDelta);
        }

        /**
         * @brief
         * Shows the terrain height at the mouse cursor position.
         */
        private void showTerrainHeight()
        {
            if (uiLabel == null)
            {
                uiLabel = createUIComponent();
            }

            uiLabel.isVisible = true;
            updateTerrainHeight();
        }

        /**
         * @brief
         * Hides the terrain height at the mouse cursor position.
         */
        private void hideTerrainHeight()
        {
            if (uiLabel != null)
            {
                uiLabel.isVisible = false;
            }
        }

        /**
         * @brief
         * Creates the UI Component with the terrain height.
         * @return The UI Component with the terrain height.
         */
        private UILabel createUIComponent()
        {
            UIView uiView = UIView.GetAView();
            UILabel label = uiView.AddUIComponent(typeof(UILabel)) as UILabel;

            label.name = "TerrainHeightLabel";
            label.relativePosition = new Vector3(0, 0);
            label.textAlignment = UIHorizontalAlignment.Left;
            label.verticalAlignment = UIVerticalAlignment.Middle;
            label.textScale = 0.8f;
            label.padding = new RectOffset(60, 10, 10, 10);
            label.autoSize = true;
            label.wordWrap = true;
            label.autoHeight = true;
            label.autoSize = true;
            label.backgroundSprite = "GenericPanel";
            label.color = new Color32(255, 255, 255, 255);
            label.opacity = 0.8f;

            return label;
        }

        /**
         * @brief
         * Updates the terrain height at the mouse cursor position
         */
        private void updateTerrainHeight()
        {
            if (uiLabel != null)
            {
                Vector3 mousePosition = Input.mousePosition;
                float terrainHeight = TerrainManager.instance.SampleRawHeightSmooth(mousePosition) / 8f;
                uiLabel.text = "Terrain Height: " + terrainHeight.ToString("0.00") + "m";
            }
        }
    }
}