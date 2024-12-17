// <copyright file="XRLayoutBuilder.cs" company="Nicolas Gnyra">
// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2023 Nicolas Gnyra
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

using HarmonyLib;
using UnityEngine.InputSystem.XR;

namespace DynamicOpenVR.BeatSaber.HarmonyPatches
{
    /// <summary>
    /// This patch prevents XRInputV1 devices (provided by the native OpenVR plugin) from being registered automatically and conflicting with our Input System devices.
    /// </summary>
    [HarmonyPatch(typeof(XRLayoutBuilder), nameof(XRLayoutBuilder.OnFindLayoutForDevice))]
    [HarmonyPatchCategory(Plugin.kOpenVRLoaderHarmonyCategory)]
    internal static class XRLayoutBuilder_OnFindLayoutForDevice
    {
        public static bool Prefix() => false;
    }
}
