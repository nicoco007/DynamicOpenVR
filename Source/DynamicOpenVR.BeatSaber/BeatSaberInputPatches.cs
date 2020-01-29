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

using Harmony;
using System;
using UnityEngine.XR;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
namespace DynamicOpenVR.BeatSaber
{
	[HarmonyPatch(typeof(VRControllersInputManager))]
	[HarmonyPatch("TriggerValue", MethodType.Normal)]
    internal class TriggerValuePatch
	{
        [HarmonyPriority(Priority.First)]
		public static bool Prefix(XRNode node, ref float __result)
		{
			try
			{
				if (node == XRNode.LeftHand)
				{
					__result = Plugin.leftTriggerValue.value;
				}
				else if (node == XRNode.RightHand)
				{
					__result = Plugin.rightTriggerValue.value;
				}
			}
			catch (Exception ex)
			{
				__result = 0;
			}

			return false;
		}
	}

	[HarmonyPatch(typeof(VRControllersInputManager))]
	[HarmonyPatch("MenuButtonDown", MethodType.Normal)]
    internal class MenuButtonDownPatch
	{
        // ReSharper disable once RedundantAssignment
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(ref bool __result)
		{
			try
			{
				__result = Plugin.menu.activeChange;
			}
			catch (Exception)
			{
				__result = false;
			}

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
			try
			{
				__result = Plugin.menu.state;
			}
			catch (Exception)
			{
				__result = false;
			}

			return false;
		}
	}

	[HarmonyPatch(typeof(VRPlatformHelper))]
	[HarmonyPatch("TriggerHapticPulse", MethodType.Normal)]
    internal class TriggerHapticPulsePatch
	{
        [HarmonyPriority(Priority.First)]
		public static bool Prefix(XRNode node, float strength)
		{
			try
			{
				if (node == XRNode.LeftHand)
				{
					Plugin.leftSlice.TriggerHapticVibration(0.05f, strength, 25f);
				}
				else if (node == XRNode.RightHand)
				{
					Plugin.rightSlice.TriggerHapticVibration(0.05f, strength, 25f);
				}
			}
			catch (Exception) { }

			return false;
		}
	}
}
