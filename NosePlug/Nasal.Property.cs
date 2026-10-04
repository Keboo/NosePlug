
/* Unmerged change from project 'NosePlug (net48)'
Before:
using NosePlug.Plugs;
using System;
After:
using System;
*/

/* Unmerged change from project 'NosePlug (net472)'
Before:
using NosePlug.Plugs;
using System;
After:
using System;
*/

/* Unmerged change from project 'NosePlug (netcoreapp3.1)'
Before:
using NosePlug.Plugs;
using System;
After:
using System;
*/
using System.Linq.Expressions;
using NosePlug.Plugs;

namespace NosePlug;

partial class Nasal
{
    /// <summary>
    /// Creates a plug for a property.
    /// </summary>
    /// <typeparam name="TProperty">The return type of the property</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> to create a plug for.</param>
    /// <returns>A new property plug</returns>
    /// <exception cref="ArgumentNullException">When the passed in <see cref="PropertyInfo"/> is <c>null</c></exception>
    public static IPropertyPlug<TProperty> Property<TProperty>(PropertyInfo property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return new PropertyPlug<TProperty>(property);
    }

    /// <summary>
    /// Creates a plug for a property
    /// </summary>
    /// <typeparam name="T">The type that contains the property</typeparam>
    /// <typeparam name="TProperty">The type of the property</typeparam>
    /// <param name="name">The name of the property</param>
    /// <returns>A new property plug</returns>
    /// <exception cref="ArgumentNullException">Thrown when the passed <see cref="Nasal"/> instance is <c>null</c></exception>
    /// <exception cref="ArgumentException">Thrown when the passed string for the property name is <c>null</c> or whitespace</exception>
    /// <exception cref="MissingMemberException">Thrown when the propery could not be found</exception>
    public static IPropertyPlug<TProperty> Property<T, TProperty>(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        var property = typeof(T).GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty)
            ?? throw new MissingMemberException($"Could not find property '{name}' on type '{typeof(T).FullName}'");
        return Property<TProperty>(property);
    }

    /// <summary>
    /// Creates a plug for a property
    /// </summary>
    /// <typeparam name="TProperty">The type of the property</typeparam>
    /// <param name="propertyExpression">An expression representing the property to create a plug for</param>
    /// <returns>A new property plug</returns>
    /// <exception cref="ArgumentNullException">Thrown when the passed <see cref="Nasal"/> instance is <c>null</c></exception>
    /// <exception cref="ArgumentException">Thrown when the passed expression is not a property expression</exception>
    public static IPropertyPlug<TProperty> Property<TProperty>(Expression<Func<TProperty>> propertyExpression)
    {
        if (propertyExpression is null)
        {
            throw new ArgumentNullException(nameof(propertyExpression));
        }

        if (propertyExpression.Body is not MemberExpression memberExpression ||
            memberExpression.Member is not PropertyInfo propertyInfo)
        {
            throw new ArgumentException("Expresion is not a member expression to a property");
        }

        return Property<TProperty>(propertyInfo);
    }

}
