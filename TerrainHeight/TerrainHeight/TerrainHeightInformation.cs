/**
 * @file TerrainHeightInformation.cs
 * @brief Terrain height information class.
 * @author Carlos Salguero
 * @date 2023-08-28
 * @version 4.0.0
 * 
 * @copyright Copyright (c) - Tec de Monterrey
 * 
 */

using ICities;

namespace TerrainHeight.Source
{
    /**
     * @class TerrainHeightInformation
     * @implements IUserMod
     * @brief Terrain height information class.
     */
    public class TerrainHeightInformation : IUserMod
    {
        public string Name => "Terrain Height Information";
        public string Description => "Shows the terrain height information as a widget.";
    }
}