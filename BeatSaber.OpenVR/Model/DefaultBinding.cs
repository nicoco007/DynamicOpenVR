using Newtonsoft.Json;

namespace BeatSaber.OpenVR.Model
{
	internal class DefaultBinding
	{
		[JsonProperty(PropertyName = "controller_type")]
		internal string ControllerType;

		[JsonProperty(PropertyName = "binding_url")]
		internal string BindingUrl;
	}
}
