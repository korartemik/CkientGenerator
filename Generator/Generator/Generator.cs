using Generator.Parser;
using Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Generator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        private readonly string _classPaths = @"C:\ServerExample\src\main\java\com\example\ServerExample\Object.java";
        private readonly string _controllerPath = @"C:\ServerExample\src\main\java\com\example\ServerExample\ObjectController.java";

        public void Execute(GeneratorExecutionContext context)
        {
            var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);

            GenerateClass(_classPaths, context);
            GenerateClient(_controllerPath, context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {

        }

        private void GenerateClass(string path, GeneratorExecutionContext context)
        {
            var parser = new ClassParser(path);
            parser.ParseFile();
            string className = parser.ClassName;
            List<FieldStructure> fields = parser.Fields;

            var classDeclaration = SyntaxFactory.ClassDeclaration(parser.ClassName).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            fields.ForEach(f =>
            {
                var propertyDeclaration = SyntaxFactory
                .PropertyDeclaration(SyntaxFactory.ParseTypeName(f.Type), f.Name)
                .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)), SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
            });


            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("Generated"));
            namespaceDeclaration = namespaceDeclaration.AddMembers(classDeclaration);
            var compilationUnit = SyntaxFactory.CompilationUnit().AddMembers(namespaceDeclaration).NormalizeWhitespace();
            context.AddSource($"{parser.ClassName}.g.cs", compilationUnit.SyntaxTree.ToString());
        }

        private void GenerateClient(string path, GeneratorExecutionContext context)
        {
            const string clientName = "Client";

            var parser = new ControllerParser(path);
            parser.ParseFile();

            var methods = parser.Methods;

            var classDeclaration = SyntaxFactory.ClassDeclaration(clientName).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var fieldDeclaration = GetFieldDeclaration();
            var constructorDeclaration = GetConstructorDeclaration(clientName);

            classDeclaration = classDeclaration.AddMembers(fieldDeclaration, constructorDeclaration);

            methods.ForEach(m =>
            {
                classDeclaration = classDeclaration.AddMembers(GetMethodDeclaration(m));
            });

            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("Generated"));
            namespaceDeclaration = namespaceDeclaration.AddMembers(classDeclaration);
            var compilationUnit = SyntaxFactory.CompilationUnit().AddMembers(namespaceDeclaration).NormalizeWhitespace();
            compilationUnit = compilationUnit.AddUsings(GetUsings()).NormalizeWhitespace();
            context.AddSource($"{clientName}.g.cs", compilationUnit.SyntaxTree.ToString());

        }

        ConstructorDeclarationSyntax GetConstructorDeclaration(string clientName)
        {
            var constructorDeclaration = SyntaxFactory.ConstructorDeclaration(clientName).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            string classDeclarationText = "client = new HttpClient();\n client.BaseAddress = new Uri(\"http://localhost:8080\");";
            var lines = Regex.Split(classDeclarationText, "\n").ToList();
            var block = SyntaxFactory.Block(lines.Select(l => SyntaxFactory.ParseStatement(l)));
            constructorDeclaration = constructorDeclaration.AddBodyStatements(block);

            return constructorDeclaration;
        }

        FieldDeclarationSyntax GetFieldDeclaration()
        {
            var fieldDeclaration = SyntaxFactory.FieldDeclaration(SyntaxFactory
                .VariableDeclaration(SyntaxFactory.ParseTypeName("HttpClient"))
                .AddVariables(SyntaxFactory.VariableDeclarator("client")));
            fieldDeclaration = fieldDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

            return fieldDeclaration;
        }

        MethodDeclarationSyntax GetMethodDeclaration(MethodStructure methodStructure)
        {
            const string POST_QUERY = "POST";

            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(GetTypeWithTask(methodStructure.Type)), methodStructure.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.AsyncKeyword));

            methodStructure.ArgumentList.ForEach(a =>
            {
                var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(a.Name)).WithType(SyntaxFactory.ParseTypeName(a.Type));
                method = method.AddParameterListParameters(parameter);
            });

            if (methodStructure.QueryType.Equals(POST_QUERY))
            {
                method = method.WithBody(GetPostBlock(methodStructure));
            }
            else
            {
                method = method.WithBody(GetGetBlock(methodStructure));
            }

            return method;
        }

        private string GetTypeWithTask(string type)
        {
            return $"Task<{type}>";
        }

        private BlockSyntax GetPostBlock(MethodStructure methodStructure)
        {
            var block = SyntaxFactory.Block();

            string urlStatement = $"var url = $\"{methodStructure.Url}\";";

            string responseStatement = $"var response = await client.PostAsJsonAsync(url, {methodStructure.ArgumentList.First().Name});";
            string resultStatement = $"var result = await response.Content.ReadAsStringAsync();";
            string returnStatement = $"return JsonConvert.DeserializeObject<{methodStructure.Type}>(result);";

            List<string> blockStatements = new List<string>
            {
                urlStatement,
                responseStatement,
                resultStatement,
                returnStatement
            };

            return SyntaxFactory.Block(blockStatements.Select(x => SyntaxFactory.ParseStatement(x)));

        }

        private BlockSyntax GetGetBlock(MethodStructure methodStructure)
        {
            var block = SyntaxFactory.Block();

            string urlStatement = $"var url = $\"{methodStructure.Url}\";";

            string responseStatement = $"var response = await client.GetAsync(url);";
            string resultStatement = $"var result = await response.Content.ReadAsStringAsync();";
            string returnStatement = $" return JsonConvert.DeserializeObject<{methodStructure.Type}>(result);";

            List<string> blockStatements = new List<string>
            {
                urlStatement,
                responseStatement,
                resultStatement,
                returnStatement
            };

            return SyntaxFactory.Block(blockStatements.Select(x => SyntaxFactory.ParseStatement(x)));
        }

        private UsingDirectiveSyntax[] GetUsings()
        {
            var usings = new List<string>
            {
                "Newtonsoft.Json",
                "System.Collections",
                "System.Net.Http.Json"
            };

            return usings.Select(u => SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(u))).ToArray();
        }
    }
}
