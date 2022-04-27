using TestingServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestingServer.Parser
{
    internal class ClassParser
    {
        private readonly string _classPath;
        private const string CLASS_DECLARATION = "class";
        private const string PUBLIC_ACCESS = "public";
        private const string PRIVATE_ACCESS = "private";

        public ClassParser(string classPath)
        {
            _classPath = classPath;
        }

        public List<FieldStructure> Fields { get; set; } = new List<FieldStructure>();
        public string ClassName { get; private set; }
        public void ParseFile()
        {
            var lines = File.ReadAllLines(_classPath).ToList();
            ClassName = FindClassName(lines);
            Fields.AddRange(FindAllFields(lines));
        }
        private static List<FieldStructure> FindAllFields(List<string> lines)
        {
            var fields = new List<FieldStructure>();
            string lineClassDeclaration = lines.First(x => x.Split().ToList().Any(y => y.Equals(CLASS_DECLARATION)));
            int startIndex = lines.IndexOf(lineClassDeclaration);
            List<string> words = new List<string>();
            for (int i = startIndex + 1; i < lines.Count; i++)
            {
                words.AddRange(lines[i].Split(' '));
            }
            for (int i = 0; i < words.Count; i++)
            {
                if (words[i].Equals(PUBLIC_ACCESS) || words[i].Equals(PRIVATE_ACCESS))
                {
                    int k = i + 1;
                    int point = 0;
                    while (!words[k].Contains(';'))
                    {
                        if (words[k].Contains('{') || words[k].Contains('}') || words[k].Contains('='))
                        {
                            point = 1;
                            break;
                        }
                        k++;
                    }
                    if (point == 1)
                    {
                        continue;
                    }
                    string type, name;
                    int z = i + 1;
                    while (words[z].Length == 0)
                    {
                        z++;
                    }
                    type = words[z];
                    z++;
                    while (words[z].Length == 0)
                    {
                        z++;
                    }
                    if (words[z].Contains(';'))
                    {
                        name = words[z].Substring(0, words[z].Length - 1);
                    }
                    else
                    {
                        name = words[z];
                    }
                    fields.Add(new FieldStructure(type, name));
                }
            }
            return fields;
        }
        private static string FindClassName(List<string> lines)
        {
            string lineClassDeclaration = lines.First(x => x.Split().ToList().Any(y => y.Equals(CLASS_DECLARATION)));
            int lineIndex = lines.IndexOf(lineClassDeclaration);
            int classIndex = lineClassDeclaration.Split().ToList().IndexOf(CLASS_DECLARATION);
            string className;
            if (lineClassDeclaration.Split().ToList().Count == classIndex + 1)
            {
                return lines[lineIndex + 1].Split()[0];
            }
            else
            {
                return className = lineClassDeclaration.Split()[classIndex + 1];
            }
        }
    }
}
