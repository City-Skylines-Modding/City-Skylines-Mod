/**
 * @file UnlimitedOreOil.cs
 * @author Carlos Salguero
 * @brief
 * Unlimited Oil and Ore Resource Mod
 * @version 0.1
 * @date 2022-09-29
 * @copyright Copyright (c) 2022
 */

using ICities;
using UnityEngine;

namespace ResourceMod
{
    public class UnlimitedOilAndOreResource : ResourceExtensionBase
    {
        // Mod Name
        public string Name
        {
            get { return "Unlimited Ore & Oil Resource Mod";  }
        }

        // Mod Description
        public string Description
        {
            get { return "Provides the player with unlimited Ore and Oil resouce.";  }
        }

        /**
         * @brief
         * Modifies the base game resources providing with unlimited Ore and Oil
         * to the player. 
         * @param x
         * @param z             
         * @param type          Enumeration of the resource in the industries expansion
         * @param amount        Ore amount
         */
        public override void OnAfterResourcesModified(int x, int z, 
            NaturalResource type, int amount)
        {
            if ((type == NaturalResource.Oil || type == NaturalResource.Ore)
                && amount < 0)
            {
                resourceManager.SetResource(x, z, type,
                       (byte)(resourceManager.GetResource(x, z, type) - amount),
                       false);
            }
        }
    }
}