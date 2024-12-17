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
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;
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

        public FourCC format => new('O', 'V', 'R', 'C');
    }
}
