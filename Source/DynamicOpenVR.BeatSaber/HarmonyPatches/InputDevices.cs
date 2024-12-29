// <copyright file="InputDevices.cs" company="Nicolas Gnyra">
// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2024 Nicolas Gnyra
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
using System.Collections.Generic;
using DynamicOpenVR.BeatSaber.InputCollections;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace DynamicOpenVR.BeatSaber.HarmonyPatches
{
    [HarmonyPatch(typeof(InputDevice), nameof(InputDevice.TryGetFeatureUsages))]
    [HarmonyPatchCategory(Plugin.kOpenVRLoaderHarmonyCategory)]
    internal static class InputDevice_TryGetFeatureUsages
    {
        public static bool Prefix(InputDevice __instance, List<InputFeatureUsage> featureUsages, ref bool __result)
        {
            if (!__instance.isValid || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.Controller) || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.HeldInHand))
            {
                return true;
            }

            UnityXRActionsHand hand;

            if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                hand = Plugin.unityXRActions.left;
            }
            else if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                hand = Plugin.unityXRActions.right;
            }
            else
            {
                return true;
            }

            if (hand.devicePose.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.devicePosition);
                featureUsages.Add((InputFeatureUsage)CommonUsages.deviceRotation);
                featureUsages.Add((InputFeatureUsage)CommonUsages.deviceVelocity);
                featureUsages.Add((InputFeatureUsage)CommonUsages.deviceAngularVelocity);
            }

            if (hand.menu.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.menuButton);
            }

            if (hand.primaryButton.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.primaryButton);
            }

            if (hand.primaryTouched.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.primaryTouch);
            }

            if (hand.secondaryButton.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.secondaryButton);
            }

            if (hand.secondaryTouched.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.secondaryTouch);
            }

            if (hand.grip.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.grip);
            }

            if (hand.gripPressed.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.gripButton);
            }

            if (hand.trigger.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.trigger);
            }

            if (hand.triggerPressed.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.triggerButton);
            }

            if (hand.thumbstick.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.primary2DAxis);
            }

            if (hand.thumbstickClicked.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.primary2DAxisClick);
            }

            if (hand.thumbstickTouched.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.primary2DAxisTouch);
            }

            if (hand.trackpad.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.secondary2DAxis);
            }

            if (hand.trackpadClicked.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.secondary2DAxisClick);
            }

            if (hand.trackpadTouched.isActive)
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.secondary2DAxisTouch);
            }

            __result = true;

            return false;
        }
    }

    /*
     * Support the following CommonUsages:
     * isTracked (bool)
     * primaryButton (bool)
     * primaryTouch (bool)
     * secondaryButton (bool)
     * secondaryTouch (bool)
     * gripButton (bool)
     * triggerButton (bool)
     * menuButton (bool)
     * primary2DAxisClick (bool)
     * primary2DAxisTouch (bool)
     * secondary2DAxisClick (bool)
     * secondary2DAxisTouch (bool)
     */
    [HarmonyPatch(typeof(InputDevice), nameof(InputDevice.TryGetFeatureValue), new Type[] { typeof(InputFeatureUsage<bool>), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out })]
    [HarmonyPatchCategory(Plugin.kOpenVRLoaderHarmonyCategory)]
    internal static class InputDevice_TryGetFeatureValue_Boolean
    {
        public static bool Prefix(InputDevice __instance, InputFeatureUsage<bool> usage, ref bool value, ref bool __result)
        {
            if (!__instance.isValid || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.Controller) || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.HeldInHand))
            {
                return true;
            }

            UnityXRActionsHand hand;

            if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                hand = Plugin.unityXRActions.left;
            }
            else if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                hand = Plugin.unityXRActions.right;
            }
            else
            {
                return true;
            }

            if (usage == CommonUsages.isTracked)
            {
                value = hand.devicePose.isTracking;
            }
            else if (usage == CommonUsages.primaryButton)
            {
                value = hand.primaryButton.state;
            }
            else if (usage == CommonUsages.primaryTouch)
            {
                value = hand.primaryTouched.state;
            }
            else if (usage == CommonUsages.secondaryButton)
            {
                value = hand.secondaryButton.state;
            }
            else if (usage == CommonUsages.secondaryTouch)
            {
                value = hand.secondaryTouched.state;
            }
            else if (usage == CommonUsages.gripButton)
            {
                value = hand.gripPressed.state;
            }
            else if (usage == CommonUsages.triggerButton)
            {
                value = hand.triggerPressed.state;
            }
            else if (usage == CommonUsages.menuButton)
            {
                value = hand.menu.state;
            }
            else if (usage == CommonUsages.primary2DAxisClick)
            {
                value = hand.thumbstickClicked.state;
            }
            else if (usage == CommonUsages.primary2DAxisTouch)
            {
                value = hand.thumbstickTouched.state;
            }
            else if (usage == CommonUsages.secondary2DAxisClick)
            {
                value = hand.trackpadClicked.state;
            }
            else if (usage == CommonUsages.secondary2DAxisTouch)
            {
                value = hand.trackpadTouched.state;
            }
            else
            {
                return true;
            }

            __result = true;

            return false;
        }
    }

    /*
     * Support the following CommonUsages:
     * trigger (float)
     * grip (float)
     */
    [HarmonyPatch(typeof(InputDevice), nameof(InputDevice.TryGetFeatureValue), new Type[] { typeof(InputFeatureUsage<float>), typeof(float) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out })]
    [HarmonyPatchCategory(Plugin.kOpenVRLoaderHarmonyCategory)]
    internal static class InputDevice_TryGetFeatureValue_Float
    {
        public static bool Prefix(InputDevice __instance, InputFeatureUsage<float> usage, ref float value, ref bool __result)
        {
            if (!__instance.isValid || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.Controller) || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.HeldInHand))
            {
                return true;
            }

            UnityXRActionsHand hand;

            if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                hand = Plugin.unityXRActions.left;
            }
            else if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                hand = Plugin.unityXRActions.right;
            }
            else
            {
                return true;
            }

            if (usage == CommonUsages.trigger)
            {
                value = hand.trigger.value;
            }
            else if (usage == CommonUsages.grip)
            {
                value = hand.grip.value;
            }
            else
            {
                return true;
            }

            __result = true;

            return false;
        }
    }

    /*
     * Support the following CommonUsages:
     * primary2DAxis (Vector2)
     * secondary2DAxis (Vector2)
     */
    [HarmonyPatch(typeof(InputDevice), nameof(InputDevice.TryGetFeatureValue), new Type[] { typeof(InputFeatureUsage<Vector2>), typeof(Vector2) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out })]
    [HarmonyPatchCategory(Plugin.kOpenVRLoaderHarmonyCategory)]
    internal static class InputDevice_TryGetFeatureValue_Vector2
    {
        public static bool Prefix(InputDevice __instance, InputFeatureUsage<Vector2> usage, ref Vector2 value, ref bool __result)
        {
            if (!__instance.isValid || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.Controller) || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.HeldInHand))
            {
                return true;
            }

            UnityXRActionsHand hand;

            if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                hand = Plugin.unityXRActions.left;
            }
            else if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                hand = Plugin.unityXRActions.right;
            }
            else
            {
                return true;
            }

            if (usage == CommonUsages.primary2DAxis)
            {
                value = hand.thumbstick.vector;
            }
            else if (usage == CommonUsages.secondary2DAxis)
            {
                value = hand.trackpad.vector;
            }
            else
            {
                return true;
            }

            __result = true;

            return false;
        }
    }

    /*
     * Support the following CommonUsages:
     * devicePosition (Vector3)
     * deviceVelocity (Vector3)
     * deviceAngularVelocity (Vector3)
     */
    [HarmonyPatch(typeof(InputDevice), nameof(InputDevice.TryGetFeatureValue), new Type[] { typeof(InputFeatureUsage<Vector3>), typeof(Vector3) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out })]
    [HarmonyPatchCategory(Plugin.kOpenVRLoaderHarmonyCategory)]
    internal static class InputDevice_TryGetFeatureValue_Vector3
    {
        public static bool Prefix(InputDevice __instance, InputFeatureUsage<Vector3> usage, ref Vector3 value, ref bool __result)
        {
            if (!__instance.isValid || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.Controller) || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.HeldInHand))
            {
                return true;
            }

            UnityXRActionsHand hand;

            if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                hand = Plugin.unityXRActions.left;
            }
            else if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                hand = Plugin.unityXRActions.right;
            }
            else
            {
                return true;
            }

            if (usage == CommonUsages.devicePosition)
            {
                value = hand.devicePose.position;
            }
            else if (usage == CommonUsages.deviceVelocity)
            {
                value = hand.devicePose.velocity;
            }
            else if (usage == CommonUsages.deviceAngularVelocity)
            {
                value = hand.devicePose.angularVelocity;
            }
            else
            {
                return true;
            }

            __result = true;

            return false;
        }
    }

    /*
     * Support the following CommonUsages:
     * deviceRotation (Quaternion)
     */
    [HarmonyPatch(typeof(InputDevice), nameof(InputDevice.TryGetFeatureValue), new Type[] { typeof(InputFeatureUsage<Quaternion>), typeof(Quaternion) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out })]
    [HarmonyPatchCategory(Plugin.kOpenVRLoaderHarmonyCategory)]
    internal static class InputDevice_TryGetFeatureValue_Quaternion
    {
        public static bool Prefix(InputDevice __instance, InputFeatureUsage<Quaternion> usage, ref Quaternion value, ref bool __result)
        {
            if (!__instance.isValid || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.Controller) || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.HeldInHand))
            {
                return true;
            }

            UnityXRActionsHand hand;

            if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                hand = Plugin.unityXRActions.left;
            }
            else if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                hand = Plugin.unityXRActions.right;
            }
            else
            {
                return true;
            }

            if (usage == CommonUsages.deviceRotation)
            {
                value = hand.devicePose.rotation;
            }
            else
            {
                return true;
            }

            __result = true;

            return false;
        }
    }

    [HarmonyPatch(typeof(InputDevice), nameof(InputDevice.SendHapticImpulse), new Type[] { typeof(uint), typeof(float), typeof(float) })]
    [HarmonyPatchCategory(Plugin.kOpenVRLoaderHarmonyCategory)]
    internal static class InputDevice_SendHapticImpulse
    {
        public static bool Prefix(InputDevice __instance, float amplitude, float duration, ref bool __result)
        {
            if (!__instance.isValid || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.Controller) || !__instance.characteristics.HasFlag(InputDeviceCharacteristics.HeldInHand))
            {
                return true;
            }

            UnityXRActionsHand hand;

            if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                hand = Plugin.unityXRActions.left;
            }
            else if (__instance.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                hand = Plugin.unityXRActions.right;
            }
            else
            {
                return true;
            }

            hand.haptics.TriggerHapticVibration(duration, amplitude);

            __result = true;

            return false;
        }
    }
}
