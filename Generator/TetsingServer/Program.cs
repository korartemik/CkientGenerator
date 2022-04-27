using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using TestingServer.Parser;

var c = new ControllerParser(@"C:\ServerExample\src\main\java\com\example\ServerExample\ObjectController.java");
c.ParseFile();
foreach(var f in c.Methods)
{
    Console.WriteLine(f.Name);
    Console.WriteLine(f.QueryType);
    Console.WriteLine(f.Type);
    Console.WriteLine(f.Url);
    foreach(var t in f.ArgumentList)
    {
        Console.WriteLine(t.Name + " " + t.Type);
    }
}

var d = new ControllerParser2(@"C:\ServerExample\src\main\java\com\example\ServerExample\ObjectController.java");
d.ParseFile();
foreach (var f in d.Methods)
{
    Console.WriteLine(f.Name);
    Console.WriteLine(f.QueryType);
    Console.WriteLine(f.Type);
    Console.WriteLine(f.Url);
    foreach (var t in f.ArgumentList)
    {
        Console.WriteLine(t.Name + " " + t.Type);
    }
}