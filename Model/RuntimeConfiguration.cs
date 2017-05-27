using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlType("runtime")]
	public class RuntimeConfiguration {
		[XmlArray("assemblyBinding", Namespace = "urn:schemas-microsoft-com:asm.v1")]
		public List<AssemblyBinding> Assemblies;

		public RuntimeConfiguration() {
			Assemblies = new List<AssemblyBinding>();
		}
	}
}

