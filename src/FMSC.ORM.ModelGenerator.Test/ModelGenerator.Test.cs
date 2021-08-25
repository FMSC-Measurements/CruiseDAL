using FluentAssertions;
using FMSC.ORM.ModelGenerator.Generators;
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
                    new FieldInfo { FieldName = "col4np", DbType = "DOUBLE"},
                },
            };

            var nonPersistedColumns = new[] { "col4np" };

            var generatedCode = CSModelGenerator.GenerateClass(tableinfo, nonPersistedColumns: nonPersistedColumns);

            // parse C# to validate syntax 
            var root = CSharpSyntaxTree.ParseText(generatedCode).GetRoot() as CompilationUnitSyntax;
            root.Should().NotBeNull();

            var @class = root.Members.Single() as ClassDeclarationSyntax;
            @class.Should().NotBeNull();

            // verify pocos have table attribute
            @class.AttributeLists.Should().NotBeEmpty();
            var attrs = @class.AttributeLists.SelectMany(x => x.Attributes).ToArray();
            attrs.Should().Contain(x => x.Name.ToString() == "Table", "model should have table attribute");


            var properties = @class.Members.OfType<PropertyDeclarationSyntax>().ToArray();
            properties.Should().HaveSameCount(tableinfo.Fields);

            foreach (var prop in properties)
            {
                var pAttrs = prop.AttributeLists.SelectMany(x => x.Attributes);

                // verify nonPersisted fields have PersistanceFlags.Never
                if (nonPersistedColumns.Contains(prop.Identifier.ValueText))
                {
                    pAttrs.Should().Contain(x => x.ToString().Contains("PersistanceFlags.Never"));
                }
            }
        }
    }
}