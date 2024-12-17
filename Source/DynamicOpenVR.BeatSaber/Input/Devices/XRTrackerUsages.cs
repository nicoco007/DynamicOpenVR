// <copyright file="XRTrackerUsages.cs" company="Nicolas Gnyra">
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

using UnityEngine.InputSystem.Utilities;

#pragma warning disable IDE1006
namespace DynamicOpenVR.BeatSaber.Input.Devices
{
    public class XRTrackerUsages
    {
        /// <summary>
        /// Device on left foot.
        /// </summary>
        public static readonly InternedString LeftFoot = new("LeftFoot");

        /// <summary>
        /// Device on right foot.
        /// </summary>
        public static readonly InternedString RightFoot = new("RightFoot");

        /// <summary>
        /// Device on left shoulder.
        /// </summary>
        public static readonly InternedString LeftShoulder = new("LeftShoulder");

        /// <summary>
        /// Device on right shoulder.
        /// </summary>
        public static readonly InternedString RightShoulder = new("RightShoulder");

        /// <summary>
        /// Device on left elbow.
        /// </summary>
        public static readonly InternedString LeftElbow = new("LeftElbow");

        /// <summary>
        /// Device on right elbow.
        /// </summary>
        public static readonly InternedString RightElbow = new("RightElbow");

        /// <summary>
        /// Device on left knee.
        /// </summary>
        public static readonly InternedString LeftKnee = new("LeftKnee");

        /// <summary>
        /// Device on right knee.
        /// </summary>
        public static readonly InternedString RightKnee = new("RightKnee");

        /// <summary>
        /// Device on left wrist.
        /// </summary>
        public static readonly InternedString LeftWrist = new("LeftWrist");

        /// <summary>
        /// Device on right wrist.
        /// </summary>
        public static readonly InternedString RightWrist = new("RightWrist");

        /// <summary>
        /// Device on left ankle.
        /// </summary>
        public static readonly InternedString LeftAnkle = new("LeftAnkle");

        /// <summary>
        /// Device on right ankle.
        /// </summary>
        public static readonly InternedString RightAnkle = new("RightAnkle");

        /// <summary>
        /// Device on waist.
        /// </summary>
        public static readonly InternedString Waist = new("Waist");

        /// <summary>
        /// Device on chest.
        /// </summary>
        public static readonly InternedString Chest = new("Chest");

        /// <summary>
        /// Device on a camera.
        /// </summary>
        public static readonly InternedString Camera = new("Camera");

        /// <summary>
        /// Device on a keyboard.
        /// </summary>
        public static readonly InternedString Keyboard = new("Keyboard");
    }
}
#pragma warning restore IDE1006
