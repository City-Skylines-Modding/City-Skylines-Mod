using ICities;

namespace WaterPainting.source
{
    /// <summary>
    /// Provides information about the plugin in the plugin manager
    /// of City Skylines.
    /// </summary>
    public class WaterPaintingDetails : IUserMod
    {
        public string Name => "Water Painting";
        public string Description => "Allows the user to flood a certain area with the mouse";

        public string Author => "Carlos Salguero";
    }
}
