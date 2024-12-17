// <copyright file="BeatSaberActions.cs" company="Nicolas Gnyra">
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
using DynamicOpenVR.IO;

namespace DynamicOpenVR.BeatSaber.InputCollections
{
    internal class BeatSaberActions : IDisposable
    {
        public VectorInput leftTrigger { get; init; }

        public VectorInput rightTrigger { get; init; }

        public BooleanInput leftMenuButton { get; init; }

        public BooleanInput rightMenuButton { get; init; }

        public HapticVibrationOutput leftHandHaptics { get; init; }

        public HapticVibrationOutput rightHandHaptics { get; init; }

        public PoseInput leftHandPose { get; init; }

        public PoseInput rightHandPose { get; init; }

        public Vector2Input leftThumbstick { get; init; }

        public Vector2Input rightThumbstick { get; init; }

        public BooleanInput headsetOnHead { get; init; }

        public void Dispose()
        {
            leftTrigger?.Dispose();
            rightTrigger?.Dispose();
            leftMenuButton?.Dispose();
            rightMenuButton?.Dispose();
            leftHandHaptics?.Dispose();
            rightHandHaptics?.Dispose();
            leftHandPose?.Dispose();
            rightHandPose?.Dispose();
            leftThumbstick?.Dispose();
            rightThumbstick?.Dispose();
            headsetOnHead?.Dispose();
        }
    }
}
