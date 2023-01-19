using System.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Inoculator.Core;

namespace Inoculator.Builder;

public class Metadata : Printable<Metadata> {
    public Object EmbededResource { get; set; }
    public Metadata(MethodDecl.Method source) => Code = source;
    public Metadata(string sourceCode) => Code = Reader.Parse<MethodDecl.Method>(sourceCode) switch {
        Success<MethodDecl.Method, Exception> success => success.Value,
        Error<MethodDecl.Method, Exception> failure => throw failure.Message,
    };
    public IdentifierDecl.Identifier ClassName {get; set;}
    public String Name => Code.Header.Name.ToString();
    [JsonIgnore]
    public (String[] Input, String Output)  Signature 
        => (
            Code.Header.Parameters.Parameters.Values.Length > 0 
                ? Code.Header.Parameters.Parameters.Values.Select(
                    x => x switch {
                        ParameterDecl.DefaultParameter p => p.TypeDeclaration.ToString(),
                        ParameterDecl.VarargParameter p => "...",
                    }).ToArray() 
                : new string[1] { "void" },
            Code.Header.Type.ToString()
        );

    public bool IsAsync => Code.Body.Items.Values.OfType<MethodDecl.CustomAttributeItem>().Any(a => a.Value.AttributeCtor.Spec.ToString() == "[System.Runtime] System.Runtime.CompilerServices.AsyncStateMachineAttribute");
    public bool IsStatic => Code.Header.Convention is null || Code.Header.MethodAttributes.Attributes.Values.Any(a => a is AttributeDecl.MethodSimpleAttribute { Name: "static" });
    public string TypeSignature => $"({string.Join(", ", Signature.Input)} -> {Signature.Output})";
    public string[] TypeParameters => Code.Header?.TypeParameters?
        .Parameters.Values
        .Select(x => x.Id.ToString())
        .ToArray() ?? new string[0];
    public Object?[] Parameters { get; set; }
    public Exception Exception{ get; set; }
    public Object? ReturnValue { get; set; }
    [JsonIgnore]
    public MethodDecl.Method Code { get; set; }

    public Result<MethodDecl.Method[], Exception> ReplaceNameWith(String name, string[] attributeName) {
        var newMethod = Wrapper.Handle(this, ClassName, attributeName);
        switch(newMethod) {
            case Error<MethodDecl.Method, Exception> e_method :
                return Error<MethodDecl.Method[], Exception>.From(new Exception($"failed to parse new method\n{e_method.Message}"));
        }

        var n_method = newMethod as Success<MethodDecl.Method, Exception>;

        if(!IsAsync) {
            var renamedMethod = Reader.Parse<MethodDecl.Method>(Code.ToString().Replace(Name, name));
            switch(renamedMethod) {
                case Error<MethodDecl.Method, Exception> e_method :
                    return Error<MethodDecl.Method[], Exception>.From(new Exception($"failed to parse modified old method\n{e_method.Message}"));
                case Success<MethodDecl.Method, Exception> o_method :
                    return Success<MethodDecl.Method[], Exception>.From(new[] { o_method.Value, n_method.Value });
            }

        } 
        return Success<MethodDecl.Method[], Exception>.From(new[] { n_method.Value });
    }
}