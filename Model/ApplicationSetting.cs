using System;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlType("setting")]
	public class ApplicationSetting {
		[XmlAttribute("name")]
		public string Name;
		[XmlAttribute("serializeAs")]
		public string SerializeAs;
		[XmlElement("value")]
		public string Value;
	}
}

