using Newtonsoft.Json;

namespace DynamicOpenVR.Manifest
{
	internal class OVRDefaultBinding
	{
		[JsonProperty(PropertyName = "controller_type")]
		internal string ControllerType;

		[JsonProperty(PropertyName = "binding_url")]
		internal string BindingUrl;
	}
}
