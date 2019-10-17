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
					__result = OpenVRActionManager.Instance.GetAction<VectorInput>(Plugin.ActionSetName, Plugin.LeftTriggerValueAction).GetValue();
				}
				else if (node == XRNode.RightHand)
				{
					__result = OpenVRActionManager.Instance.GetAction<VectorInput>(Plugin.ActionSetName, Plugin.RightTriggerValueAction).GetValue();
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
				__result = OpenVRActionManager.Instance.GetAction<BooleanInput>(Plugin.ActionSetName, Plugin.MenuAction).GetActiveChange();
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
				__result = OpenVRActionManager.Instance.GetAction<BooleanInput>(Plugin.ActionSetName, Plugin.MenuAction).GetState();
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
					OpenVRActionManager.Instance.GetAction<HapticVibrationOutput>(Plugin.ActionSetName, Plugin.LeftSliceAction).TriggerHapticVibration(0.05f, strength);
				}
				else if (node == XRNode.RightHand)
				{
					OpenVRActionManager.Instance.GetAction<HapticVibrationOutput>(Plugin.ActionSetName, Plugin.RightSliceAction).TriggerHapticVibration(0.05f, strength);
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
                __result = OpenVRActionManager.Instance.GetAction<PoseInput>(Plugin.ActionSetName, Plugin.LeftHandPoseName).GetPose().position;
                return false;
            }
            
            if (node == XRNode.RightHand)
            {
                __result = OpenVRActionManager.Instance.GetAction<PoseInput>(Plugin.ActionSetName, Plugin.RightHandPoseName).GetPose().position;
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
                __result = OpenVRActionManager.Instance.GetAction<PoseInput>(Plugin.ActionSetName, Plugin.LeftHandPoseName).GetPose().rotation;
                return false;
            }
            
            if (node == XRNode.RightHand)
            {
                __result = OpenVRActionManager.Instance.GetAction<PoseInput>(Plugin.ActionSetName, Plugin.RightHandPoseName).GetPose().rotation;
                return false;
            }

            return true;
        }
    }
}
