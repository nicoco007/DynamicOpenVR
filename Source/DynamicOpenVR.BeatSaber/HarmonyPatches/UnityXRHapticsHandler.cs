// <copyright file="UnityXRHapticsHandler.cs" company="Nicolas Gnyra">
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

using System.Collections.Generic;
using System.Reflection;
using DynamicOpenVR.IO;
using HarmonyLib;
using UnityEngine.XR;

namespace DynamicOpenVR.BeatSaber.HarmonyPatches
{
    internal static class UnityXRHapticsHandler
    {
        private static readonly MethodInfo kTargetMethod = AccessTools.Method(typeof(InputDevice), nameof(InputDevice.SendHapticImpulse));
        private static readonly MethodInfo kOverrideMethod = AccessTools.Method(typeof(UnityXRHapticsHandler), nameof(Handle));

        private static bool Handle(ref InputDevice instance, uint channel, float amplitude, float duration)
        {
            if (!instance.isValid || !instance.characteristics.HasFlag(InputDeviceCharacteristics.Controller) || !instance.characteristics.HasFlag(InputDeviceCharacteristics.HeldInHand))
            {
                return false;
            }

            HapticVibrationOutput output;

            if (instance.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                output = Plugin.beatSaberActions.leftHandHaptics;
            }
            else if (instance.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                output = Plugin.beatSaberActions.rightHandHaptics;
            }
            else
            {
                return true;
            }

            output.TriggerHapticVibration(duration, amplitude);

            return true;
        }

        [HarmonyPatch(typeof(UnityXRController), nameof(UnityXRController.UpdateHapticsHandler))]
        internal static class UnityXRController_UpdateHapticsHandler
        {
            // don't use KnucklesUnityXRHapticsHandler
            public static bool Prefix() => false;
        }

        [HarmonyPatch(typeof(DefaultUnityXRHapticsHandler), nameof(DefaultUnityXRHapticsHandler.TriggerHapticPulse))]
        internal static class DefaultUnityXRHapticsHandler_TriggerHapticPulse
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchForward(true, new CodeMatch(i => i.Calls(kTargetMethod)))
                    .ThrowIfInvalid("InputDevice.SendHapticImpulse call not found")
                    .SetOperandAndAdvance(kOverrideMethod)
                    .InstructionEnumeration();
            }
        }
    }
}
