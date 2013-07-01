using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaticSiteGenerator
{
	public class LineInfo
	{
		public string Identifier;
		public string Value;
	}

	public static class Parse
	{
		public static string RemovePadding(string s)
		{
			int FirstNonBlank = 0;
			while (FirstNonBlank < s.Length)
			{
				if (s[FirstNonBlank] == ' ' || s[FirstNonBlank] == '\n' || s[FirstNonBlank] == '\r')
				{
					FirstNonBlank++;
				}
				else
				{
					break;
				}
			}

			int LastNonBlank = s.Length - 1;
			while (LastNonBlank > 0)
			{
				if (s[LastNonBlank] == ' ' || s[LastNonBlank] == '\n' || s[LastNonBlank] == '\r')
				{
					LastNonBlank--;
				}
				else
				{
					break;
				}
			}

			return s.Substring(FirstNonBlank, LastNonBlank + 1 - FirstNonBlank);
		}

		static bool IdentifierCheck_If(string key)
		{
			try
			{
				if (key.Substring(0, 3) == "if ")
				{
					return true;
				}
			}
			catch (Exception e)
			{
				return false;
			}

			return false;
		}

		static string VariableAfterIdentifier(string key)
		{
			int FirstChar = key.IndexOf(' ') + 1;
			for (int i = FirstChar; i < key.Length; i++)
			{
				if (key[i] == '\n' || key[i] == '\r' || key[i] == ' ')
				{
					string Variable = key.Substring(FirstChar, i - FirstChar);
					return Variable;
				}
			}

			return key.Substring(FirstChar);
		}

		public static string CompileTemplate(string Template, Dictionary<string, string> Values)
		{
			var pieces = Template.Split('✖');

			string s = "";
			for (int i = 0; i < pieces.Length; i++)
			{
				if (i % 2 == 0)
				{
					s += pieces[i];
				}
				else
				{
					string key = pieces[i];
					key = RemovePadding(key);

					if (IdentifierCheck_If(key))
					{
						string inner_key = VariableAfterIdentifier(key);
						try
						{
							if (Values[inner_key] == "true")
								s += key.Substring(key.IndexOf(inner_key) + inner_key.Length);
						}
						catch
						{
						}
					}
					else
					{
						if (key.Contains('='))
						{
							var parts = key.Split('=');
							try
							{
								s += Values[parts[0]];
							}
							catch
							{
								s += parts[1];
							}
						}
						else
						{
							s += Values[key];
						}
					}
				}
			}

			return s;
		}

		public static List<LineInfo> ParseSmu(string Smu)
		{
			// Remove big pieces (✖ ... ✖)
			var pieces = Smu.Split('✖');

			Smu = "";
			string[] BigValues = new string[pieces.Length / 2];
			for (int i = 0; i < pieces.Length; i++)
			{
				if (i % 2 == 0)
				{
					Smu += pieces[i];
				}
				else
				{
					Smu += '✖';
					BigValues[i / 2] = pieces[i];
				}
			}
			int NextBigValue = 0;

			// Break-up by line (semi-colon)
			var raw_lines = Smu.Split(';');

			List<LineInfo> Lines = new List<LineInfo>(raw_lines.Length);
			foreach (var raw_line in raw_lines)
			{
				LineInfo line = new LineInfo();
				var line_pieces = raw_line.Split('=');

				if (line_pieces.Length <= 1) continue;

				line.Identifier = RemovePadding(line_pieces[0]);

				if (line_pieces[1].Contains('✖'))
				{
					line.Value = BigValues[NextBigValue];
					NextBigValue++;
				}
				else
				{
					line.Value = RemovePadding(line_pieces[1]);
				}

				Lines.Add(line);
			}

			return Lines;
		}
	}
}
