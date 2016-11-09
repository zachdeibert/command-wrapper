using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace commandwrapper {
	public class MSBuildTask : Task {
		[Required]
		public string Command {
			get;
			set;
		}

		public override bool Execute() {
			return MainClass.Run(Command.Split(' ')) == 0;
		}
	}
}

