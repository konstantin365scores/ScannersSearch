using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;

namespace ScoresThreeSixFive.ScannerSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter scanner name: ");
            var scanner = Console.ReadLine();

            if (!string.IsNullOrEmpty(scanner)) 
            {
                scanner = scanner.ToUpper();
                Console.WriteLine($"Searching for {scanner} scanner...");

                var result = ScannerSearch(scanner);
                foreach (var item in result) 
                {
                    Console.WriteLine(item);
                }

                var count = result.Where(x => x.Contains("Name: ")).Count();
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine($"Total results: {count}");
                Console.ReadLine();
            }
            else 
            {
                Console.WriteLine("Invalid input, bye bye...");
            }
        }

        static List<string> ScannerSearch(string scanner)
        {
            var result = new List<string>();

            foreach(var srv in new int[] { 1, 2, 3, 4, 5, 6, 7 })
            {
                try 
                {
                    var html = GetHtml(srv);
                    var found = PraseHtml(html, scanner, srv);
                    result.AddRange(found);
                }
                catch(Exception ex) 
                {
                    Console.WriteLine($"Error at server #{srv}: {ex.Message}");
                    continue;
                }
            }

            return result;
        }

        static string GetHtml(int srv)
        {
            var cleint = new WebClient();
            var html = cleint.DownloadString(new Uri($"http://scn{srv}.sportifier.com/CheckStatus.aspx?Code=VerYg00dPass"));
            return html;
        }
    
        static List<string> PraseHtml(string html, string scanner, int srv)
        {
            var result = new List<string>();

            var lines = html.Split('\n');
            var foundByName = false;
            var foundName = string.Empty;
            foreach (var line in lines)
            {
                if (!foundByName && line.Contains("<tr><td><b>Name:</b>"))
                {
                    var name = line.Replace("<tr><td><b>Name:</b>", "").Replace("</td></tr>", "").Trim().ToUpper();
                    foundName = name;
                    if (name.Contains(scanner))
                    {
                        foundByName = true;
                        continue;
                    }
                }

                if (line.Contains("<tr><td><b>Type:</b>"))
                {
                    var type = line.Replace("<tr><td><b>Type:</b>", "").Replace("</td></tr>", "").Trim().ToUpper();
                    if (type.Contains(scanner) || foundByName)
                    {
                        result.Add("-------------------------------------------------------");
                        result.Add($"Name: {foundName}");
                        result.Add($"Type: {type}");
                        result.Add($"Server: SCN{srv}.SPORTIFIER.COM");
                    }
                }

                if (foundByName) 
                {
                    foundByName = false;
                }
            }

            return result;
        }
    }
}
