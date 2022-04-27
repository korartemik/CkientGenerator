using Generator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Generator.Parser
{
    public class ControllerParser
    {
        private readonly string _path;
        private const string REQUEST_ANNOTATION = "@RequestMapping";

        public ControllerParser(string path)
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
            string preUrl = line.Split('=')[1].Split(',')[0];
            return preUrl.Substring(2, preUrl.Length - 3);
        }

        private static string GetQueryType(string line)
        {
            var preQueryType = line.Split('=')[2].Split('.')[1];
            return preQueryType.Substring(0, preQueryType.Length - 1);
        }

        private static string GetName(string line)
        {
            var words = Regex.Split(line, " ").Where(x => x != string.Empty).ToList();
            var name = words[2];
            return name.Split('(')[0];
        }

        private static string GetReturnedType(string line)
        {
            string preType = Regex.Split(line, "ResponseEntity<")[1].Split(' ')[0];
            return preType.Substring(0, preType.Length - 1);
        }

        private static List<FieldStructure> GetArgumentList(string line)
        {
            var argumentList = new List<FieldStructure>();
            string parametrBody = line.Substring(line.IndexOf('(') + 1, line.LastIndexOf(')') - line.IndexOf('(') - 1);
            if (parametrBody.Length != 0)
            {
                var variables = parametrBody.Split(',');
                foreach (var variable in variables)
                {
                    var naming = variable.Split(' ').ToList();
                    argumentList.Add(new FieldStructure(naming[naming.Count - 2], naming[naming.Count - 1]));
                }
            }


            return argumentList;
        }
    }
}
