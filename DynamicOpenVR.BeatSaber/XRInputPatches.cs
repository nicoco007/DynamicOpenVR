// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019 Nicolas Gnyra

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

using Harmony;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
namespace DynamicOpenVR.BeatSaber
{
    [HarmonyPatch(typeof(InputTracking))]
    [HarmonyPatch("GetLocalPosition", MethodType.Normal)]
    class InputTrackingGetLocalPositionPatch
    {
        public static bool Prefix(XRNode node, ref Vector3 __result)
        {
            if (node == XRNode.LeftHand)
            {
                __result = Plugin.LeftHandPose.Pose.position;
                return false;
            }
            
            if (node == XRNode.RightHand)
            {
                __result = Plugin.RightHandPose.Pose.position;
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
                __result = Plugin.LeftHandPose.Pose.rotation;
                return false;
            }
            
            if (node == XRNode.RightHand)
            {
                __result = Plugin.RightHandPose.Pose.rotation;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InputTracking))]
    [HarmonyPatch("GetNodeStates", MethodType.Normal)]
    class InputTrackingGetNodeStatesPatch
    {
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
                            position = Plugin.LeftHandPose.Pose.position,
                            rotation = Plugin.LeftHandPose.Pose.rotation,
                            tracked = Plugin.LeftHandPose.IsTracking,
                            velocity = Plugin.LeftHandPose.Velocity,
                            angularVelocity = Plugin.LeftHandPose.AngularVelocity,
                            uniqueID = nodeState.uniqueID
                        });
                        break;

                    case XRNode.RightHand:
                        nodeStates.Remove(nodeState);
                        nodeStates.Add(new XRNodeState
                        {
                            nodeType = XRNode.RightHand,
                            position = Plugin.RightHandPose.Pose.position,
                            rotation = Plugin.RightHandPose.Pose.rotation,
                            tracked = Plugin.RightHandPose.IsTracking,
                            velocity = Plugin.RightHandPose.Velocity,
                            angularVelocity = Plugin.RightHandPose.AngularVelocity,
                            uniqueID = nodeState.uniqueID
                        });
                        break;
                }
            }
        }
    }
}
