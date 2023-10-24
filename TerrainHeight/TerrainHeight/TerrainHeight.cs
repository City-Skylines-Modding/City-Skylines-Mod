/**
* @file TerrainHeight.cs
* @brief Main class of the mod. Handles the logic
* @author Carlos Salguero
* @date 2023-08-28
* @version 4.0.0
* 
* @copyright Copyright (c) - Tec de Monterrey
*
*/
using ICities;
using UnityEngine;
using ColossalFramework.UI;

namespace TerrainHeight.Source
{
    /**
     * @class TerrainHeight
     * @implements ThreadingExtensionBase
     * @brief Main class of the mod. Handles the logic
     */
    public class TerrainHeight : ThreadingExtensionBase
    {
        private bool m_isUpdating = false;
        private UILabel m_label = null;
        private readonly float m_zoomSpeed = 20f;
        private readonly Camera m_mainCamera;      // Cached camera reference
        private float lastHeight = -1;    // Stored last value

        // Constructor
        /**
         * Construct a new TerrainHeight object
         */
        public TerrainHeight()
        {
            m_mainCamera = Camera.main;
        }

        // Methods
        /**
         * @brief 
         * Disposes of the class's instance
         */
        public void Dispose()
        {
            if (m_label != null)
            {
                Object.Destroy(m_label);
                m_label = null;
            }
        }

        /**
         * @brief
         * Called when the game updates itself.
         * @parma realTimeDelta The time in seconds since the last update.
         * @param simulationTimeDelta The time in seconds since the last simulation update.
         */
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            CustomZoom();

            if (Input.GetKeyDown(KeyCode.T) 
                && Input.GetKey(KeyCode.LeftControl) 
                && Input.GetKey(KeyCode.LeftShift))
            {
                m_isUpdating = !m_isUpdating;

                if (m_isUpdating)
                    ShowTerrainHeight();

                else
                    HideTerrainHeight();
            }

            if (m_isUpdating)
                UpdateTerrainHeight();

            base.OnUpdate(realTimeDelta, simulationTimeDelta);
        }

       
        // Private Methods
        /**
        * @brief 
        * Shows the terrain height widget
        */
        private void ShowTerrainHeight()
        {
            if (m_label == null)
                CreateUIComponent();

            m_label.isVisible = true;
            UpdateTerrainHeight();
        }

        /**
         * @brief
         * Hides the terrain height widget
         */
        private void HideTerrainHeight()
        {
            if (m_label != null)
                m_label.isVisible = false;

            m_isUpdating = false;
        }

        /**
         * @brief
         * Creates the UI Component for the terrain height widget
         */
        private void CreateUIComponent()
        {
            UIView view = UIView.GetAView();
            m_label = view.AddUIComponent(typeof(UILabel)) as UILabel;

            m_label.name = "TerrainHeightLabel";
            m_label.relativePosition = new Vector3(60, 15);
            m_label.textAlignment = UIHorizontalAlignment.Left;
            m_label.verticalAlignment = UIVerticalAlignment.Middle;

            m_label.textScale = 0.8f;
            m_label.padding = new RectOffset(10, 10, 10, 10);
            m_label.autoSize = true;

            m_label.width = 200;
            m_label.height = 30;

            m_label.wordWrap = false;
            m_label.backgroundSprite = "ButtonMenu";

            m_label.color = new Color32(255, 255, 255, 255);
            m_label.opacity = 0.8f;
        }

        /**
         * Handles the custom camera zoom. 
         * @details Removes the camera's height from the zoom calculation so that
         *          it is not affected by the camera's height.  
         */
        private void CustomZoom()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            Vector3 moveDirection = m_mainCamera.transform.forward;

            moveDirection.y = 0;

            m_mainCamera.transform.position -= moveDirection * scrollInput * m_zoomSpeed;
        }

        /**
         * @brief
         * Updates the terrain height widget with the current terrain height. 
         * @details Sets a max height of 1000 meters so that the result of the 
         *          height is accurate and is not affected by the camera's height.
         * @details The height is rounded to 6 decimal places.
         */
        private void UpdateTerrainHeight()
        {
            if (m_label == null) return;

            float height = Mathf.Clamp
                (TerrainManager.instance.SampleRawHeightSmoothWithWater(
                 Camera.main.transform.position, true, 1000), 0, 1000);

            if (Mathf.Abs(height - lastHeight) > 0.000001f)
            {
                m_label.text = "Terrain Height: " + height.ToString("F6") + " m";
                lastHeight = height;
            }
        }
    }
}
