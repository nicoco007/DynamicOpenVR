// <copyright file="Input.cs" company="Nicolas Gnyra">
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

using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace DynamicOpenVR.BeatSaber.HarmonyPatches
{
#pragma warning disable IDE0065, SA1200
    using Input = UnityEngine.Input;
#pragma warning restore IDE0065, SA1200

    [HarmonyPatch]
    [HarmonyPatchCategory(Plugin.kOpenVRLoaderHarmonyCategory)]
    internal static class Input_GetAxis
    {
        public static bool Prefix(string axisName, ref float __result)
        {
            switch (axisName)
            {
                case "TriggerLeftHand":
                    __result = Plugin.unityXRActions.left.trigger.value;
                    break;

                case "TriggerRightHand":
                    __result = Plugin.unityXRActions.right.trigger.value;
                    break;

                case "HorizontalLeftHand":
                case "Oculus_CrossPlatform_SecondaryThumbstickHorizontal":
                    __result = Plugin.unityXRActions.left.primary2DAxis.vector.x;
                    break;

                case "HorizontalRightHand":
                case "Oculus_CrossPlatform_PrimaryThumbstickHorizontal":
                    __result = Plugin.unityXRActions.right.primary2DAxis.vector.x;
                    break;

                case "VerticalLeftHand":
                case "Oculus_CrossPlatform_SecondaryThumbstickVertical":
                    __result = Plugin.unityXRActions.left.primary2DAxis.vector.y;
                    break;

                case "VerticalRightHand":
                case "Oculus_CrossPlatform_PrimaryThumbstickVertical":
                    __result = Plugin.unityXRActions.right.primary2DAxis.vector.y;
                    break;

                default:
                    return true;
            }

            return false;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Input), nameof(Input.GetAxis));
            yield return AccessTools.Method(typeof(Input), nameof(Input.GetAxisRaw));
        }
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetButtonDown))]
    [HarmonyPatchCategory(Plugin.kOpenVRLoaderHarmonyCategory)]
    internal static class Input_GetButtonDown
    {
        public static bool Prefix(string buttonName, ref bool __result)
        {
            switch (buttonName)
            {
                case "MenuButtonLeftHand":
                    __result = Plugin.unityXRActions.left.secondaryButton.enabledChange;
                    break;

                case "MenuButtonRightHand":
                    __result = Plugin.unityXRActions.right.secondaryButton.enabledChange;
                    break;

                default:
                    return true;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetButton))]
    [HarmonyPatchCategory(Plugin.kOpenVRLoaderHarmonyCategory)]
    internal static class Input_GetButton
    {
        public static bool Prefix(string buttonName, ref bool __result)
        {
            switch (buttonName)
            {
                case "MenuButtonLeftHand":
                    __result = Plugin.unityXRActions.left.secondaryButton.state;
                    break;

                case "MenuButtonRightHand":
                    __result = Plugin.unityXRActions.right.secondaryButton.state;
                    break;

                default:
                    return true;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetButtonUp))]
    [HarmonyPatchCategory(Plugin.kOpenVRLoaderHarmonyCategory)]
    internal static class Input_GetButtonUp
    {
        public static bool Prefix(string buttonName, ref bool __result)
        {
            switch (buttonName)
            {
                case "MenuButtonLeftHand":
                    __result = Plugin.unityXRActions.left.secondaryButton.disabledChange;
                    break;

                case "MenuButtonRightHand":
                    __result = Plugin.unityXRActions.right.secondaryButton.disabledChange;
                    break;

                default:
                    return true;
            }

            return false;
        }
    }
}
