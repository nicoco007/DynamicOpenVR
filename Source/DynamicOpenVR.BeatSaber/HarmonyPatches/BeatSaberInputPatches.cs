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
using System;
using System.Reflection;
using UnityEngine;
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
		private static bool _scrollingLastFrame;
		private static FieldInfo _joystickWasCenteredThisFrameEvent;
		private static FieldInfo _joystickWasNotCenteredThisFrameEvent;

		static OpenVRHelper_Update()
        {
			_joystickWasCenteredThisFrameEvent = typeof(OpenVRHelper).GetField("joystickWasCenteredThisFrameEvent", BindingFlags.NonPublic | BindingFlags.Instance);
			_joystickWasNotCenteredThisFrameEvent = typeof(OpenVRHelper).GetField("joystickWasNotCenteredThisFrameEvent", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public static bool Prefix(OpenVRHelper __instance)
        {
			Vector2 vector = Plugin.thumbstick.vector;

			if (vector.sqrMagnitude <= 0.01f)
			{
				if (_scrollingLastFrame)
				{
					_scrollingLastFrame = false;
					InvokeEvent(__instance, _joystickWasCenteredThisFrameEvent);
				}
			}
			else
            {
				_scrollingLastFrame = true;
				InvokeEvent(__instance, _joystickWasNotCenteredThisFrameEvent, vector);
			}

			// prevent OpenVRHelper from consuming events
			return false;
        }

		private static void InvokeEvent<T>(T obj, FieldInfo field, params object[] args)
        {
			var multicastDelegate = (MulticastDelegate)field.GetValue(obj);

			if (multicastDelegate == null) return;

			foreach (Delegate handler in multicastDelegate.GetInvocationList())
			{
				handler.Method.Invoke(handler.Target, args);
			}
		}
    }
}
