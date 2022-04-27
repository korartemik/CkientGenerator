using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServer.Models
{
    public class MethodStructure
    {
        public MethodStructure(string type, string name, List<FieldStructure> argumentList, string queryType, string url)
        {
            Type = type;
            Name = name;
            ArgumentList = argumentList;
            QueryType = queryType;
            Url = url;
        }

        public string Type { get; set; }
        public string Name { get; set; }
        public List<FieldStructure> ArgumentList { get; set; }
        public string QueryType { get; set; }
        public string Url { get; set; }
    }
}
