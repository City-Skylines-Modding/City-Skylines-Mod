using ICities;
using UnityEngine;
using ColossalFramework.UI;
using System.Collections.Generic;

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
        private static readonly List<UILabel> uILabels = new List<UILabel>();
        private readonly List<UILabel> savedLabels = uILabels;
        private Vector3 fixedRayOrigin;

        // Cached camera reference
        private readonly Camera m_mainCamera;

        // Stored last value
        private float LastHeight { get; set; } = -1; 


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
        /// </summary>
        /// <para><paramref name="realTimeDelta"/>: The time in seconds since the last update.</para>
        /// <para><paramref name="simulationTimeDelta"/>: The time in seconds since the last simulation update.</para>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            CustomZoom();

            if (Input.GetKeyDown(KeyCode.T) 
                && Input.GetKey(KeyCode.LeftControl) 
                && Input.GetKey(KeyCode.LeftShift))
            {
                IsUpdating = !IsUpdating;

                if (IsUpdating)
                {
                    ShowTerrainHeight();
                    InitializedFixedRayOrigin();
                }

                else
                    HideTerrainHeight();
            }

            if (IsUpdating)
            {
                UpdateTerrainHeight();
                InitializedFixedRayOrigin();
            }

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
            int terrainLayerMask = 1 << 8;

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainLayerMask))
            {
                float terrainHeight = hit.point.y;

                if (newPosition.y < terrainHeight + MIN_HEIGHT_ABOVE_TERRAIN)
                    newPosition.y = terrainHeight + MIN_HEIGHT_ABOVE_TERRAIN;
            }

            m_mainCamera.transform.position = newPosition;
        }

        /// <summary>
        /// Creates a pin label at the specified position with the specified height
        /// </summary>
        /// <para><paramref name="position"/>: The position of the label.</para>
        /// <para><paramref name="height"/>: The height to be set in the label</para>
        /// <returns>The Label</returns>
        private UILabel CreatePinLabel(Vector3 position, float height)
        {
            UIView view = UIView.GetAView();
            UILabel label = view.AddUIComponent(typeof(UILabel)) as UILabel;

            label.name = nameof(TerrainHeight) + "Label";
            label.textAlignment = UIHorizontalAlignment.Left;
            label.verticalAlignment = UIVerticalAlignment.Middle;

            label.textScale = 0.8f;
            label.padding = new RectOffset(10, 10, 10, 10);
            label.autoSize = true;

            label.width = 200;
            label.height = 30;

            label.wordWrap = false;
            label.backgroundSprite = "ButtonMenu";

            label.color = new Color32(255, 255, 255, 255);
            label.opacity = 0.8f;

            // Set the label's position in the real world position
            label.relativePosition = position;

            label.text = $"Terrain Height: {height:F6} m";
            return label;
        }

        /// <summary>
        /// Initializes the fixed ray origin based on the current viewport's center.
        /// This fixed point is used to consistently sample terrain height regardless 
        /// of camera movement or zoom. The world space is scaled to match the game's 
        /// representation, where each "cell" unit correlates to 8x8 meters.
        /// </summary>
        /// <remarks>
        /// See https://skylines.paradoxwikis.com/Maps for more details on the game's scaling.
        /// </remarks>
        private void InitializedFixedRayOrigin()
        {
            Vector3 screenMidPoint = new Vector3(0.5f, 0.5f, m_mainCamera.nearClipPlane);
            fixedRayOrigin = m_mainCamera.ViewportToWorldPoint(screenMidPoint);

            fixedRayOrigin.x *= 8;
            fixedRayOrigin.z *= 8;
        }

        /// <summary>
        /// Updates the terrain height label based on the fixed ray origin.
        /// If the left mouse button is clicked, a pin label is created at the mouse
        /// position to mark the terrain height at that point.The terrain height is clamped
        /// to a maximum displayable height and updated only if it has changed significantly
        /// since the last update.
        /// </summary>
        /// <remarks>
        /// The fixed ray origin should be initialized before calling this method to ensure 
        /// accurate height sampling. The method accounts for the game's scaling factor,
        /// where each "cell" unit correlates to 8x8 meters.
        /// </remarks>
        private void UpdateTerrainHeight()
        {
            if (Label == null || TerrainManager.instance == null) return;

            float height = TerrainManager.instance
                .SampleRawHeightSmoothWithWater(fixedRayOrigin, true, 0);

            if (height > MAX_DISPLAYABLE_HEIGHT)
                return;

            height = Mathf.Clamp(height, 0, MAX_DISPLAYABLE_HEIGHT);

            if (Mathf.Abs(height - LastHeight) > 0.000001f)
            {
                LastHeight = height;
                Label.text = $"Terrain Height: {height:F6} m";
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = new Vector3(
                    Input.mousePosition.x, Screen.height - Input.mousePosition.y);

                // Sets the pin in a fixed world position
                Vector3 worldPosition = m_mainCamera.ScreenToWorldPoint(mousePosition);

                worldPosition.x *= 8;
                worldPosition.z *= 8;

                UILabel label = CreatePinLabel(mousePosition, height);
                savedLabels.Add(label);
            }
        }
    }
}
