using HtmlAgilityPack;
using System;
using System.IO;
using System.Text;

namespace FontAwesomeExtractor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Goto https://fontawesome.com/cheatsheet 
            //Inspect  the source, locate the body, expand it, locate <div class="ph4 ph6-ns pv6 ph0-pr pv0-pr bg-white"> minimize the it, right click, copy.
            //Add an html file to your project and set build to 'copy always' and paste the code into the file.
            string path = @"5.8.2-pro.html";

            //Initializes the Variable to use
            HtmlDocument htmlDoc = new HtmlDocument();
            //load html into variable
            htmlDoc.Load(path);

            //Load the three sessions (solid, regular, brands ) into the var 
            HtmlNodeCollection htmlSessions = htmlDoc.DocumentNode.SelectNodes("//section");
            //Iterate through sessions to process.
            string outputString = "";
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

                    case "light": // pro
                        mainClass = "fal";
                        break;
                }

                HtmlNodeCollection session_articles = session.SelectNodes("//article");
                //Iterate through the Article List to process
                foreach (HtmlNode article in session_articles)
                {

                    string title = article.Id;
                    HtmlNode dlNode = article.ChildNodes[1];
                    HtmlNode ddNode = dlNode.ChildNodes[5];
                    string unicode = ddNode.InnerText;
                    //string output = $@" public static string {Processor.Edit(title)} = ""\u{unicode}"";"; //&#x
                    string output = $@"{{ cssClass:""{mainClass} fa-{title}"", ""code"": ""&#x{unicode};"" }},";

                    outputString += output + System.Environment.NewLine;
                }
            }

            var systemPath = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(systemPath, $"{path}.txt");
            Console.WriteLine($"Create file at: {filePath}");
            using (FileStream fs = File.Create(filePath))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(outputString);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            Console.ReadKey();
        }
    }
}
