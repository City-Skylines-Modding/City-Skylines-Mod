using ICities;

namespace UnlimitedOilandOre
{
    public class UnlimitedOilandOre : IUserMod
    {
        public string Name
        {
            get { return "Infinite Oil and Ore"; }
        }

        public string Description
        {
            get { return "Provides the player with unlimited oil and ores."; }
        }
    }

    public class UnlimitedOilandOreResource : ResourceExtensionBase
    {
        /**
         * @brief
         * Overrides the base resource extension. Gives the player unlimited Oil and Ore resources
         * even if the are doesn't have any resources. 
         * @param x 
         * @param z 
         * @param type type of resource to give the player
         * @param amount amount of resources to give the player
         */
        public override void OnAfterResourcesModified(int x, int z, NaturalResource type, int amount)
        {
            if ((type == NaturalResource.Oil || type == NaturalResource.Ore) &&
                    amount < 0)
            {
                resourceManager.SetResource(x, z, type,
                    (byte)(resourceManager.GetResource(x, z, type) - amount), false);
            }
        }
    }
}