// <copyright file="SkeletalInput.cs" company="Nicolas Gnyra">
// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
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

using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace DynamicOpenVR.IO
{
    public class SkeletalInput : PoseInput
    {
        private readonly bool _onlyUpdateSummaryData;

        private VRBoneTransform_t[] _boneData;
        private BoneTransform[] _bones;
        private InputSkeletalActionData_t _actionData;

        public SkeletalInput(string name, bool onlyUpdateSummaryData = true)
            : base(name)
        {
            _onlyUpdateSummaryData = onlyUpdateSummaryData;
        }

        /// <inheritdoc/>
        public override bool isActive => _actionData.bActive;

        /// <summary>
        /// Gets the bone transforms, all relative to <see cref="PoseInput.pose"/>.
        /// </summary>
        public IReadOnlyList<BoneTransform> bones => _bones;

        /// <summary>
        /// Gets the summary data of the skeleton (finger curl and splay).
        /// </summary>
        public SkeletalSummaryData summaryData { get; private set; }

        internal override void Initialize()
        {
            base.Initialize();

            if (!_onlyUpdateSummaryData)
            {
                uint count = OpenVRFacade.GetBoneCount(handle);

                _boneData = new VRBoneTransform_t[count];
                _bones = new BoneTransform[count];
            }
        }

        /// <inheritdoc/>
        internal override void UpdateData()
        {
            base.UpdateData();

            _actionData = OpenVRFacade.GetSkeletalActionData(handle);
            summaryData = new SkeletalSummaryData(OpenVRFacade.GetSkeletalSummaryData(handle));

            if (!_onlyUpdateSummaryData)
            {
                OpenVRFacade.GetSkeletalBoneData(handle, _boneData);

                for (int i = 0; i < _boneData.Length; i++)
                {
                    _bones[i] = new BoneTransform(_boneData[i]);
                }
            }
        }

        public readonly struct BoneTransform
        {
            internal BoneTransform(VRBoneTransform_t rawBoneTransform)
            {
                position = new Vector3(rawBoneTransform.position.v0, rawBoneTransform.position.v1, -rawBoneTransform.position.v2);
                rotation = new Quaternion(-rawBoneTransform.orientation.x, -rawBoneTransform.orientation.y, rawBoneTransform.orientation.z, rawBoneTransform.orientation.w);
            }

            public Vector3 position { get; }

            public Quaternion rotation { get; }
        }
    }
}
