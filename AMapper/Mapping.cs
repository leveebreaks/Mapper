using System.Linq.Expressions;
using System.Reflection;

namespace AMapper;

public class Mapping<TA, TB> : Mapping  where TB : new() where TA : class
{
    private bool _ignoreNotDeclared = false;
    private readonly Dictionary<string, (string, Func<object, object>)> _propertyMappings = new Dictionary<string, (string, Func<object, object>)>();
    
    public Mapping<TA, TB> AddMap(Expression<Func<TA, object>> propertyA, Expression<Func<TB, object>> propertyB, Func<object, object>? converter)
    {
        AssertPropertyExpressions(propertyA, propertyB);

        AddNewMapping(propertyA, propertyB, converter);
        
        return this;
    }

    private TB ConvertMembers(TA aInstance)
    {
        var aFields = aInstance!.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        var aProperties = aInstance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var bObj = new TB();
        foreach (var aField in aFields)
        {
            if (_propertyMappings.TryGetValue(aField.Name, out var aPropertyMapping))
            {
                var aValue = aField.GetValue(aInstance);
                var bValue = ConvertMemberValue(aValue, aPropertyMapping.Item2);
                bObj.GetType().GetField(aPropertyMapping.Item1)!.SetValue(bObj, bValue);
            }
        }

        foreach (var aProperty in aProperties)
        {
            if (_propertyMappings.TryGetValue(aProperty.Name, out var aPropertyMapping))
            {
                var aValue = aProperty.GetValue(aInstance);
                var bValue = ConvertMemberValue(aValue, aPropertyMapping.Item2);
                bObj.GetType().GetProperty(aPropertyMapping.Item1)!.SetValue(bObj, bValue, null);
            }
        }

        return bObj;
    }

    private object? ConvertMemberValue(object? aValue, Func<object, object>? converter)
    {
        if (aValue == null)
        {
            return null;
        }
        
        if (converter != null)
        {
            return converter(aValue);
        }

        return aValue;
    }

    private void AddNewMapping(Expression<Func<TA, object>> propertyA, Expression<Func<TB, object>> propertyB, Func<object, object>? converter)
    {
        var propertyAExpr = (propertyA.Body as MemberExpression) ?? (propertyA.Body as UnaryExpression)?.Operand as MemberExpression;
        var propertyBExpr =  (propertyB.Body as MemberExpression) ?? (propertyB.Body as UnaryExpression)?.Operand as MemberExpression;

        var propertyAName = "";
        var propertyBName = "";
        if (propertyAExpr != null && propertyBExpr != null)
        {
            propertyAName = propertyAExpr.Member.Name;
            propertyBName = propertyBExpr.Member.Name;
        }

        this._propertyMappings[propertyAName] = (propertyBName, converter)!;
    }

    public void IgnoreNotDeclared()
    {
        this._ignoreNotDeclared = true;
    }

    private void AssertPropertyExpressions(Expression<Func<TA, object>> propertyA,
        Expression<Func<TB, object>> propertyB)
    {
        if (propertyA == null || propertyB == null)
        {
            throw new ArgumentNullException(nameof(propertyA));
        }
        
        if (propertyB == null)
        {
            throw new ArgumentNullException(nameof(propertyB));
        }
    }

    public override TB1 Convert<TB1>(object aInstance)
    {
        return this.ConvertMembers(aInstance as TA) as TB1;
    }
}

public abstract class Mapping
{
    public abstract TB Convert<TB>(object aInstance) where TB : class;
}