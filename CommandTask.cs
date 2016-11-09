﻿using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace commandwrapper {
	public class CommandTask : Task {
		[Required]
		public string Command {
			get;
			set;
		}
			
		public string WorkingDirectory {
			get;
			set;
		}

		public override bool Execute() {
			Environment.CurrentDirectory = WorkingDirectory;
			return MainClass.Run(Command.Split(' ')) == 0;
		}
	}
}
