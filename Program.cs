using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace commandwrapper {
	class MainClass {
		// Usage: command-wrapper.exe ENVNAME1=ENVVAL1 ENVNAME2=ENVVAL2 ... -- PROGRAM ARG1 ARG2
		public static void Main(string[] args) {
			ProcessStartInfo psi = new ProcessStartInfo();
			// Copy this process's environment
			psi.EnvironmentVariables.Clear();
			foreach ( DictionaryEntry var in Environment.GetEnvironmentVariables() ) {
				psi.EnvironmentVariables[(string) var.Key] = (string) var.Value;
			}
			// Parse the environment variable parameters
			int i;
			for ( i = 0; i < args.Length && args[i] != "--"; ++i ) {
				string[] parts = args[i].Split(new [] { '=' }, 2);
				if ( parts.Length == 1 ) {
					psi.EnvironmentVariables.Remove(args[i]);
				} else {
					psi.EnvironmentVariables[parts[0]] = parts[1];
				}
			}
			// Parse the command
			psi.Arguments = args.Skip(i + 2).DefaultIfEmpty().Aggregate((a, b) => string.Concat(a, " ", b));
			psi.FileName = args[i + 1];
			psi.RedirectStandardError = false;
			psi.RedirectStandardInput = false;
			psi.RedirectStandardOutput = false;
			psi.UseShellExecute = false;
			// Extra stuff
			AskCommand.Process(psi);
			// Launch the process
			using ( Process proc = Process.Start(psi) ) {
				proc.WaitForExit();
				Environment.ExitCode = proc.ExitCode;
			}
		}
	}
}
