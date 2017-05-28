using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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

		private static string AggregatePaths(string a, string b) {
			switch (Environment.OSVersion.Platform) {
				case PlatformID.MacOSX:
				case PlatformID.Unix:
					return string.Concat(a, ":", b);
				default:
					return string.Concat(a, ";", b);
			}
		}

		private static IEnumerable<string> FindDlls(string dir) {
			IEnumerable<string> dlls = new string[0];
			string lib = Path.Combine(dir, "lib");
			if (Directory.Exists(lib)) {
				if (Directory.GetFiles(lib).Any(f => f.EndsWith(".dll"))) {
					dlls = Directory.GetFiles(lib).Where(f => f.EndsWith(".dll"));
				}
				List<string> nonNets = new List<string>();
				int highestNet = -1;
				foreach (string subdir in Directory.GetDirectories(lib).Select(d => Path.GetFileName(d))) {
					int net;
					if (subdir.StartsWith("net")) {
						if (int.TryParse(subdir.Substring(3), out net)) {
							highestNet = Math.Max(highestNet, net);
							continue;
						}
					} else if (int.TryParse(subdir, out net)) {
						highestNet = Math.Max(highestNet, net);
						continue;
					}
					nonNets.Add(subdir);
				}
				if (highestNet > 0) {
					string d = Path.Combine(lib, string.Concat("net", highestNet.ToString()));
					if (!Directory.Exists(d)) {
						d = Path.Combine(lib, highestNet.ToString());
					}
					dlls = dlls.Concat(Directory.GetFiles(d).Where(f => f.EndsWith(".dll")));
				} else {
					dlls = dlls.Concat(nonNets.SelectMany(d => Directory.GetFiles(Path.Combine(lib, d))).Where(f => f.EndsWith(".dll")));
				}
			}
			return dlls;
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
                    switch (Environment.OSVersion.Platform) {
                        case PlatformID.MacOSX:
                        case PlatformID.Unix:
                            break;
                        default:
                            // Fix: The private assembly was located outside the appbase directory.
                            string junction = Path.Combine(Path.GetDirectoryName(psi.FileName), ".packages");
                            if (!Directory.Exists(junction)) {
                                ProcessStartInfo p = new ProcessStartInfo();
                                p.Arguments = string.Concat("/c mklink /J \"", junction.Replace("\\", "\\\\"), "\" \"", packagesDir.Replace("\\", "\\\\"), "\"");
                                p.FileName = "cmd.exe";
                                p.UseShellExecute = false;
                                p.Verb = "RunAs";
                                Process proc = System.Diagnostics.Process.Start(p);
                                proc.WaitForExit();
                                if (proc.ExitCode != 0) {
                                    break;
                                }
                            }
                            packagesDir = junction;
                            break;
                    }
                    string packageId = Path.GetFileName(dir);
					BuildDependencyTree(packagesDir, packageId, dependencies);
					dependencies.Remove(packageId);
					IEnumerable<string> newPath = dependencies.SelectMany(p => FindDlls(Path.Combine(packagesDir, p))
															  .Select(d => Path.GetDirectoryName(d))
															  .GroupBy(s => s)
															  .Select(g => g.Key));
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
					string configFile = string.Concat(psi.FileName, ".config");
					string realConfigFile = string.Concat(configFile, ".real");
					XmlSerializer serializer = new XmlSerializer(typeof(ApplicationConfiguration));
					if (File.Exists(configFile)) {
						ApplicationConfiguration config;
						using (Stream stream = File.OpenRead(configFile)) {
							config = (ApplicationConfiguration) serializer.Deserialize(stream);
						}
						if (config.Settings != null
						    && config.Settings.Wrapper != null
						    && config.Settings.Wrapper.Generated != null
						    && config.Settings.Wrapper.Generated.Name == "generated"
						    && config.Settings.Wrapper.Generated.Value == "true") {
							File.Delete(configFile);
						} else {
							if (File.Exists(realConfigFile)) {
								File.Delete(realConfigFile);
							}
							File.Move(configFile, realConfigFile);
						}
					}
					{
						ApplicationConfiguration config = new ApplicationConfiguration() {
							Sections = new List<ConfigurationSectionGroup>(new [] {
								new ConfigurationSectionGroup() {
									Name = "applicationSettings",
									Type = "System.Configuration.ApplicationSettingsGroup, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
									Sections = new List<ConfigurationSection>(new [] {
										new ConfigurationSection() {
											Name = "Com.GitHub.ZachDeibert.CommandWrapper",
											Type = "System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
										}
									})
								}
							}),
							Settings = new ApplicationSettings() {
								Wrapper = new WrapperSettings() {
									Generated = new ApplicationSetting() {
										Name = "generated",
										SerializeAs = "String",
										Value = "true"
									}
								}
							},
							Runtime = new RuntimeConfiguration() {
								Bindings = new AssemblyBindings() {
                                    Assemblies = dependencies.SelectMany(p => FindDlls(Path.Combine(packagesDir, p)))
														     .Select(d => {
															     try {
																     return Assembly.LoadFile(d).GetName();
															     } catch (Exception ex) {
											    Console.Error.WriteLine(ex);
																     return null;
															     }})
														     .Where(n => n != null)
														     .GroupBy(a => a.Name)
														     .Select(a => new AssemblyBinding() {
										    Name = new AssemblyId() {
											    Name = a.Key,
											    PublicKey = a.First().GetPublicKeyToken() == null ? null : BitConverter.ToString(a.First().GetPublicKeyToken()).Replace("-", "").ToLower(),
											    Culture = string.IsNullOrEmpty(a.First().CultureName) ? "neutral" : a.First().CultureName
										    },
										    CodeBases = a.GroupBy(n => n.Version).Select(g => g.First()).Select(n => new AssemblyCodeBase() {
											    Version = n.Version.ToString(),
											    Url = n.CodeBase
										    }).ToList()
								    }).ToList()
                                }
							},
							Config = File.Exists(realConfigFile) ? new RuntimeConfiguration() {
                                Bindings = new AssemblyBindings() {
                                    Assemblies = new List<AssemblyBinding>(new[] {
                                        new AssemblyBinding() {
                                            LinkedUrl = realConfigFile
                                        }
                                    })
                                }
							} : null
						};
						XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
						ns.Add("", "");
						using (Stream stream = File.OpenWrite(configFile)) {
							serializer.Serialize(stream, config, ns);
						}
					}
				}
			}
		}
	}
}

