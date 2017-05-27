using System;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlType("codeBase")]
	public class AssemblyCodeBase {
		[XmlAttribute("version")]
		public string Version;
		[XmlAttribute("href")]
		public string Url;
	}
}

