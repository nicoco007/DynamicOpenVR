// <copyright file="DefaultBinding.cs" company="Nicolas Gnyra">
// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
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

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicOpenVR.SteamVR.VRManifest
{
    public record DefaultBinding
    {
#pragma warning disable IDE0044, IDE0051, CS0169
        [JsonExtensionData]
        private IDictionary<string, JToken> _properties;
#pragma warning restore IDE0044, IDE0051, CS0169

        public string controllerType { get; set; }

        public string url { get; set; }
    }
}
