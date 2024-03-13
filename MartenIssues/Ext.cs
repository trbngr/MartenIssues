using System.Linq.Expressions;

namespace MartenIssues;

public static class Ext
{
    public static string Join(this IEnumerable<string> values, string separator) => string.Join(separator, values);

    public static IReadOnlyDictionary<string, object> Take(this object document, params string[] members)
    {
        var type = document.GetType();

        var values = from member in members
                     let property = type.GetProperty(member)
                     let field = type.GetField(member)
                     let value = property != null ? property.GetValue(document) :
                         field != null            ? field.GetValue(document) :
                                                    throw new ArgumentException(
                                                        $"Member '{member}' does not exist in the object.")
                     select (Name: member, Value: value);

        return values.ToDictionary(x => x.Name, x => x.Value);
    }

    public static TB Then<TA, TB>(this TA a, Func<TA, TB> run)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        return run.Invoke(a);
    }

    public static string PropertyName<T>(this Expression<Func<T, object?>> expression) => expression.Body switch
    {
        MemberExpression exp => exp.Member.Name,
        UnaryExpression exp  => ((MemberExpression)exp.Operand).Member.Name,
        _                    => throw new ArgumentException("Invalid expression", nameof(expression))
    };

    public static string[] PropertyNames<T>(this IEnumerable<Expression<Func<T, object?>>> expressions)
    {
        var names = expressions.Select(PropertyName);
        return [..new HashSet<string>(names)];
    }
}