using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlType("dependentAssembly")]
	public class AssemblyBinding {
		[XmlElement("assemblyIdentity")]
		public AssemblyId Name;
		[XmlElement("href")]
		public string LinkedUrl;
		[XmlElement("codeBase")]
		public List<AssemblyCodeBase> CodeBases;

		public AssemblyBinding() {
			CodeBases = new List<AssemblyCodeBase>();
		}
	}
}

