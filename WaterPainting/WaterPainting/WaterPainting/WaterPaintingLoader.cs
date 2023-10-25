using ICities;
using ColossalFramework;

namespace WaterPainting.source
{
    /// <summary>
    /// Loads the plugin into the game. 
    /// </summary>
    public class WaterPaintingLoader : LoadingExtensionBase
    {
        private WaterPainting waterPainting;

        /// <summary>
        /// Loads the mod when the game is loaded or created.
        /// 
        /// <paramref name="mode"/> is the mode in which the game is loaded.
        /// </summary>
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                waterPainting = new WaterPainting();
            }

            base.OnLevelLoaded(mode);
        }

        /// <summary>
        /// Disables the mod when the game is closed. 
        /// </summary>
        public override void OnLevelUnloading()
        {
            waterPainting = null;

            base.OnLevelUnloading();
        }
    }
}
