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
    internal class UnityXRActionsHand : IDisposable
    {
        public PoseInput pose { get; init; }

        public BooleanInput primaryButton { get; init; }

        public BooleanInput primaryTouch { get; init; }

        public BooleanInput secondaryButton { get; init; }

        public BooleanInput secondaryTouch { get; init; }

        public VectorInput grip { get; init; }

        public BooleanInput gripButton { get; init; }

        public VectorInput trigger { get; init; }

        public BooleanInput triggerButton { get; init; }

        public BooleanInput menuButton { get; init; }

        public Vector2Input primary2DAxis { get; init; }

        public BooleanInput primary2DAxisClick { get; init; }

        public BooleanInput primary2DAxisTouch { get; init; }

        public SkeletalInput skeleton { get; init; }

        public HapticVibrationOutput haptics { get; init; }

        public void Dispose()
        {
            pose?.Dispose();
            primaryButton?.Dispose();
            primaryTouch?.Dispose();
            secondaryButton?.Dispose();
            secondaryTouch?.Dispose();
            grip?.Dispose();
            gripButton?.Dispose();
            trigger?.Dispose();
            triggerButton?.Dispose();
            menuButton?.Dispose();
            primary2DAxis?.Dispose();
            primary2DAxisClick?.Dispose();
            primary2DAxisTouch?.Dispose();
            haptics?.Dispose();
        }
    }
}
