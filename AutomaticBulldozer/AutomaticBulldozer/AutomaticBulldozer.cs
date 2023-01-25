using System;
using System.Collections;
using System.Collections.Generic;

using ColossalFramework;
using ICities;

namespace AutomaticBulldoze.Source
{
    public class AutomaticBulldozeInformation : IUserMod
    {
        public string Name
        {
            get { return "Automatic Bulldoze"; }
        }

        public string Description
        {
            get { return "Automatically bulldoze abandoned buildings"; }
        }
    }

    public class AutomaticBulldozeLoader : LoadingExtensionBase
    {
        private AutomaticBulldozer _automaticBulldozer;

        /**
         * @brief
         * Overrides the default behaviour of the OnLevelLoaded function. 
         * This implementation creates a new object of type AutomaticBulldozer.
         * @param mode The mode in which the game is accessed (created or loaded)
         * @return void
         */
        public override void OnLevelLoaded(LoadMode mode)
        {
            _automaticBulldozer = new AutomaticBulldozer();

            base.OnLevelLoaded(mode);
        }

        /**
         * @brief
         * Overrides the default behaviour of the OnLevelUnloading function.
         * This implementation deletes the AutomaticBulldozer object created
         * in OnLevelLoaded function
         * @return void
         */
        public override void OnLevelUnloading()
        {
            if (_automaticBulldozer != null)
                _automaticBulldozer.Destroy();

            _automaticBulldozer = null;
            base.OnLevelUnloading();
        }
    }

    public class AutomaticBulldozer
    {

    }
}