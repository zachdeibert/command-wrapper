using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlType("metadata")]
	public class NuGetMetaData {
		[XmlArray("dependencies")]
		public List<NuGetDependency> Dependencies;

		public NuGetMetaData() {
			Dependencies = new List<NuGetDependency>();
		}
	}
}

