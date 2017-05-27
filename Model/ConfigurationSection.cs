using System;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlType("section")]
	public class ConfigurationSection {
		[XmlAttribute("name")]
		public string Name;
		[XmlAttribute("type")]
		public string Type;
	}
}

