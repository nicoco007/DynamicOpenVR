// <copyright file="MainSystemInit.cs" company="Nicolas Gnyra">
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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine.XR;

namespace DynamicOpenVR.BeatSaber.HarmonyPatches
{
    internal static class MainSystemInit_InstallBindings
    {
        private static readonly MethodInfo kXRSettingsLoadedDeviceNameGetter = AccessTools.DeclaredPropertyGetter(typeof(XRSettings), nameof(XRSettings.loadedDeviceName));
        private static readonly MethodInfo kStringIndexOfMethod = AccessTools.DeclaredMethod(typeof(string), nameof(string.IndexOf), new Type[] { typeof(string), typeof(StringComparison) });

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            Label? label = default;

            return new CodeMatcher(instructions, ilGenerator)
                .MatchForward(
                    true,
                    new CodeMatch(i => i.Calls(kXRSettingsLoadedDeviceNameGetter)),
                    new CodeMatch(OpCodes.Ldstr, "OpenXR"),
                    new CodeMatch(OpCodes.Ldc_I4_5), // StringComparison.OrdinalIgnoreCase
                    new CodeMatch(i => i.Calls(kStringIndexOfMethod)),
                    new CodeMatch(OpCodes.Ldc_I4_0),
                    new CodeMatch(i => i.Branches(out label)))
                .ThrowIfInvalid("OpenXR string comparison not found")
                .CreateLabelWithOffsets(1, out Label label2)
                .SetAndAdvance(OpCodes.Bge_S, label2)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Call, kXRSettingsLoadedDeviceNameGetter),
                    new CodeInstruction(OpCodes.Ldstr, "OpenVR"),
                    new CodeInstruction(OpCodes.Ldc_I4_5),
                    new CodeInstruction(OpCodes.Callvirt, kStringIndexOfMethod),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Blt_S, label))
                .InstructionEnumeration();
        }
    }
}
