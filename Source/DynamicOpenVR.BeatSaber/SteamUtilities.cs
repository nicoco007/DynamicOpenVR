// <copyright file="SteamUtilities.cs" company="Nicolas Gnyra">
// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2021 Nicolas Gnyra
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace DynamicOpenVR
{
    public static class SteamUtilities
    {
        public static string GetSteamHomeDirectory()
        {
            string steamPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", string.Empty).ToString();

            if (!string.IsNullOrEmpty(steamPath) && Directory.Exists(steamPath))
            {
                return steamPath.Replace('/', '\\');
            }

            Process steamProcess = Process.GetProcessesByName("Steam").FirstOrDefault();

            if (steamProcess == null)
            {
                throw new Exception("Steam process could not be found.");
            }

            var stringBuilder = new StringBuilder(2048);
            int capacity = stringBuilder.Capacity + 1;

            if (NativeMethods.QueryFullProcessImageName(steamProcess.Handle, 0, stringBuilder, ref capacity) == 0)
            {
                throw new Exception("QueryFullProcessImageName returned 0");
            }

            string exePath = stringBuilder.ToString();

            if (string.IsNullOrEmpty(exePath))
            {
                throw new Exception("Steam path could not be found.");
            }

            steamPath = Path.GetDirectoryName(exePath);

            return steamPath.Replace('/', '\\');
        }
    }
}
