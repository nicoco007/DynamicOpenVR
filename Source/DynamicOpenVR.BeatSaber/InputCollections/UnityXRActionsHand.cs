// <copyright file="UnityXRActionsHand.cs" company="Nicolas Gnyra">
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
    /// <summary>
    /// See <see href="https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.14/manual/input.html#mapping-between-openxr-paths-and-unity-bindings">Mapping between OpenXR paths and Unity bindings</see> for details.
    /// </summary>
    internal class UnityXRActionsHand : IDisposable
    {
        public BooleanInput system { get; init; }

        public BooleanInput systemTouched { get; init; }

        public BooleanInput select { get; init; }

        public BooleanInput menu { get; init; }

        public BooleanInput primaryButton { get; init; }

        public BooleanInput primaryTouched { get; init; }

        public BooleanInput secondaryButton { get; init; }

        public BooleanInput secondaryTouched { get; init; }

        public VectorInput grip { get; init; }

        public BooleanInput gripPressed { get; init; }

        public VectorInput gripForce { get; init; }

        public VectorInput trigger { get; init; }

        public BooleanInput triggerPressed { get; init; }

        public BooleanInput triggerTouched { get; init; }

        public Vector2Input thumbstick { get; init; }

        public BooleanInput thumbstickClicked { get; init; }

        public BooleanInput thumbstickTouched { get; init; }

        public Vector2Input trackpad { get; init; }

        public BooleanInput trackpadClicked { get; init; }

        public BooleanInput trackpadTouched { get; init; }

        public VectorInput trackpadForce { get; init; }

        public PoseInput devicePose { get; init; }

        public PoseInput pointer { get; init; }

        public SkeletalInput skeleton { get; init; }

        public HapticVibrationOutput haptics { get; init; }

        public void Dispose()
        {
            system?.Dispose();
            systemTouched?.Dispose();
            primaryButton?.Dispose();
            primaryTouched?.Dispose();
            secondaryButton?.Dispose();
            secondaryTouched?.Dispose();
            grip?.Dispose();
            gripPressed?.Dispose();
            gripForce?.Dispose();
            trigger?.Dispose();
            triggerPressed?.Dispose();
            triggerTouched?.Dispose();
            thumbstick?.Dispose();
            thumbstickClicked?.Dispose();
            thumbstickTouched?.Dispose();
            trackpad?.Dispose();
            trackpadTouched?.Dispose();
            trackpadForce?.Dispose();
            devicePose?.Dispose();
            pointer?.Dispose();
            skeleton?.Dispose();
            haptics?.Dispose();
        }
    }
}
