using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace commandwrapper {
	public static class AskCommand {
		public static string ShowDialog(string prompt, out bool remember) {
			using ( AskForm form = new AskForm(prompt) ) {
				try {
					if ( form.ShowDialog() == DialogResult.OK ) {
						return form.Value;
					} else {
						Environment.Exit(127);
						return null;
					}
				} finally {
					remember = form.RememberValue;
				}
			}
		}

		public static void Process(ProcessStartInfo psi) {
			if ( psi.EnvironmentVariables.ContainsKey("WRAPPER_ASK") ) {
				StringDictionary remembered = new StringDictionary();
				try {
					using ( Stream stream = new FileStream(".command-wrapper.txt", FileMode.Open, FileAccess.Read) ) {
						using ( StreamReader reader = new StreamReader(stream) ) {
							while ( !reader.EndOfStream ) {
								remembered[reader.ReadLine()] = reader.ReadLine();
							}
						}
					}
				} catch ( IOException ) {
				}
				bool needSave = false;
				string[] args = psi.Arguments.Split(' ');
				foreach ( int arg in psi.EnvironmentVariables["WRAPPER_ASK"].Split(',').Select(s => int.Parse(s)) ) {
					string msg = args[arg].Replace("__", " ");
					if ( remembered.ContainsKey(msg) ) {
						args[arg] = remembered[msg];
					} else {
						bool remember;
						args[arg] = ShowDialog(msg, out remember);
						if ( remember ) {
							remembered[msg] = args[arg];
							needSave = true;
						}
					}
				}
				if ( needSave ) {
					using ( Stream stream = new FileStream(".command-wrapper.txt", FileMode.Create, FileAccess.Write) ) {
						using ( StreamWriter writer = new StreamWriter(stream) ) {
							foreach ( DictionaryEntry entry in remembered ) {
								writer.WriteLine(entry.Key);
								writer.WriteLine(entry.Value);
							}
						}
					}
				}
				psi.Arguments = string.Join(" ", args.Select(s => string.Format("\"{0}\"", s)));
			}
		}
	}
}

