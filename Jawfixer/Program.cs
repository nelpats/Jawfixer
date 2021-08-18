using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jawfixer
{
    class Program
    {
        static string baseContent;

        static string rawContent;

        static string urlContent;

        static string finalContent;

        static string[] content;

        static List<methodRef> refs = new List<methodRef>();

        static string vAlias;

        static void Main(string[] args)
        {
            Console.Title = "Jawfixer | github.com/NSDCode";

            if (args.Length < 1)
                end("invalid argument");

            if (!args[0].EndsWith(".py"))
                end("invalid file extension");

            content = File.ReadAllLines(args[0]);
            baseContent = File.ReadAllText(args[0]);

            log("importing file");


            log($"file name is {Path.GetFileNameWithoutExtension(args[0])}");

            log("press any key to start unpacking process");

            Console.ReadKey();

            log("unpacking first encoded code");
            unpack();

            log("unpacking second encoded code");
            unpack();

            urlContent = baseContent.Split(',')[0].Replace('"'.ToString(), string.Empty);
            urlContent = urlContent.Substring(17, urlContent.Length - 17);

            log($"found base url '{urlContent}'");
            log("downloading original code");

            finalContent = new WebClient().DownloadString(urlContent);

            log("saving original code");
            File.WriteAllText("unpacked.py", finalContent);

            end("unpacking process is done");
        }

        static void unpack()
        {
            if (baseContent.Contains(";"))
                beautify();

            collectMethods();
            getVulnerableAlias();

            if (vAlias.Length == 4)
                log($"found vulnerable alias '{vAlias}'");

            baseContent = getOutput().Replace('b' + '"'.ToString(), string.Empty).Replace('"'.ToString(), string.Empty);
        }
        static void beautify()
        {
            baseContent = baseContent.Replace(";", "\n");

            content = baseContent.Split('\n');
        }

        static string getOutput()
        {
            File.WriteAllText("temp.py", rawContent);

            ProcessStartInfo pInfos = new ProcessStartInfo();

            pInfos.RedirectStandardOutput = true;
            pInfos.UseShellExecute = false;

            pInfos.CreateNoWindow = true;
            pInfos.FileName = "cmd.exe";
            pInfos.Arguments = "/C py temp.py";

            Process process = new Process();

            process.StartInfo = pInfos;
            process.Start();

            return process.StandardOutput.ReadToEnd();
        }


        static void collectMethods()
        {
            refs.Clear();

            for (int i = 0; i < content.Length; i++)
            {           
                if (Regex.IsMatch(content[i], "[a-zA-Z-0-9] = [a-zA-Z]"))
                    refs.Add(new methodRef(content[i], i));                 
            }
        }

        static void getVulnerableAlias()
        {
            foreach (methodRef mRef in refs)
                if (mRef.methodName.Trim() == "exec")
                {
                    vAlias = mRef.alias.Trim();
                    content[mRef.line] = $"{vAlias} = print";

                    rawContent = string.Join(Environment.NewLine, content);
                }
        }

        static void log(string input)
        {
            Console.Write("[");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("JAWFIXER");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");

            Console.WriteLine(input);
        }

        static void end(string input)
        {
            log(input);

            Console.WriteLine("press any key to exit...");
            Console.ReadKey();

            Environment.Exit(0);

        }
    }
}
