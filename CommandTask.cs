using System;
using System.Collections.Generic;
using System.Text;
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

		public static string[] SplitCommandLine(string argString) {
			List<string> args = new List<string>();
			StringBuilder arg = new StringBuilder();
			bool singleQuoted = false;
			bool doubleQuoted = false;
			foreach (char c in argString) {
				switch (c) {
					case '"':
						if (singleQuoted) {
							arg.Append('"');
						} else {
							doubleQuoted = !doubleQuoted;
						}
						break;
					case '\'':
						if (doubleQuoted) {
							arg.Append('\'');
						} else {
							singleQuoted = !singleQuoted;
						}
						break;
					case ' ':
						if (singleQuoted || doubleQuoted) {
							arg.Append(' ');
						} else {
							args.Add(arg.ToString());
							arg.Clear();
						}
						break;
					default:
						arg.Append(c);
						break;
				}
			}
			if (arg.Length > 0) {
				args.Add(arg.ToString());
			}
			return args.ToArray();
		}

		public override bool Execute() {
			string oldcwd = Environment.CurrentDirectory;
			try {
				Environment.CurrentDirectory = WorkingDirectory;
				return MainClass.Run(SplitCommandLine(Command)) == 0;
			} finally {
				Environment.CurrentDirectory = oldcwd;
			}
		}
	}
}

