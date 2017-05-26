using System;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlRoot("package")]
	public class NuSpecFile {
		[XmlElement("metadata")]
		public NuGetMetaData Metadata;
	}
}

