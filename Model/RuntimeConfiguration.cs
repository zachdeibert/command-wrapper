using System;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlType("runtime")]
	public class RuntimeConfiguration {
        [XmlElement("assemblyBinding", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public AssemblyBindings Bindings;
	}
}

