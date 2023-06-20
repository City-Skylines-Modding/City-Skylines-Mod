using System;
using UnityEngine;
using UnityEngine.UI;
using ICities;

namespace WaterPainting.Source
{
    /**
     * @class WaterPaintingInformation
     * @extends IUserMod
     * @brief
     * Gives the game information about the mod.
     */
    public class WaterPaintingInformation : IUserMod
    {
        public string Name => "Water Painting";
        public string Description => "Paint water with the terrain tool when " +
            "the keys ctrl + shift + p are pressed.";
    }

    /**
     * @class WaterPaintingLoader
     * @extends LoadingExtensionBase
     * @brief
     * Loads the mod when the game is loaded.
     */
    public class WaterPaintingLoader : LoadingExtensionBase
    {
        private WaterPainting waterPainting;
        private GameObject bannerObject;
        private Text bannerText;

        /**
         * @brief
         * Overloaded OnLevelLoaded method. Creates an instance of the
         * mod
         * @param mode The mode the game is in.
         */
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                waterPainting = new WaterPainting();

                // Create the banner object
                bannerObject = new GameObject("WaterPaintingBanner", typeof(RectTransform), typeof(CanvasRenderer));
                bannerObject.transform.parent = bannerObject.transform;
                bannerObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 100f);

                // Add a Text component to the banner object
                bannerText = bannerObject.AddComponent<Text>();
                bannerText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                bannerText.text = "Water Painting Mod Active";
                bannerText.fontSize = 24;
                bannerText.color = Color.white;
                bannerText.alignment = TextAnchor.MiddleCenter;
            }

            base.OnLevelLoaded(mode);
        }

        /**
         * @brief
         * Overrides the defualt behaviour of the OnLevelUnloading function.
         */
        public override void OnLevelUnloading()
        {
            if (waterPainting != null)
            {
                waterPainting.Dispose();
                waterPainting = null;
            }

            base.OnLevelUnloading();
        }
    }

    /**
     * @class WaterPainting
     * @extends MonoBehaviour, ThreadingExtensionBase, IDisposable
     * @brief
     * Handles the mod's logic.
     */
    public class WaterPainting : ThreadingExtensionBase, IDisposable
    {
        private bool isPainting = false;

        /**
         * @brief
         * Disposes of the tool
         */
        public void Dispose()
        {
            if (isPainting)
            {
                isPainting = false;
            }
        }

        /**
         * @brief
         * Overrides the default behaviour of the OnUpdate function.
         * Generates the tool when the keys ctrl + shift + p are pressed.
         * @param realTimeDelta The real time delta.
         * @param simulationTimeDelta The simulation time delta.
         */
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (isKeysPressed())
            {
                isPainting = !isPainting;
            }

            if (isPainting)
            {
                PainWater();
            }

            base.OnUpdate(realTimeDelta, simulationTimeDelta);
        }

        /**
         * @brief
         * Checks if the following keys are pressed: ctrl + shift + t
         * @return True if the following keys are pressed: ctrl + shift + t
         * @return False if the following keys are not pressed: ctrl + shift + t
         */
        private bool isKeysPressed()
        {
            return Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) 
                && Input.GetKeyDown(KeyCode.P);
        }

        /**
         * @brief
         * Paints the water with the terrain tool.
         */
        private void PainWater()
        {
            Action<GameObject> instantiateWater = (GameObject waterPrefab) =>
            {
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit))
                {
                    Vector3 waterPosition = raycastHit.point;
                    GameObject.Instantiate(waterPrefab, waterPosition, Quaternion.identity);
                }
            };

            GameObject WaterPrefab = Resources.Load<GameObject>("WaterPrefab");
            instantiateWater(WaterPrefab);

            // Paints the water at the mouse position with the terrain tool.
            Vector3 MousePosition = Input.mousePosition;
            Ray Ray = Camera.main.ScreenPointToRay(MousePosition);
            RaycastHit m_raycastHit;

            if (Physics.Raycast(Ray, out m_raycastHit))
            {
                Vector3 WaterPosition = m_raycastHit.point;
                float waterLevel = WaterPosition.y;

                TerrainModify.UpdateArea(
                    WaterPosition.x - 10f,
                    WaterPosition.z - 10f,
                    WaterPosition.x + 10f,
                    waterLevel,
                    false,
                    false,
                    false
                );
            }
        }
    }
}
