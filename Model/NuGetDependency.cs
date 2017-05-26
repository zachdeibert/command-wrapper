using System;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlType("dependency")]
	public class NuGetDependency {
		[XmlAttribute("id")]
		public string Id;
		[XmlAttribute("version")]
		public string Version;

		public string DependencyId {
			get {
				return string.Concat(Id, ".", Version);
			}
		}
	}
}

