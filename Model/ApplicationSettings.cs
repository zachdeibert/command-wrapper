using System;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlType("applicationSettings")]
	public class ApplicationSettings {
		[XmlElement("Com.GitHub.ZachDeibert.CommandWrapper")]
		public WrapperSettings Wrapper;
	}
}

