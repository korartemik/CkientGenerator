using TestingServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestingServer.Parser
{
    public class ControllerParser2
    {
        private readonly string _path;
        private const string REQUEST_ANNOTATION = "@RequestMapping";

        public ControllerParser2(string path)
        {
            _path = path;
        }

        public List<MethodStructure> Methods { get; set; } = new List<MethodStructure>();

        public void ParseFile()
        {
            var lines = File.ReadAllLines(_path).ToList();

            var requestLines = lines.Where(l => l.Contains(REQUEST_ANNOTATION)).ToList();

            requestLines.ForEach(line =>
            {
                var url = GetUrl(line);
                var queryType = GetQueryType(line);
                var methodDeclarationLine = lines[lines.IndexOf(line) + 1];
                var name = GetName(methodDeclarationLine);
                var type = GetReturnedType(methodDeclarationLine);
                var argumentList = GetArgumentList(methodDeclarationLine);

                Methods.Add(new MethodStructure(type, name, argumentList, queryType, url));
            });
        }

        private static string GetUrl(string line)
        {
            var preUrl = Regex.Match(line, "/.*\"").Value;
            return preUrl.Substring(0, preUrl.Length - 1);
        }

        private static string GetQueryType(string line)
        {
            var preQueryType = Regex.Match(line, @"\..*\w").Value;
            return preQueryType.Substring(1);
        }

        private static string GetName(string line)
        {
            var words = Regex.Split(line, " ").Where(x => x != string.Empty).ToList();
            var name = words[2];
            return name.Split('(')[0];
        }

        private static string GetReturnedType(string line)
        {
            var preType = Regex.Match(line, "<.*>").Value;
            return preType.Substring(1, preType.Length - 2);
        }

        private static List<FieldStructure> GetArgumentList(string line)
        {
            var argumentList = new List<FieldStructure>();
            string parametrBody = Regex.Match(line, @"\(.*\)").Value;
            parametrBody = parametrBody.Substring(1, parametrBody.Length - 2);

            var variables = parametrBody.Split(',');
            variables.ToList().ForEach(v =>
            {
                var names = Regex.Split(v, " ").Where(x => x != string.Empty).ToList();
                if (names.Count > 1)
                {
                    argumentList.Add(new FieldStructure(names[names.Count - 2], names[names.Count - 1]));
                }
            });

            return argumentList;
        }
    }
}
