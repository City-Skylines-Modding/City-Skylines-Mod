/**
 * @file UnlimitedMoney.cs
 * @author Carlos Salguero
 * @brief 
 * Unlimited money mod
 * @version 0.1
 * @date 2022-11-17
 * @copyright Copyright (c) 2022
 */

using System;
using System.IO;
using System.Reflection;
using ICities;
using UnityEngine;

namespace UnlimitedMoney
{
    public class UnlimitedMoney : IUserMod
    {
        public string Name
        {
            get { return "Unlimited Money"; }
        }

        public string Description
        {
            get { return "Provides the player with unlimited money."; }
        }
    }

    public class UnlimitedMoney_Economy : EconomyExtensionBase
    {
        /**
         * @brief 
         * Overrides the base class starting budget. 
         * @param budget player's budget
         * @return long MaxValue unlimited money.
         */
        public override long OnUpdateMoneyAmount(long budget)
        {
            return long.MaxValue;
        }

        /**
         * @brief 
         * Overrides Default Peek Resource function. 
         */
        public override bool OverrideDefaultPeekResource
        {
            get { return true; }
        }

        /**
         * @brief
         * Overrides OnPeekResource function. 
         * @param resource resource to peek
         * @param amount amount of resource to peek
         */
        public override int OnPeekResource(EconomyResource resource, int amount)
        {
            return amount;
        }
    }
}