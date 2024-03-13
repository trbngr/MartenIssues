// ReSharper disable MemberCanBePrivate.Global

using System.Linq.Expressions;
using System.Text.Json;
using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Frames;
using JasperFx.Core.Reflection;
using Marten.Schema;
using Marten.Schema.Identity;
using UUIDNext;

namespace MartenIssues;

public static class DeterministicId
{
    private static readonly Guid NamespaceId = Guid.Parse("18a46daa-7166-47d0-9513-656684a486a1");
    
    public static Guid NewId(object document, params string[] members)
    {
        var idContent = document
            .Take(members)
            .Then(values => JsonSerializer.Serialize(values));

        return Uuid.NewNameBased(NamespaceId, idContent);
    }

    public static Guid NewId<T>(object document, params Expression<Func<T, object?>>[] properties) =>
        NewId(document, properties.PropertyNames());
}

public class DeterministicId<T>(params Expression<Func<T, object?>>[] properties) : IIdGeneration
{
    public void GenerateCode(GeneratedMethod method, DocumentMapping mapping)
    {
        var document = new Use(mapping.DocumentType);

        var members = properties.PropertyNames().Select(n => $"\"{n}\"").Join(", ");
        var idMember = mapping.IdMember.Name;
        var generator = typeof(DeterministicId).FullNameInCode();

        var code = $$"""
                     if ({0}.{{idMember}} == Guid.Empty)
                        _setter({0}, {{generator}}.NewId({0}, {{members}}));
                     """;

        method.Frames.Code(code, document);
        method.Frames.Code($"return {{0}}.{idMember};", document);
    }

    public IEnumerable<Type> KeyTypes => [typeof(Guid)];
    public bool RequiresSequences => false;
}
