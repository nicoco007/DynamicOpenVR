// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2020 Nicolas Gnyra

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.

using HarmonyLib;
using UnityEngine.XR;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
namespace DynamicOpenVR.BeatSaber.HarmonyPatches
{
	[HarmonyPatch(typeof(VRControllersInputManager))]
	[HarmonyPatch("TriggerValue", MethodType.Normal)]
    internal class VRControllersInputManager_TriggerValue
	{
        [HarmonyPriority(Priority.First)]
		public static bool Prefix(XRNode node, ref float __result)
		{
			if (node == XRNode.LeftHand)
			{
				__result = Plugin.leftTriggerValue.value;
				return false;
			}

			if (node == XRNode.RightHand)
			{
				__result = Plugin.rightTriggerValue.value;
				return false;
			}

			return true;
		}
	}

	[HarmonyPatch(typeof(VRControllersInputManager))]
	[HarmonyPatch("MenuButtonDown", MethodType.Normal)]
    internal class VRControllersInputManager_MenuButtonDown
	{
        // ReSharper disable once RedundantAssignment
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(ref bool __result)
		{
			__result = Plugin.menu.activeChange;

			return false;
		}
	}

	[HarmonyPatch(typeof(VRControllersInputManager))]
	[HarmonyPatch("MenuButton", MethodType.Normal)]
    internal class MenuButtonPatch
	{
        // ReSharper disable once RedundantAssignment
        [HarmonyPriority(Priority.First)]
		public static bool Prefix(ref bool __result)
		{
			__result = Plugin.menu.state;

			return false;
		}
	}

	[HarmonyPatch(typeof(OpenVRHelper))]
	[HarmonyPatch("TriggerHapticPulse", MethodType.Normal)]
    internal class OpenVRHelper_TriggerHapticPulse
	{
        [HarmonyPriority(Priority.Last)]
		public static bool Prefix(XRNode node, float strength)
		{
			if (node == XRNode.LeftHand)
			{
				Plugin.leftSlice.TriggerHapticVibration(0.01f, strength, 25f);
				return false;
			}

			if (node == XRNode.RightHand)
			{
				Plugin.rightSlice.TriggerHapticVibration(0.01f, strength, 25f);
				return false;
			}
			
			return true;
		}
	}

	[HarmonyPatch(typeof(OpenVRHelper))]
	[HarmonyPatch("Update", MethodType.Normal)]
	internal class OpenVRHelper_Update
    {
		public static bool Prefix()
        {
			// prevent OpenVRHelper from consuming events
			return false;
        }
    }
}
