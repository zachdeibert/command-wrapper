using System;
using System.IO;
using System.Xml;

namespace Com.GitHub.ZachDeibert.CommandWrapper {
	public class PatchedXmlTextReader : XmlTextReader {
		public override string NamespaceURI {
			get {
				return "";
			}
		}

		public PatchedXmlTextReader(Stream stream) : base(stream) {
		}
	}
}

