/**
 * Copyright (c) 2012 Nokia Corporation.
 */

using CarTrumps.Resources;

namespace CarTrumps
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}