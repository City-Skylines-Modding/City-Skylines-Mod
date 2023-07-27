using System;
using ICities;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;

namespace WaterPainting.Source
{
    /**
     * @class WaterPaintingInformation
     * @extends IUserMod
     * @brief Displays the information for the mod.
     */
    public class WaterPaintingInformation : IUserMod
    {
        public string Name => "Water Painting Tool";
        public string Description => "Allows the user to paint with water when the following " +
            "keys are pressed: ctrl + shift + p";
    }

    /**
     * @class WaterPaintingLoader
     * @extends LoadingExtensionBase
     * @brief Loads the mod.
     */
    public class WaterPaintingLoader : LoadingExtensionBase
    {
        private WaterPaintingTool m_waterPaintingTool;

        /**
         * @brief 
         * Overrides the OnLevelLoaded method and sets the tool to the water painting tool.
         */
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                m_waterPaintingTool = new WaterPaintingTool();
            }

            base.OnLevelLoaded(mode);
        }
    }

    /**
     * @class WaterPaintingTool 
     * @extends TerrainTool
     * @brief Allows the user to paint with water by using the terrain landscape tool. 
     * It overrides this tool and adds the functionality to paint with water.
     */
    public class WaterPaintingTool : TerrainTool
    {
        private bool m_isPainting = false;
        private UITextureAtlas.SpriteInfo m_toolCursor;

        /**
         * @brief
         * Called when the user presses the control key, shift key, and p key.
         */
        protected override void OnEnable()
        {
            m_toolCursor = GetToolCursor();
            base.OnEnable();
        }

        /**
         * @brief 
         * Get the cursor sprint info
         */
        private UITextureAtlas.SpriteInfo GetToolCursor()
        {
            UITextureAtlas atlas = Resources.FindObjectsOfTypeAll<UITextureAtlas>()[0];
            return atlas["OptionBaseFocused"];
        }

        /**
         * @brief 
         * Overrides the default behaviour of the tool to handle tool updates 
         * (mouse movement)
         */
        protected override void OnToolUpdate()
        {
            HandleInput();

            if (!m_isPainting)
                ShowToolCursor(false);

            ShowToolCursor(true);
            base.OnToolUpdate();
        }

        /**
         * @brief
         * Overrides the default behaviour of the destruction tool to handle
         */
        protected override void OnDestroy()
        {
            m_isPainting = false;
            m_toolCursor = null;
            base.OnDestroy();
        }

        /**
         * @brief
         * Show and hide the tool cursor based on the input parameter
         * @param show Boolean that determines whether to show or hide the tool cursor
         */
        private void ShowToolCursor(bool show)
        {
            if (m_toolCursor != null)
            {
                CursorInfo cursorInfo = new CursorInfo
                {
                    m_texture = m_toolCursor.texture,
                    m_hotspot = m_toolCursor.region.size / 2,
                };

                if (show) 
                    Cursor.SetCursor(cursorInfo.m_texture, cursorInfo.m_hotspot, CursorMode.Auto);

                else 
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

        /**
         * @brief
         * Handles the input from the user. If the user presses the control key, shift key, and p key
         * then the tool will start painting with water. If the user releases the keys then the tool
         * will stop painting with water.
         */
        private void HandleInput()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift)
                && Input.GetKeyDown(KeyCode.P))
                m_isPainting = !m_isPainting;
        }

        /**
         * @brief
         * Applies brush to the terrain when painting with water.
         * @param cameraInfo Camera information for the current camera.
         */
        public override void RenderGeometry(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderGeometry(cameraInfo);

            if (!m_isPainting)
                return;

            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (RayCastTerrain(mouseRay, out Vector3 hitPos))
            {
                int x = Mathf.FloorToInt(hitPos.x);
                int y = Mathf.FloorToInt(hitPos.y);
                int z = Mathf.FloorToInt(hitPos.z);
                float height = TerrainManager.instance.GetBlockHeight(x, z);

                height = Mathf.Min(1f, height + 0.1f);
                TerrainModify.UpdateArea(x, y, z, height, true, false, false);
            }
        }

        /**
         * @brief
         * Casts a ray from the mouse position to the terrain and returns the hit position. 
         * Uses the following layers: 15, 17, 19, 20, 21, and 22 of the terrain.
         * @param ray Ray from the mouse position to the terrain.
         * @param hitPosition The position that the ray hits the terrain.
         * @return true if the ray hits the terrain.
         * @return false if the ray does not hit the terrain.
         */
        private bool RayCastTerrain(Ray ray, out Vector3 hitPosition)
        {
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 1000f, 
                               1 << 15 | 1 << 17 | 1 << 19 | 1 << 20 | 1 << 21 | 1 << 22))
            {
                hitPosition = raycastHit.point;
                return true;
            }

            hitPosition = Vector3.zero;
            return false;
        }
    }
}