// <copyright file="PCAppInit.cs" company="Nicolas Gnyra">
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
using HarmonyLib;
using UnityEngine.XR;

namespace DynamicOpenVR.BeatSaber.HarmonyPatches
{
    [HarmonyPatch(typeof(PCAppInit), nameof(PCAppInit.TransitionToNextScene))]
    internal static class PCAppInit_TransitionToNextScene
    {
        private static readonly MethodInfo kStringIsNullOrEmptyMethod = AccessTools.DeclaredMethod(typeof(string), nameof(string.IsNullOrEmpty), new[] { typeof(string) });
        private static readonly MethodInfo kIsOpenXRAndNullOrEmptyMethod = AccessTools.DeclaredMethod(typeof(PCAppInit_TransitionToNextScene), nameof(IsOpenXRAndNullOrEmpty));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions).MatchForward(false, new CodeMatch(i => i.Calls(kStringIsNullOrEmptyMethod))).SetOperandAndAdvance(kIsOpenXRAndNullOrEmptyMethod).InstructionEnumeration();
        }

        private static bool IsOpenXRAndNullOrEmpty(string xrRuntimeName)
        {
            return XRSettings.loadedDeviceName.IndexOf("OpenXR", StringComparison.OrdinalIgnoreCase) >= 0 && string.IsNullOrEmpty(xrRuntimeName);
        }
    }
}
