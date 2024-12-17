// <copyright file="OpenVRInputHMDState.cs" company="Nicolas Gnyra">
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
using UnityEngine.XR;

namespace DynamicOpenVR.BeatSaber.Input.Devices
{
    internal struct OpenVRInputHMDState : IInputStateTypeInfo
    {
        [InputControl(layout = "Integer")]
        public InputTrackingState trackingState;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button")]
        public bool isTracked;

        [InputControl(layout = "Vector3")]
        public Vector3 devicePosition;

        [InputControl(layout = "Quaternion")]
        public Quaternion deviceRotation;

        [InputControl(layout = "Vector3")]
        public Vector3 centerEyePosition;

        [InputControl(layout = "Quaternion")]
        public Quaternion centerEyeRotation;

        [InputControl(layout = "Vector3")]
        public Vector3 leftEyePosition;

        [InputControl(layout = "Quaternion")]
        public Quaternion leftEyeRotation;

        [InputControl(layout = "Vector3")]
        public Vector3 rightEyePosition;

        [InputControl(layout = "Quaternion")]
        public Quaternion rightEyeRotation;

        [MarshalAs(UnmanagedType.I1)]
        [InputControl(layout = "Button")]
        public bool userPresence;

        public FourCC format => new('O', 'V', 'R', 'H');
    }
}
