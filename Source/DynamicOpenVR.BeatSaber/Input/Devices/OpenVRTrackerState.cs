// <copyright file="OpenVRTrackerState.cs" company="Nicolas Gnyra">
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

using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace DynamicOpenVR.BeatSaber.Input.Devices
{
    internal struct OpenVRTrackerState : IInputStateTypeInfo
    {
        [InputControl(layout = "Integer")]
        public int trackingState; // TODO: this should be an enum

        [InputControl(layout = "Button")]
        public bool isTracked;

        [InputControl(layout = "Vector3")]
        public Vector3 devicePosition;

        [InputControl(layout = "Quaternion")]
        public Quaternion deviceRotation;

        public FourCC format => new('O', 'V', 'R', 'T');
    }
}
