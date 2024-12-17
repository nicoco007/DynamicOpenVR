// <copyright file="OpenVRHandProvider.cs" company="Nicolas Gnyra">
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
using DynamicOpenVR.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;

namespace DynamicOpenVR.BeatSaber.Input
{
    internal class OpenVRHandProvider : XRHandSubsystemProvider
    {
        private bool _isValid;

        public static string id { get; } = "OpenVR Hands";

        public override void GetHandLayout(NativeArray<bool> handJointsInLayout)
        {
            handJointsInLayout[XRHandJointID.Wrist.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.Palm.ToIndex()] = false;
            handJointsInLayout[XRHandJointID.ThumbMetacarpal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.ThumbProximal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.ThumbDistal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.ThumbTip.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.IndexMetacarpal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.IndexProximal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.IndexIntermediate.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.IndexDistal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.IndexTip.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.MiddleMetacarpal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.MiddleProximal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.MiddleIntermediate.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.MiddleDistal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.MiddleTip.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.RingMetacarpal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.RingProximal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.RingIntermediate.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.RingDistal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.RingTip.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.LittleMetacarpal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.LittleProximal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.LittleIntermediate.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.LittleDistal.ToIndex()] = true;
            handJointsInLayout[XRHandJointID.LittleTip.ToIndex()] = true;

            _isValid = true;
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }

        public override void Destroy()
        {
        }

        public override XRHandSubsystem.UpdateSuccessFlags TryUpdateHands(XRHandSubsystem.UpdateType updateType, ref Pose leftHandRootPose, NativeArray<XRHandJoint> leftHandJoints, ref Pose rightHandRootPose, NativeArray<XRHandJoint> rightHandJoints)
        {
            if (!_isValid)
            {
                return XRHandSubsystem.UpdateSuccessFlags.None;
            }

            XRHandSubsystem.UpdateSuccessFlags result = XRHandSubsystem.UpdateSuccessFlags.None;

            result |= SetRootPose(Plugin.unityXRActions.left.skeleton, ref leftHandRootPose, XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose);
            result |= SetRootPose(Plugin.unityXRActions.right.skeleton, ref rightHandRootPose, XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose);

            result |= PopulateHand(Handedness.Left, Plugin.unityXRActions.left.skeleton, leftHandRootPose, leftHandJoints, XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints);
            result |= PopulateHand(Handedness.Right, Plugin.unityXRActions.right.skeleton, rightHandRootPose, rightHandJoints, XRHandSubsystem.UpdateSuccessFlags.RightHandJoints);

            return result;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        internal static void Register()
        {
            XRHandSubsystemDescriptor.Cinfo cinfo = new()
            {
                id = id,
                providerType = typeof(OpenVRHandProvider),
            };
            XRHandSubsystemDescriptor.Register(cinfo);
        }

        private XRHandSubsystem.UpdateSuccessFlags SetRootPose(SkeletalInput input, ref Pose rootPose, XRHandSubsystem.UpdateSuccessFlags successFlags)
        {
            if (!input.isActive || !input.isTracking)
            {
                return XRHandSubsystem.UpdateSuccessFlags.None;
            }

            rootPose = input.pose;

            return successFlags;
        }

        private XRHandSubsystem.UpdateSuccessFlags PopulateHand(Handedness handedness, SkeletalInput input, Pose rootPose, NativeArray<XRHandJoint> joints, XRHandSubsystem.UpdateSuccessFlags successFlags)
        {
            if (!input.isActive)
            {
                return XRHandSubsystem.UpdateSuccessFlags.None;
            }

            IReadOnlyList<SkeletalInput.BoneTransform> bones = input.bones;

            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.Wrist, HandSkeletonBone.Wrist);

            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.ThumbMetacarpal, HandSkeletonBone.ThumbMetacarpal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.ThumbProximal, HandSkeletonBone.ThumbProximal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.ThumbDistal, HandSkeletonBone.ThumbDistal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.ThumbTip, HandSkeletonBone.ThumbTip);

            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.IndexMetacarpal, HandSkeletonBone.IndexMetacarpal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.IndexProximal, HandSkeletonBone.IndexProximal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.IndexIntermediate, HandSkeletonBone.IndexIntermediate);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.IndexDistal, HandSkeletonBone.IndexDistal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.IndexTip, HandSkeletonBone.IndexTip);

            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.MiddleMetacarpal, HandSkeletonBone.MiddleMetacarpal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.MiddleProximal, HandSkeletonBone.MiddleProximal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.MiddleIntermediate, HandSkeletonBone.MiddleIntermediate);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.MiddleDistal, HandSkeletonBone.MiddleDistal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.MiddleTip, HandSkeletonBone.MiddleTip);

            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.RingMetacarpal, HandSkeletonBone.RingMetacarpal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.RingProximal, HandSkeletonBone.RingProximal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.RingIntermediate, HandSkeletonBone.RingIntermediate);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.RingDistal, HandSkeletonBone.RingDistal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.RingTip, HandSkeletonBone.RingTip);

            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.LittleMetacarpal, HandSkeletonBone.LittleMetacarpal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.LittleProximal, HandSkeletonBone.LittleProximal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.LittleIntermediate, HandSkeletonBone.LittleIntermediate);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.LittleDistal, HandSkeletonBone.LittleDistal);
            SetJoint(joints, bones, rootPose, handedness, XRHandJointID.LittleTip, HandSkeletonBone.LittleTip);

            return successFlags;
        }

        private void SetJoint(NativeArray<XRHandJoint> joints, IReadOnlyList<SkeletalInput.BoneTransform> bones, Pose rootPose, Handedness handedness, XRHandJointID handJoint, HandSkeletonBone bone)
        {
            joints[handJoint.ToIndex()] = new XRHandJoint()
            {
                m_IdAndHandedness = IdAndHandedness(handJoint, handedness),
                m_Pose = GetPose(bones[(int)bone]).GetTransformedBy(rootPose),
                m_TrackingState = XRHandJointTrackingState.Pose,
            };
        }

        private int IdAndHandedness(XRHandJointID handJoint, Handedness handedness)
        {
            // Handedness is encoded in the most significant bit; 0 is left, 1 is right.
            return (handedness == Handedness.Left ? 0 : int.MinValue) + (int)handJoint;
        }

        private Pose GetPose(SkeletalInput.BoneTransform boneTransform)
        {
            // Rotate to match OpenXR's standard joint orientation.
            return new Pose(boneTransform.position, boneTransform.rotation * Quaternion.Euler(0, 90, 180));
        }
    }
}
