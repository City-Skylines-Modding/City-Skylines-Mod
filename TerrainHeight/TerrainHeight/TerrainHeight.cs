using ICities;
using UnityEngine;
using ColossalFramework.UI;
using System.Reflection.Emit;

namespace TerrainHeight.Source
{
    /// <summary>
    /// Shows the terrain height at the current camera position.
    /// </summary>
    public class TerrainHeight : ThreadingExtensionBase
    {
        private const float MIN_HEIGHT_ABOVE_TERRAIN = 10f;
        private const float MAX_DISPLAYABLE_HEIGHT = 1000f;


        private bool IsUpdating { get; set; } = false;
        private UILabel Label { get; set; }
        private readonly float m_zoomSpeed = 20f;
        private readonly Camera m_mainCamera;        // Cached camera reference
        private float LastHeight { get; set; } = -1; // Stored last value

        // Constructor
        /// <summary>
        /// Default constructor for the class. 
        /// Initializes the camera reference.
        /// </summary>
        public TerrainHeight()
        {
            m_mainCamera = Camera.main;
        }

        // Methods
        /// <summary>
        /// Disposes of the class's resources.
        /// </summary>
        public void Dispose()
        {
            if (Label != null)
            {
                Object.Destroy(Label);
                Label = null;
            }
        }

        /// <summary>
        /// Called when the game updates itself.
        /// <para><paramref name="realTimeDelta"/>: The time in seconds since the last update.</para>
        /// <para><paramref name="simulationTimeDelta"/>: The time in seconds since the last simulation update.</para>
        /// </summary>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            CustomZoom();

            if (Input.GetKeyDown(KeyCode.T) 
                && Input.GetKey(KeyCode.LeftControl) 
                && Input.GetKey(KeyCode.LeftShift))
            {
                IsUpdating = !IsUpdating;

                if (IsUpdating)
                    ShowTerrainHeight();

                else
                    HideTerrainHeight();
            }

            if (IsUpdating)
                UpdateTerrainHeight();

            base.OnUpdate(realTimeDelta, simulationTimeDelta);
        }

       
        // Private Methods
        /// <summary>
        /// Shows the terrain height widget
        /// </summary>
        private void ShowTerrainHeight()
        {
            if (Label == null)
                CreateUIComponent();

            Label.isVisible = true;
            UpdateTerrainHeight();
        }

         /// <summary>
         /// Hides the terrain height widget
         /// </summary>
        private void HideTerrainHeight()
        {
            if (Label != null)
                Label.isVisible = false;

            IsUpdating = false;
        }

        /// <summary>
        /// Creates the UI component for the terrain height widget
        /// </summary>
        private void CreateUIComponent()
        {
            UIView view = UIView.GetAView();
            Label = view.AddUIComponent(typeof(UILabel)) as UILabel;

            Label.name = nameof(TerrainHeight) + "Label";
            Label.relativePosition = new Vector3(60, 15);
            Label.textAlignment = UIHorizontalAlignment.Left;
            Label.verticalAlignment = UIVerticalAlignment.Middle;

            Label.textScale = 0.8f;
            Label.padding = new RectOffset(10, 10, 10, 10);
            Label.autoSize = true;

            Label.width = 200;
            Label.height = 30;

            Label.wordWrap = false;
            Label.backgroundSprite = "ButtonMenu";

            Label.color = new Color32(255, 255, 255, 255);
            Label.opacity = 0.8f;
        }

        /// <summary>
        /// Adjusts the caemra's vertical position based on user input, 
        /// ensuring consistent zoom speed regardless of the current zoom level.
        /// </summary>
        private void CustomZoom()
        {
            float zoom = Input.GetAxis("Mouse ScrollWheel") * m_zoomSpeed;
            if (zoom == 0) return;

            Vector3 cameraPosition = m_mainCamera.transform.position;
            Vector3 newPosition = new Vector3(cameraPosition.x, 
                cameraPosition.y + zoom, cameraPosition.z);

            Vector3 rayOrigin = m_mainCamera
                .ScreenToWorldPoint(
                    new Vector3(Screen.width / 2, Screen.height / 2, m_mainCamera.nearClipPlane));

            Ray ray = new Ray(rayOrigin + Vector3.up * 5000f, Vector3.down);
            RaycastHit hit;
            int terrainLayerMask = 1 << 8;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayerMask))
            {
                float terrainHeight = hit.point.y;

                if (newPosition.y < terrainHeight + MIN_HEIGHT_ABOVE_TERRAIN)
                    newPosition.y = terrainHeight + MIN_HEIGHT_ABOVE_TERRAIN;
            }

            m_mainCamera.transform.position = newPosition;
        }

        /// <summary>
        /// Updates the displayed terrain height based on the camera's current position.
        /// Temporary sets the camera to a top-down view to sample the height. And samples
        /// the height from the top-down view. Then reverts the camera back to its original
        /// view
        /// </summary>
        private void UpdateTerrainHeight()
        {
            if (Label == null) return;

            Vector3 originalPosition = m_mainCamera.transform.position;
            Quaternion originalRotation = m_mainCamera.transform.rotation;

            m_mainCamera.transform.position = new Vector3(
                originalPosition.x, 5000f, originalPosition.z); 
            m_mainCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f); 

            Vector3 rayOrigin = m_mainCamera.ScreenToWorldPoint(
                new Vector3(Screen.width / 2, Screen.height / 2, m_mainCamera.nearClipPlane));

            float height = TerrainManager.instance
                .SampleRawHeightSmoothWithWater(rayOrigin, true, 0);

            m_mainCamera.transform.position = originalPosition;
            m_mainCamera.transform.rotation = originalRotation;

            if (height > MAX_DISPLAYABLE_HEIGHT)
                return;

            height = Mathf.Clamp(height, 0, MAX_DISPLAYABLE_HEIGHT);

            if (Mathf.Abs(height - LastHeight) > 0.000001f)
            {
                LastHeight = height;
                Label.text = "Terrain Height: " + height.ToString("F6") + " m";
            }
        }

    }
}
