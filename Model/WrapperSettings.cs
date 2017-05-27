using System;
using System.Xml.Serialization;

namespace Com.GitHub.ZachDeibert.CommandWrapper.Model {
	[XmlType("Com.GitHub.ZachDeibert.CommandWrapper")]
	public class WrapperSettings {
		[XmlElement("setting")]
		public ApplicationSetting Generated;
	}
}

