// <copyright file="XRInputPatches.cs" company="Nicolas Gnyra">
// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2021 Nicolas Gnyra
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
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace DynamicOpenVR.BeatSaber.HarmonyPatches
{
    [HarmonyPatch(typeof(InputTracking))]
    [HarmonyPatch("GetLocalPosition", MethodType.Normal)]
    internal class InputTracking_GetLocalPosition
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(XRNode node, ref Vector3 __result)
        {
            if (node == XRNode.LeftHand)
            {
                __result = Plugin.leftHandPose.pose.position;
                return false;
            }

            if (node == XRNode.RightHand)
            {
                __result = Plugin.rightHandPose.pose.position;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InputTracking))]
    [HarmonyPatch("GetLocalRotation", MethodType.Normal)]
    internal class InputTracking_GetLocalRotation
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(XRNode node, ref Quaternion __result)
        {
            if (node == XRNode.LeftHand)
            {
                __result = Plugin.leftHandPose.pose.rotation;
                return false;
            }

            if (node == XRNode.RightHand)
            {
                __result = Plugin.rightHandPose.pose.rotation;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InputTracking))]
    [HarmonyPatch("GetNodeStates", MethodType.Normal)]
    internal class InputTracking_GetNodeStates
    {
        [HarmonyPriority(Priority.First)]
        public static void Postfix(List<XRNodeState> nodeStates)
        {
            foreach (XRNodeState nodeState in nodeStates.ToList())
            {
                switch (nodeState.nodeType)
                {
                    case XRNode.LeftHand:
                        nodeStates.Remove(nodeState);
                        nodeStates.Add(new XRNodeState()
                        {
                            nodeType = XRNode.LeftHand,
                            position = Plugin.leftHandPose.pose.position,
                            rotation = Plugin.leftHandPose.pose.rotation,
                            tracked = Plugin.leftHandPose.isTracking,
                            velocity = Plugin.leftHandPose.velocity,
                            angularVelocity = Plugin.leftHandPose.angularVelocity,
                            uniqueID = nodeState.uniqueID,
                        });
                        break;

                    case XRNode.RightHand:
                        nodeStates.Remove(nodeState);
                        nodeStates.Add(new XRNodeState
                        {
                            nodeType = XRNode.RightHand,
                            position = Plugin.rightHandPose.pose.position,
                            rotation = Plugin.rightHandPose.pose.rotation,
                            tracked = Plugin.rightHandPose.isTracking,
                            velocity = Plugin.rightHandPose.velocity,
                            angularVelocity = Plugin.rightHandPose.angularVelocity,
                            uniqueID = nodeState.uniqueID,
                        });
                        break;
                }
            }
        }
    }
}
