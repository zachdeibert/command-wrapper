using System;
using System.Diagnostics;

namespace commandwrapper {
	[Obsolete]
	public static class AskCommand {
		public static string ShowDialog(string prompt, out bool remember) {
			return Com.GitHub.ZachDeibert.CommandWrapper.AskCommand.ShowDialog(prompt, out remember);
		}

		public static void Process(ProcessStartInfo psi) {
			Com.GitHub.ZachDeibert.CommandWrapper.AskCommand.Process(psi);
		}
	}

	[Obsolete]
	public class AskForm : Com.GitHub.ZachDeibert.CommandWrapper.AskForm {
		public AskForm(string prompt) : base(prompt) {
		}
	}

	[Obsolete]
	public class CommandTask : Com.GitHub.ZachDeibert.CommandWrapper.CommandTask {
	}

	[Obsolete]
	class MainClass {
		public static int Run(string[] args) {
			return Com.GitHub.ZachDeibert.CommandWrapper.MainClass.Run(args);
		}

		public static void Main(string[] args) {
			Com.GitHub.ZachDeibert.CommandWrapper.MainClass.Main(args);
		}
	}
}

