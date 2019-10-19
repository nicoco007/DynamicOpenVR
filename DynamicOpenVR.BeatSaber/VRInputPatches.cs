// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019 Nicolas Gnyra

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

using DynamicOpenVR.IO;
using Harmony;
using System;
using UnityEngine;
using UnityEngine.XR;

// ReSharper disable InconsistentNaming
namespace DynamicOpenVR.BeatSaber
{
	[HarmonyPatch(typeof(VRControllersInputManager))]
	[HarmonyPatch("TriggerValue", MethodType.Normal)]
	class TriggerValuePatch
	{
		public static bool Prefix(XRNode node, ref float __result)
		{
			try
			{
				if (node == XRNode.LeftHand)
				{
					__result = Plugin.LeftTriggerValue.GetValue();
				}
				else if (node == XRNode.RightHand)
				{
					__result = Plugin.RightTriggerValue.GetValue();
				}
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex);
				__result = 0;
			}

			return false;
		}
	}

	[HarmonyPatch(typeof(VRControllersInputManager))]
	[HarmonyPatch("MenuButtonDown", MethodType.Normal)]
	class MenuButtonDownPatch
	{
		public static bool Prefix(ref bool __result)
		{
			try
			{
				__result = Plugin.Menu.GetActiveChange();
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
	class MenuButtonPatch
	{
		public static bool Prefix(ref bool __result)
		{
			try
			{
				__result = Plugin.Menu.GetState();
			}
			catch (Exception)
			{
				__result = false;
			}

			return false;
		}
	}

	[HarmonyPatch(typeof(OpenVRHelper))]
	[HarmonyPatch("TriggerHapticPulse", MethodType.Normal)]
	class TriggerHapticPulsePatch
	{
		public static bool Prefix(XRNode node, float strength)
		{
			try
			{
				if (node == XRNode.LeftHand)
				{
					Plugin.LeftSlice.TriggerHapticVibration(0.05f, strength);
				}
				else if (node == XRNode.RightHand)
				{
					Plugin.RightSlice.TriggerHapticVibration(0.05f, strength);
				}
			}
			catch (Exception) { }

			return false;
		}
	}

    [HarmonyPatch(typeof(InputTracking))]
    [HarmonyPatch("GetLocalPosition", MethodType.Normal)]
    class InputTrackingGetLocalPositionPatch
    {
        public static bool Prefix(XRNode node, ref Vector3 __result)
        {
            if (node == XRNode.LeftHand)
            {
                __result = Plugin.LeftHandPose.GetPose().position;
                return false;
            }
            
            if (node == XRNode.RightHand)
            {
                __result = Plugin.RightHandPose.GetPose().position;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InputTracking))]
    [HarmonyPatch("GetLocalRotation", MethodType.Normal)]
    class InputTrackingGetLocalRotationPatch
    {
        public static bool Prefix(XRNode node, ref Quaternion __result)
        {
            if (node == XRNode.LeftHand)
            {
                __result = Plugin.LeftHandPose.GetPose().rotation;
                return false;
            }
            
            if (node == XRNode.RightHand)
            {
                __result = Plugin.RightHandPose.GetPose().rotation;
                return false;
            }

            return true;
        }
    }
}
