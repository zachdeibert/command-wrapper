using System;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlType("assemblyIdentity")]
	public class AssemblyId {
		[XmlAttribute("name")]
		public string Name;
		[XmlAttribute("publicKeyToken")]
		public string PublicKey;
		[XmlAttribute("culture")]
		public string Culture;
	}
}

