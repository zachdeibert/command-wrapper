using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlRoot("configuration")]
	public class ApplicationConfiguration {
		[XmlArray("configSections")]
		public List<ConfigurationSectionGroup> Sections;
		[XmlElement("applicationSettings")]
		public ApplicationSettings Settings;
		[XmlElement("runtime")]
		public RuntimeConfiguration Runtime;
		[XmlElement("configuration")]
		public RuntimeConfiguration Config;

		public ApplicationConfiguration() {
			Sections = new List<ConfigurationSectionGroup>();
		}
	}
}

