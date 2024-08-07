using System.Threading.Tasks;
using Avalonia.Generators.Common;
using Avalonia.Generators.Compiler;
using Avalonia.Generators.NameGenerator;
using Avalonia.Generators.Tests.OnlyProperties.GeneratedCode;
using Avalonia.Generators.Tests.Views;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Avalonia.Generators.Tests.OnlyProperties;

public class OnlyPropertiesTests
{
    [Theory]
    [InlineData(OnlyPropertiesCode.NamedControl, View.NamedControl)]
    [InlineData(OnlyPropertiesCode.NamedControls, View.NamedControls)]
    [InlineData(OnlyPropertiesCode.XNamedControl, View.XNamedControl)]
    [InlineData(OnlyPropertiesCode.XNamedControls, View.XNamedControls)]
    [InlineData(OnlyPropertiesCode.NoNamedControls, View.NoNamedControls)]
    [InlineData(OnlyPropertiesCode.CustomControls, View.CustomControls)]
    [InlineData(OnlyPropertiesCode.DataTemplates, View.DataTemplates)]
    [InlineData(OnlyPropertiesCode.SignUpView, View.SignUpView)]
    [InlineData(OnlyPropertiesCode.AttachedProps, View.AttachedProps)]
    [InlineData(OnlyPropertiesCode.FieldModifier, View.FieldModifier)]
    [InlineData(OnlyPropertiesCode.ControlWithoutWindow, View.ControlWithoutWindow)]
    public async Task Should_Generate_FindControl_Refs_From_Avalonia_Markup_File(string expectation, string markup)
    {
        var compilation =
            View.CreateAvaloniaCompilation()
                .WithCustomTextBox();

        var classResolver = new XamlXViewResolver(
            new RoslynTypeSystem(compilation),
            MiniCompiler.CreateDefault(
                new RoslynTypeSystem(compilation),
                MiniCompiler.AvaloniaXmlnsDefinitionAttribute));

        var xaml = await View.Load(markup);
        var classInfo = classResolver.ResolveView(xaml);
        Assert.NotNull(classInfo);
        var nameResolver = new XamlXNameResolver();
        var names = nameResolver.ResolveNames(classInfo.Xaml);

        var generator = new OnlyPropertiesCodeGenerator();
        var generatorVersion = typeof(OnlyPropertiesCodeGenerator).Assembly.GetName().Version?.ToString();

        var code = generator
            .GenerateCode("SampleView", "Sample.App",  classInfo.XamlType, names)
            .Replace("\r", string.Empty);

        var expected = (await OnlyPropertiesCode.Load(expectation))
            .Replace("\r", string.Empty)
            .Replace("$GeneratorVersion", generatorVersion);

        CSharpSyntaxTree.ParseText(code);
        Assert.Equal(expected, code);
    }
}
