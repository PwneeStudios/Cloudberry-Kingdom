using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.IO;
using Drawing;

namespace CloudberryKingdom
{
    public class InfoWad2
    {
        public static void Read(String file)
        {
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader reader = new StreamReader(stream);

            String line;

            line = reader.ReadLine();
            while (line != null)
            {
                line = Tools.RemoveComment(line);

                var bits = line.Split(' ');

                if (bits.Length > 0)
                {
                    var first = bits[0];

                    if (first.Contains("Pillar_"))
                    {
                        var num_str = first.Substring(first.IndexOf("_") + 1);
                        int width = int.Parse(num_str);
                        
                    }
                    else if (first.Contains("Pillar_"))
                    {
                    }
                }


                line = reader.ReadLine();
            }

            reader.Close();
            stream.Close();
        }
    }
}