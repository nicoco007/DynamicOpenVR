using DynamicOpenVR.IO;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
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
					__result = OpenVRActionManager.Instance.GetAction<VectorInput>(Plugin.ActionSetName, Plugin.LeftTriggerValueAction).GetAxisRaw();
				}
				else if (node == XRNode.RightHand)
				{
					__result = OpenVRActionManager.Instance.GetAction<VectorInput>(Plugin.ActionSetName, Plugin.RightTriggerValueAction).GetAxisRaw();
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
				__result = OpenVRActionManager.Instance.GetAction<BooleanInput>(Plugin.ActionSetName, Plugin.MenuAction).GetButtonDown();
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
				__result = OpenVRActionManager.Instance.GetAction<BooleanInput>(Plugin.ActionSetName, Plugin.MenuAction).GetButton();
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
                __result = OpenVRActionManager.Instance.GetAction<PoseInput>(Plugin.ActionSetName, Plugin.LeftHandPoseName).GetPosition();
                return false;
            }
            
            if (node == XRNode.RightHand)
            {
                __result = OpenVRActionManager.Instance.GetAction<PoseInput>(Plugin.ActionSetName, Plugin.RightHandPoseName).GetPosition();
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
                __result = OpenVRActionManager.Instance.GetAction<PoseInput>(Plugin.ActionSetName, Plugin.LeftHandPoseName).GetRotation();
                return false;
            }
            
            if (node == XRNode.RightHand)
            {
                __result = OpenVRActionManager.Instance.GetAction<PoseInput>(Plugin.ActionSetName, Plugin.RightHandPoseName).GetRotation();
                return false;
            }

            return true;
        }
    }
}
