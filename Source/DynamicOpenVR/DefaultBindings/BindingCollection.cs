// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright © 2019-2020 Nicolas Gnyra

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace DynamicOpenVR.DefaultBindings
{
    internal class BindingCollection
    {
        [JsonProperty(PropertyName = "sources")]
        public List<SourceBinding> sources { get; set; } = new List<SourceBinding>();

        [JsonProperty(PropertyName = "haptics")]
        public List<HapticBinding> haptics { get; set; } = new List<HapticBinding>();

        [JsonProperty(PropertyName = "poses")]
        public List<PoseBinding> poses { get; set; } = new List<PoseBinding>();

        [JsonProperty(PropertyName = "skeleton")]
        public List<SkeletonBinding> skeleton { get; set; } = new List<SkeletonBinding>();

        [JsonProperty(PropertyName = "chords")]
        public List<ChordBinding> chords { get; set; } = new List<ChordBinding>();
    }
}
