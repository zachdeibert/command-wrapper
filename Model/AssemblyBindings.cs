using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
    [XmlType("assemblyBinding", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class AssemblyBindings {
        [XmlElement("dependentAssembly")]
        public List<AssemblyBinding> Assemblies;

        public AssemblyBindings() {
            Assemblies = new List<AssemblyBinding>();
        }
    }
}
