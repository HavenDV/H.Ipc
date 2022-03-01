//using System.Reflection;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.Text;

//namespace H.NSwag.Generator.IntegrationTests;

//[TestClass]
//public class NSwagGeneratorTests
//{
//    public async Task BaseGenerateTest(string text)
//    {
//        var path = Path.GetTempFileName();
//        File.WriteAllText(path, text);

//        var source = await NSwagGenerator.GenerateAsync(path);

//        Console.WriteLine(source);
//    }

//    [TestMethod]
//    public async Task GenerateFromYamlTest()
//    {
//        await BaseGenerateTest(Resources.openapi_from_yaml_nswag.AsString());
//    }

//    [TestMethod]
//    public async Task GenerateFromYamlFileTest()
//    {
//        File.WriteAllBytes(Path.Combine(Path.GetTempPath(), "openapi.yaml"), Resources.openapi_yaml.AsBytes());

//        await BaseGenerateTest(Resources.openapi_from_yaml_file_nswag.AsString());
//    }

//    [TestMethod]
//    public void ExecuteTest()
//    {
//        var text = Resources.openapi_from_url_nswag.AsString();
//        var path = Path.GetTempFileName() + ".nswag";
//        File.WriteAllText(path, text);

//        if (Environment.OSVersion.Platform == PlatformID.Unix)
//        {
//            Directory.SetCurrentDirectory(Path.GetDirectoryName(path) ?? string.Empty);
//        }

//        var inputCompilation = CSharpCompilation.Create(
//            "compilation",
//            new[] { CSharpSyntaxTree.ParseText(@"
//namespace MyCode
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//        }
//    }
//}
//") },
//            new[]
//            {
//                    MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
//            },
//            new CSharpCompilationOptions(OutputKind.ConsoleApplication));

//        var generator = new NSwagGenerator();
//        var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(
//            new ISourceGenerator[] { generator },
//            new AdditionalText[] { new CustomAdditionalText(path) });

//        driver.RunGeneratorsAndUpdateCompilation(
//            inputCompilation,
//            out var outputCompilation,
//            out var diagnostics);

//        diagnostics.Should().BeEmpty();
//        outputCompilation.SyntaxTrees.Should().HaveCount(2);

//        var source = outputCompilation.SyntaxTrees.ElementAt(1).GetText().ToString();
//        Console.WriteLine(source);
//    }

//    public class CustomAdditionalText : AdditionalText
//    {
//        public string Text { get; }

//        public override string Path { get; }

//        public CustomAdditionalText(string path)
//        {
//            Path = path;
//            Text = File.ReadAllText(path);
//        }

//        public override SourceText GetText(CancellationToken cancellationToken = default)
//        {
//            return SourceText.From(Text);
//        }
//    }
//}
