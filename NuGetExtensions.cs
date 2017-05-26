using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
#if DEBUG
using System.Text;
#endif
using System.Xml;
using System.Xml.Serialization;
using Com.GitHub.ZachDeibert.CommandWrapper.Model;

namespace Com.GitHub.ZachDeibert.CommandWrapper {
	public class NuGetExtensions {
#if DEBUG
		private static StringBuilder Indent = new StringBuilder();
#endif

		private static void BuildDependencyTree(string packagesDir, string packageId, HashSet<string> dependencies) {
			string file = Path.Combine(packagesDir, packageId, string.Concat(packageId, ".nupkg"));
			if (!File.Exists(file)) {
				string tmp = packageId;
				packageId = Directory.GetDirectories(packagesDir).Select(d => Path.GetFileName(d)).FirstOrDefault(d => d.StartsWith(string.Concat(packageId, ".")));
				if (packageId == null) {
					Console.Error.WriteLine("Unable to find package {0}", tmp);
					return;
				}
				file = Path.Combine(packagesDir, packageId, string.Concat(packageId, ".nupkg"));
			}
			dependencies.Add(packageId);
#if DEBUG
			Console.WriteLine("{0}{1}", Indent, packageId);
			Indent.Append(' ');
			try {
#endif
				ZipArchive archive = ZipFile.Open(file, ZipArchiveMode.Read);
				using (Stream stream = archive.Entries.First(e => e.Name.EndsWith(".nuspec")).Open()) {
					XmlSerializer serializer = new XmlSerializer(typeof(NuSpecFile));
					NuSpecFile nuspec = (NuSpecFile) serializer.Deserialize(new PatchedXmlTextReader(stream));
					foreach (string dependency in nuspec.Metadata.Dependencies.Select(d => d.DependencyId).Where(d => !dependencies.Contains(d))) {
						BuildDependencyTree(packagesDir, dependency, dependencies);
					}
				}
#if DEBUG
			} finally {
				Indent.Remove(0, 1);
			}
#endif
		}

		private static IEnumerable<string> FindBinaryDirs(string dir) {
			IEnumerable<string> dirs = new string [0];
			if (Directory.GetFiles(dir).Any(f => f.EndsWith(".exe") || f.EndsWith(".dll"))) {
				dirs = new [] { dir };
			}
			return dirs.Concat(Directory.GetDirectories(dir).SelectMany(d => FindBinaryDirs(d)));
		}

		private static string AggregatePaths(string a, string b) {
			switch (Environment.OSVersion.Platform) {
				case PlatformID.MacOSX:
				case PlatformID.Unix:
					return string.Concat(a, ":", b);
				default:
					return string.Concat(a, ";", b);
			}
		}

		public static void Process(ProcessStartInfo psi) {
			if (psi.EnvironmentVariables["WRAPPER_NUGET"] == "true") {
				string dir = Path.GetDirectoryName(psi.FileName);
				while (!string.IsNullOrEmpty(dir) && !File.Exists(Path.Combine(dir, string.Concat(Path.GetFileName(dir), ".nupkg")))) {
					dir = Path.GetDirectoryName(dir);
				}
				if (string.IsNullOrEmpty(dir)) {
					Console.Error.WriteLine("Unable to find nuget metadata");
				} else {
					HashSet<string> dependencies = new HashSet<string>();
					string packagesDir = Path.GetDirectoryName(dir);
					string packageId = Path.GetFileName(dir);
					BuildDependencyTree(packagesDir, packageId, dependencies);
					dependencies.Remove(packageId);
					IEnumerable<string> newPath = dependencies.SelectMany(p => FindBinaryDirs(Path.Combine(packagesDir, p)));
					if (psi.EnvironmentVariables.ContainsKey("PATH")) {
						psi.EnvironmentVariables["PATH"] = new [] { psi.EnvironmentVariables["PATH"] }.Concat(newPath).Aggregate(AggregatePaths);
					} else {
						psi.EnvironmentVariables["PATH"] = newPath.Aggregate(AggregatePaths);
					}
					if (psi.EnvironmentVariables.ContainsKey("MONO_PATH")) {
						psi.EnvironmentVariables["MONO_PATH"] = new [] { psi.EnvironmentVariables["MONO_PATH"] }.Concat(newPath).Aggregate(AggregatePaths);
					} else {
						psi.EnvironmentVariables["MONO_PATH"] = newPath.Aggregate(AggregatePaths);
					}
#if DEBUG
					Console.WriteLine(psi.EnvironmentVariables["PATH"]);
#endif
				}
			}
		}
	}
}

