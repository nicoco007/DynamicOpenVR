// <copyright file="InputTracking.cs" company="Nicolas Gnyra">
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
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace DynamicOpenVR.BeatSaber.HarmonyPatches
{
    [HarmonyPatch(typeof(InputTracking), nameof(InputTracking.GetLocalPosition))]
    internal class InputTracking_GetLocalPosition
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(XRNode node, ref Vector3 __result)
        {
            if (node == XRNode.LeftHand)
            {
                __result = Plugin.unityXRActions.left.pose.position;
                return false;
            }

            if (node == XRNode.RightHand)
            {
                __result = Plugin.unityXRActions.right.pose.position;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InputTracking), nameof(InputTracking.GetLocalRotation))]
    internal class InputTracking_GetLocalRotation
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(XRNode node, ref Quaternion __result)
        {
            if (node == XRNode.LeftHand)
            {
                __result = Plugin.unityXRActions.left.pose.rotation;
                return false;
            }

            if (node == XRNode.RightHand)
            {
                __result = Plugin.unityXRActions.right.pose.rotation;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InputTracking), nameof(InputTracking.GetNodeStates))]
    internal class InputTracking_GetNodeStates
    {
        [HarmonyPriority(Priority.First)]
        public static void Postfix(List<XRNodeState> nodeStates)
        {
            for (int i = 0; i < nodeStates.Count; ++i)
            {
                XRNodeState nodeState = nodeStates[i];

                switch (nodeStates[i].nodeType)
                {
                    case XRNode.LeftHand:
                        nodeState.position = Plugin.unityXRActions.left.pose.position;
                        nodeState.rotation = Plugin.unityXRActions.left.pose.rotation;
                        nodeState.tracked = Plugin.unityXRActions.left.pose.isTracking;
                        nodeState.velocity = Plugin.unityXRActions.left.pose.velocity;
                        nodeState.angularVelocity = Plugin.unityXRActions.left.pose.angularVelocity;
                        nodeState.acceleration = Vector3.zero;
                        nodeState.angularAcceleration = Vector3.zero;
                        break;

                    case XRNode.RightHand:
                        nodeState.position = Plugin.unityXRActions.right.pose.position;
                        nodeState.rotation = Plugin.unityXRActions.right.pose.rotation;
                        nodeState.tracked = Plugin.unityXRActions.right.pose.isTracking;
                        nodeState.velocity = Plugin.unityXRActions.right.pose.velocity;
                        nodeState.angularVelocity = Plugin.unityXRActions.right.pose.angularVelocity;
                        nodeState.acceleration = Vector3.zero;
                        nodeState.angularAcceleration = Vector3.zero;
                        break;
                }

                nodeStates[i] = nodeState;
            }
        }
    }
}
