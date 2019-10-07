using DynamicOpenVR;
using DynamicOpenVR.IO;
using Harmony;
using System;
using UnityEngine.XR;

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
				__result = OpenVRActionManager.Instance.GetAction<ButtonInput>(Plugin.ActionSetName, Plugin.MenuAction).GetButtonDown();
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
				__result = OpenVRActionManager.Instance.GetAction<ButtonInput>(Plugin.ActionSetName, Plugin.MenuAction).GetButton();
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
}
