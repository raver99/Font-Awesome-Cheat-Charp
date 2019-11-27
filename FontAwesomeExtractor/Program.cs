using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FontAwesomeExtractor
{
    internal class Program
    {
        //Goto https://fontawesome.com/cheatsheet 
        //Inspect  the source, locate the body, expand it, locate <div class="ph4 ph6-ns pv6 ph0-pr pv0-pr bg-white"> minimize the it, right click, copy.
        //Add an html file to your project and set build to 'copy always' and paste the code into the file.

        //Since lasted version cheat sheat now need to copy for each type: solid, regular, brands, light,duotone .
        //For more info contact me:duc.nguyen.duy@preciofishbone.se

        private static void Main(string[] args)
        {
            string folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\html\";
            string[] filePaths = Directory.GetFiles(folder, "*.html");

            string outputString = "";
            foreach (string fileName in filePaths)
            {
                outputString += ProcessHTMLFile(fileName);
            }

            var systemPath = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(systemPath, $"fontAwesome.json");
            Console.WriteLine($"Create file at: {filePath}");
            using (FileStream fs = File.Create(filePath))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(outputString);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            Console.WriteLine($"All files processed - Press Any Keys To End");
            Console.ReadKey();
        }

        private static string ProcessHTMLFile(string fileName)
        {
            string path = fileName;

            //Initializes the Variable to use
            HtmlDocument htmlDoc = new HtmlDocument();
            //load html into variable
            htmlDoc.Load(path);

            //Load the three sessions (solid, regular, brands ) into the var 
            HtmlNodeCollection htmlSessions = htmlDoc.DocumentNode.SelectNodes("//section");
            //Iterate through sessions to process.
            string result = "";
            foreach (HtmlNode session in htmlSessions)
            {
                string mainClass = "";
                switch (session.Id)
                {
                    case "solid":
                        mainClass = "fas";
                        break;

                    case "regular":
                        mainClass = "far";
                        break;

                    case "brands":
                        mainClass = "fab";
                        break;

                    case "light":
                        mainClass = "fal";
                        break;

                    case "duotone":
                        mainClass = "fad";
                        break;
                }

                HtmlNodeCollection session_articles = session.SelectNodes(".//article");
                //Iterate through the Article List to process
                foreach (HtmlNode article in session_articles)
                {
                    string title = article.Id;
                    HtmlNode dlNode = article.ChildNodes.FindFirst("dl");
                    HtmlNode ddNode = dlNode.SelectSingleNode("dd[last()]");
                    string unicode = ddNode.InnerText;

                    string output = $@"{{cssClass:""{mainClass} fa-{title}"",""code"":""&#x{unicode};""}},";

                    result += output;
                }
            }

            return result;
        }
    }
}
