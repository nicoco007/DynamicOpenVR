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

using Newtonsoft.Json;
using System.Collections.Generic;

namespace DynamicOpenVR.Manifest
{
    internal class ActionManifest
    {
        [JsonProperty(PropertyName = "version")]
        internal ulong version { get; set; } = 1;

        [JsonProperty(PropertyName = "actions")]
        internal List<ManifestAction> actions { get; set; } = new List<ManifestAction>();

        [JsonProperty(PropertyName = "action_sets")]
        internal List<ManifestActionSet> actionSets { get; set; } = new List<ManifestActionSet>();

        [JsonProperty(PropertyName = "default_bindings")]
        internal List<ManifestDefaultBinding> defaultBindings { get; set; } = new List<ManifestDefaultBinding>();

        [JsonProperty(PropertyName = "localization")]
        internal List<Dictionary<string, string>> localization { get; set; } = new List<Dictionary<string, string>>();
    }
}
