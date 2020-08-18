using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

namespace codegenny.generators
{
    [Generator]
    public class ExpressionGenerator : ISourceGenerator
    {
        public void Initialize(InitializationContext context)
        {
            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new MySyntaxReceiver());
        }

        public void Execute(SourceGeneratorContext context)
        {
            // the generator infrastructure will create a receiver and populate it
            // we can retrieve the populated instance via the context
            MySyntaxReceiver syntaxReceiver = (MySyntaxReceiver)context.SyntaxReceiver;

            // get the recorded user class
            ClassDeclarationSyntax userClass = syntaxReceiver.ClassToAugment;
            if (userClass is null)
            {
                // if we didn't find the user class, there is nothing to do
                return;
            }

            // add the generated implementation to the compilation
            SourceText sourceText = SourceText.From($@"
public partial class {userClass.Identifier}
{{
    public void GeneratedMethod()
    {{
        // generated code
    }}
}}", Encoding.UTF8);
            context.AddSource("ExpressionHolder.Generated.cs", sourceText);
        }

        class MySyntaxReceiver : ISyntaxReceiver
        {
            public ClassDeclarationSyntax ClassToAugment { get; private set; }

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                // Business logic to decide what we're interested in goes here
                if (syntaxNode is ClassDeclarationSyntax cds &&
                    cds.Identifier.ValueText == "ExpressionHolder")
                {
                    ClassToAugment = cds;
                }
            }
        }
    }
}
