using TestingServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestingServer.Parser
{
    public class ClassParser2
    {
        private readonly string _classPath;
        private const string CLASS_DECLARATION = "class";
        private const string PUBLIC_ACCESS = "public";
        private const string PRIVATE_ACCESS = "private";

        public ClassParser2(string classPath)
        {
            _classPath = classPath;
        }

        public List<FieldStructure> Fields { get; set; } = new List<FieldStructure>();
        public string ClassName { get; private set; }
        public void ParseFile()
        {
            var lines = File.ReadAllLines(_classPath).ToList();
            int startIndex = lines
                .IndexOf(lines
                .First(x => x.Split()
                .ToList()
                .Any(y => y.Equals(CLASS_DECLARATION))));

            ClassName = lines[startIndex].Split()[2];

            for (int i = startIndex + 1; i < lines.Count; i++)
            {
                if (IsFieldDeclaration(lines[i]))
                {
                    var separatedLine = Regex.Split(lines[i], " ").Where(x => x != string.Empty).ToList();
                    Fields.Add(new FieldStructure(separatedLine[1], separatedLine[2].Substring(0, separatedLine[2].Length - 1)));
                }
            }
        }

        private static bool IsFieldDeclaration(string line)
        {
            return !line.Contains('{') && !line.Contains('(') && line.Length != 0 && (line.Contains(PRIVATE_ACCESS) || line.Contains(PUBLIC_ACCESS));
        }
    }
}
