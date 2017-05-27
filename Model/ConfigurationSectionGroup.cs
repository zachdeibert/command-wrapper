using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlType("sectionGroup")]
	public class ConfigurationSectionGroup {
		[XmlAttribute("name")]
		public string Name;
		[XmlAttribute("type")]
		public string Type;
		[XmlElement("section")]
		public List<ConfigurationSection> Sections;

		public ConfigurationSectionGroup() {
			Sections = new List<ConfigurationSection>();
		}
	}
}

