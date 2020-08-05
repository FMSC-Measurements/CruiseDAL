using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Xunit;

namespace FMSC.ORM.ModelGenerator.Test
{
    public class ModelGenerator_Test
    {
        [Fact]
        public void GenerateClass_Test()
        {
            var tableinfo = new TableInfo()
            {
                TableName = "MyTable",
                Fields = new[]
                {
                    new FieldInfo { FieldName = "col1", DbType = "INTEGER"},
                    new FieldInfo { FieldName = "col2", DbType = "TEXT"},
                    new FieldInfo { FieldName = "col3", DbType = "DOUBLE"},
                },
            };

            var generatedCode = CSModelGenerator.GenerateClass(tableinfo);

            var root = CSharpSyntaxTree.ParseText(generatedCode).GetRoot() as CompilationUnitSyntax;
            root.Should().NotBeNull();

            var @class = root.Members.Single() as ClassDeclarationSyntax;
            @class.AttributeLists.Should().NotBeEmpty(); // class should have EntitySource attr
            @class.Should().NotBeNull();

            var properties = @class.Members.OfType<PropertyDeclarationSyntax>().ToArray();
            properties.Should().HaveSameCount(tableinfo.Fields);
        }
    }
}