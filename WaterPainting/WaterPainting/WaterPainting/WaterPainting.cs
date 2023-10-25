using ICities;
using ColossalFramework;
using UnityEngine;

namespace WaterPainting.source
{
    /// <summary>
    /// Allows the user to flood a certain area when the mouse is hovered
    /// after pressing the following key combination: 
    /// control + shift + p
    /// </summary>
    public class WaterPainting : MonoBehaviour
    {
        /// <summary>
        /// Flag to check if flooding is enabled.
        /// </summary>
        private bool isFloadingModeActive = false;

        /// <summary> 
        /// Default constructor for the class.
        /// </summary>
        public WaterPainting()
        {
            Update();
        }

        /// <summary>
        /// Updates the game every frame.
        /// </summary>
        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && 
                Input.GetKey(KeyCode.LeftShift) && 
                Input.GetKeyDown(KeyCode.P))
            {
                ToggleFloodMode();
            }

            if (isFloadingModeActive && Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 inputPosition = hit.point;

                    TerrainManager terrainManager = Singleton<TerrainManager>.instance;

                    // Show it in the debug mode of the game
                    DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, 
                                               "Input position: " + inputPosition);
                }
            }
        }

        /// <summary>
        /// Toggles the flooding on and off when the user presses the 
        /// following key combination: control + shift + p
        /// 
        /// Uses the default water texture from the game.
        /// </summary>
        private void ToggleFloodMode()
        {
            isFloadingModeActive = !isFloadingModeActive;

            if (isFloadingModeActive)
            {
                Texture2D waterTexture = Resources.FindObjectsOfTypeAll<Texture2D>()[0];
                Cursor.SetCursor(waterTexture, Vector2.zero, CursorMode.Auto);
            }

            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }
    }
}
