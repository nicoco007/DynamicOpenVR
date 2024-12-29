// <copyright file="OpenVRInputControllerState.cs" company="Nicolas Gnyra">
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

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR;

namespace DynamicOpenVR.BeatSaber.Input.Devices
{
    internal struct OpenVRInputControllerState : IInputStateTypeInfo
    {
        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", alias = "systemButton", usage = "MenuButton")]
        public bool system;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", usage = "MenuTouch")]
        public bool systemTouched;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", aliases = new[] { "SelectButton" }, usage = "PrimaryButton")]
        public bool select;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", aliases = new[] { "MenuButton" }, usage = "MenuButton")]
        public bool menu;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", usage = "PrimaryButton")]
        public bool primaryButton;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", usage = "PrimaryTouch")]
        public bool primaryTouched;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", usage = "SecondaryButton")]
        public bool secondaryButton;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", usage = "SecondaryTouch")]
        public bool secondaryTouched;

        [InputControl(layout = "Axis", aliases = new string[] { "GripAxis", "squeeze" }, usage = "Grip")]
        public float grip;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", aliases = new string[] { "GripButton", "squeezeClicked" }, usage = "GripButton")]
        public bool gripPressed;

        [InputControl(layout = "Axis", alias = "squeezeForce", usage = "GripForce")]
        public float gripForce;

        [InputControl(layout = "Axis", usage = "Trigger")]
        public float trigger;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", usage = "TriggerButton")]
        public bool triggerPressed;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", usage = "TriggerTouch")]
        public bool triggerTouched;

        [InputControl(layout = "Vector2", aliases = new string[] { "joystick", "Primary2DAxis" }, usage = "Primary2DAxis")]
        public Vector2 thumbstick;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", alias = "joystickClicked", usage = "Primary2DAxisClick")]
        public bool thumbstickClicked;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", alias = "joystickTouched", usage = "Primary2DAxisTouch")]
        public bool thumbstickTouched;

        [InputControl(layout = "Vector2", aliases = new string[] { "touchpad", "Secondary2DAxis" }, usage = "Secondary2DAxis")]
        public Vector2 trackpad;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", alias = "touchpadTouched", usage = "Secondary2DAxisClick")]
        public bool trackpadClicked;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", alias = "touchpadTouched", usage = "Secondary2DAxisTouch")]
        public bool trackpadTouched;

        [InputControl(layout = "Axis", alias = "touchpadForce", usage = "Secondary2DAxisForce")]
        public float trackpadForce;

        [InputControl(layout = "Pose", aliases = new string[] { "device", "gripPose" }, usage = "Device")]
        public PoseState devicePose;

        [InputControl(layout = "Pose", alias = "aimPose", usage = "Pointer")]
        public PoseState pointer;

        // trackingState and isTracked MUST be below PoseStates or else their offsets in the struct get overwritten
        [InputControl(layout = "Integer", usage = "IsTracked")]
        public InputTrackingState trackingState;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button", usage = "TrackingState")]
        public bool isTracked;

        [InputControl(layout = "Vector3", alias = "gripPosition")]
        public Vector3 devicePosition;

        [InputControl(layout = "Quaternion", alias = "gripOrientation")]
        public Quaternion deviceRotation;

        [InputControl(layout = "Vector3")]
        public Vector3 pointerPosition;

        [InputControl(layout = "Quaternion", alias = "pointerOrientation")]
        public Quaternion pointerRotation;

        public readonly FourCC format => new('O', 'V', 'R', 'C');

        /// <summary>
        /// This is the same as <see cref="UnityEngine.InputSystem.XR.PoseState"/> but with <see cref="isTracked"/> as a <see cref="byte"/> rather
        /// than a <see cref="bool"/> so it gets read properly by <see cref="UnityEngine.InputSystem.XR.PoseControl"/>, in which it is read as an
        /// unsigned byte (due to <c>sizeInBits = 8u</c>) rather than as a single bit (the default for <see cref="bool"/> values in a struct).
        /// </summary>
        /// <remarks>
        /// The struct size, field names, and field offsets are identical to <see cref="UnityEngine.InputSystem.XR.PoseState" />.
        /// </remarks>
        [StructLayout(LayoutKind.Explicit, Size = 60)]
        public readonly struct PoseState : IInputStateTypeInfo
        {
            [FieldOffset(0)]
            public readonly byte isTracked;

            [FieldOffset(4)]
            public readonly InputTrackingState trackingState;

            [FieldOffset(8)]
            public readonly Vector3 position;

            [FieldOffset(20)]
            public readonly Quaternion rotation;

            [FieldOffset(36)]
            public readonly Vector3 velocity;

            [FieldOffset(48)]
            public readonly Vector3 angularVelocity;

            public PoseState(bool isTracked, InputTrackingState trackingState, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
            {
                this.isTracked = isTracked ? byte.MaxValue : byte.MinValue;
                this.trackingState = trackingState;
                this.position = position;
                this.rotation = rotation;
                this.velocity = velocity;
                this.angularVelocity = angularVelocity;
            }

            public FourCC format => new('P', 'o', 's', 'e');
        }
    }
}
