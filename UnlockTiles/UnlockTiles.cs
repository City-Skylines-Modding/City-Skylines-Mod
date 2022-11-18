using System;
using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace UnlockAllTiles
{
    public class AllTilesStart : IUserMod
    {
        public string Name
        {
            get { return "Unlock All Tiles"; }
        }

        public string Description
        {
            get { return "Unlocks all tiles in the map editor."; }
        }
    }

    public class ATSAreasExtensionBase : AreasExtensionBase
    {
        /**
         * @brief 
         * Overrides the default behavior of the game to allow all tiles to be unlocked.
         * @param ore Area's ore amount
         * @param oil Area's oil amount
         * @param forest Area's forest amount
         * @param fertility Area's fertility
         * @param water Area's water amount
         * @param road Area's road amount
         * @param train Area's train amount 
         * @param ship indicating if the area can receive ships
         * @param plane indicating if the area can receive planes
         * @param landFlatness Area's land flatness
         * @param originalPrice Area's original price
         * @return originalPrice new area's price set to 0.
         */
        public override int OnGetAreaPrice(uint ore, uint oil, uint forest, uint fertility,
            uint water, bool road, bool train, bool ship, bool plane, float landFlatness,
            int originalPrice)
        {
            if (MilestonesExtension.Instance == null || !MilestonesExtension.Instance.unlockedAreas)
            {
                originalPrice = 0;
            }

            return originalPrice;
        }

        /**
         * @brief
         * Overrides the default behavior of the game to allow all tiles to be unlocked.
         * @param x X coordinate of the tile
         * @param z Z coordinate of the tile
         * @param originalResult original result of the tile
         * @return true if all tiles are unlocked.
         */
        public override bool OnCanUnlockArea(int x, int z, bool originalResult)
        {
            if (MilestonesExtension.Instance == null || !MilestonesExtension.Instance.unlockedAreas)
            {
                if (!GameAreaManager.instance.IsUnlocked(x, z))
                {
                    originalResult = true;
                }
            }

            return originalResult;
        }
    }

    public class MilestonesExtension : MilestonesExtensionBase
    {
        public static MilestonesExtension Instance { get; private set; }
        public IManagers Managers;
        public bool unlockedAreas = false;

        /**
         * @brief 
         * Thread: Main
         * Called when the game is loaded.
         * @param milestones Milestones object
         */
        public override void OnCreated(IMilestones milestones)
        {
            Instance = this;
            Managers = milestones.managers;
        }

        /**
         * @brief 
         * Thread: Main 
         * Overrides refresh milestones function. 
         */
        public override void OnRefreshMilestones()
        {
            if (Singleton<SimulationManager>.instance.m_metaData.m_updateMode
                == SimulationManager.UpdateMode.NewGameFromMap ||
                Singleton<SimulationManager>.instance.m_metaData.m_updateMode
                == SimulationManager.UpdateMode.NewMap)
            {

                // only do once
                if (!unlockedAreas)
                {
                    IAreas AreasManager = Managers.areas;

                    // calculate original cash
                    long originalCash = EconomyManager.instance.InternalCashAmount;

                    // causes rendering issue in new areas
                    Singleton<UnlockManager>.instance.UnlockAllProgressionMilestones();

                    // unlock all tiles
                    int rows = (int)Math.Sqrt(AreasManager.maxAreaCount);

                    for (int x = 0; x < rows; x++)
                    {

                        for (int y = 0; y < rows; y++)
                        {
                            int column = x;
                            int row = y;

                            if (rows.Equals(3))
                            {
                                column = x + 1;
                                row = y + 1;
                            }

                            if (!AreasManager.IsAreaUnlocked(column, row) && AreasManager.CanUnlockArea(column, row))
                            {
                                AreasManager.UnlockArea(column, row, false);
                            }
                        }
                    }

                    // copy milestone info so we can reset it to default
                    MilestoneInfo[] MilestoneInfos = new MilestoneInfo[UnlockManager.instance.m_allMilestones.Count];
                    UnlockManager.instance.m_allMilestones.Values.CopyTo(MilestoneInfos, 0);

                    UnlockManager.ResetMilestones(MilestoneInfos, false);

                    // calculated added cash
                    long finalCash = EconomyManager.instance.InternalCashAmount;
                    int cashDifference = (Int32)(originalCash - finalCash);

                    // remove cash difference
                    EconomyManager.instance.AddResource(EconomyManager.Resource.LoanAmount, cashDifference, ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Level.None);
                }
            }

            unlockedAreas = true;
        }
    }

    public class LoadingExtension : LoadingExtensionBase
    {
        /**
         * @brief 
         * Thread: Simulation
         * Overrides the loading function for the game.
         * @param mode Mode of the game
         */
        public override void OnLevelLoaded(LoadMode mode)
        {
            // check for new game
            if (mode.Equals(ICities.LoadMode.NewGame))
            {
                int rows = 5;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        int column = i;
                        int row = j;

                        if (rows.Equals(3))
                        {
                            column = i + 1;
                            row = j + 1;
                        }

                        int skip = 2;
                        int num = 120;

                        TerrainModify.UpdateArea((i + skip) * num - 4, (j + skip) * num - 4,
                            ((i + skip) + 1) * num + 4, ((j + skip) + 1) * num + 4, true, true, true);
                    }
                }
            }
        }
    }
}

