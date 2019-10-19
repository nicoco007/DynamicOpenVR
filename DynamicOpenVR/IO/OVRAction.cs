// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright © 2019 Nicolas Gnyra

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

using System;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;

namespace DynamicOpenVR.IO
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class OVRAction
    {
        public string Name { get; }
        internal ulong Handle { get; private set; }

        private readonly Regex nameRegex = new Regex(@"^\/actions\/[a-z0-9_-]+\/(?:in|out)\/[a-z0-9_-]+$");

        protected OVRAction(string name)
        {
            if (!nameRegex.IsMatch(name))
            {
                throw new Exception($"Unexpected action name '{name}'; name should only contain letters, numbers, dashes, and underscores.");
            }

            Name = name.ToLowerInvariant();
        }
        
        internal string GetActionSetName()
        {
            return string.Join("/", Name.Split('/').Take(3));
        }

        internal void UpdateHandle()
        {
            Handle = OpenVRWrapper.GetActionHandle(Name);
        }
    }
}
