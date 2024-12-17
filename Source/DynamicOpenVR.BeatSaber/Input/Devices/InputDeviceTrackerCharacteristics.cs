// <copyright file="InputDeviceTrackerCharacteristics.cs" company="Nicolas Gnyra">
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

using System;

namespace DynamicOpenVR.BeatSaber.Input.Devices
{
    [Flags]
    public enum InputDeviceTrackerCharacteristics : uint
    {
        /// <summary>
        /// The user's left foot.
        /// </summary>
        TrackerLeftFoot = 0x1000u,

        /// <summary>
        /// The user's right foot.
        /// </summary>
        TrackerRightFoot = 0x2000u,

        /// <summary>
        /// The user's left shoulder.
        /// </summary>
        TrackerLeftShoulder = 0x4000u,

        /// <summary>
        /// The user's right shoulder.
        /// </summary>
        TrackerRightShoulder = 0x8000u,

        /// <summary>
        /// The user's left elbow.
        /// </summary>
        TrackerLeftElbow = 0x10000u,

        /// <summary>
        /// The user's right elbow.
        /// </summary>
        TrackerRightElbow = 0x20000u,

        /// <summary>
        /// The user's left knee.
        /// </summary>
        TrackerLeftKnee = 0x40000u,

        /// <summary>
        /// The user's right knee.
        /// </summary>
        TrackerRightKnee = 0x80000u,

        /// <summary>
        /// The user's left wrist.
        /// </summary>
        TrackerLeftWrist = 0x100000u,

        /// <summary>
        /// The user's right wrist.
        /// </summary>
        TrackerRightWrist = 0x200000u,

        /// <summary>
        /// The user's left ankle.
        /// </summary>
        TrackerLeftAnkle = 0x400000u,

        /// <summary>
        /// The user's right ankle.
        /// </summary>
        TrackerRightAnkle = 0x800000u,

        /// <summary>
        /// The user's waist.
        /// </summary>
        TrackerWaist = 0x1000000u,

        /// <summary>
        /// The user's chest.
        /// </summary>
        TrackerChest = 0x2000000u,

        /// <summary>
        /// A camera.
        /// </summary>
        TrackerCamera = 0x4000000u,

        /// <summary>
        /// A keyboard.
        /// </summary>
        TrackerKeyboard = 0x8000000u,
    }
}
