using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

// ReSharper disable CheckNamespace
/// <summary>
/// Provides methods for verification of argument preconditions.
/// </summary>
public static partial class Argument
{
    // ReSharper restore CheckNamespace   

    /// <summary>
    /// Checks whether the specified <paramref name="type"/> inherits from the <paramref name="baseType"/>.
    /// </summary>
    /// <remarks>
    /// 	<para>The inheritance tree doesn't include implemented interfaces, so use
    /// this method only to see if a type inherits from some base class type. To check
    /// whether a type implements a specific interface use   .</para>
    /// 	<para>This method cannot be used with Code Contracts.</para>
    /// </remarks>
    /// <param name="paramName">Name of the parameter.</param>
    /// <param name="type">The type to check.</param>
    /// <param name="baseType">The base type to check.</param>
    /// <exception cref="System.ArgumentException">ArgumentException</exception>
    /// <exception cref="ArgumentException">The <paramref name="paramName"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="baseType"/> is <c>null</c>.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static void InheritsFrom(string paramName, Type type, Type baseType)
    {
        NotNull("type", type);
        NotNull("baseType", baseType);

        var runtimeBaseType = type.GetBaseTypeEx();

        do
        {
            if (runtimeBaseType == baseType)
            {
                return;
            }

            // Prevent some endless while loops
            if (runtimeBaseType == typeof(Object))
            {
                // Break, no return because this should cause an exception
                break;
            }

            runtimeBaseType = type.GetBaseTypeEx();
        } while (runtimeBaseType != null);

        throw new ArgumentException(string.Format("Type '{0}' should have type '{1}' as base class, but does not", type.Name, baseType.Name), paramName);
    }

    #region ImplementsInterface methods
    /// <summary>
    /// Checks whether the specified <paramref name="instance" /> implements the specified <paramref name="interfaceType" />.
    /// </summary>
    /// <remarks>This method cannot be used with Code Contracts.</remarks>
    /// <param name="paramName">Name of the param.</param>
    /// <param name="instance">The instance to check.</param>
    /// <param name="interfaceType">The type of the interface to check for.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="instance" /> does not implement the <paramref name="interfaceType" />.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static void ImplementsInterface(string paramName, object instance, Type interfaceType)
    {
        NotNull("instance", instance);

        ImplementsInterface(paramName, instance.GetType(), interfaceType);
    }

    /// <summary>
    /// Checks whether the specified <paramref name="instance" /> implements the specified <typeparamref name="TInterface" />.
    /// </summary>
    /// <remarks>This method cannot be used with Code Contracts.</remarks>
    /// <typeparam name="TInterface">The type of the T interface.</typeparam>
    /// <param name="paramName">Name of the param.</param>
    /// <param name="instance">The instance to check.</param>
    /// <exception cref="ArgumentException">The <paramref name="paramName" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static void ImplementsInterface<TInterface>(string paramName, object instance)
        where TInterface : class
    {
        var interfaceType = typeof(TInterface);

        ImplementsInterface(paramName, instance, interfaceType);
    }

    /// <summary>
    /// Checks whether the specified <paramref name="type" /> implements the specified <paramref name="interfaceType" />.
    /// </summary>
    /// <remarks>This method cannot be used with Code Contracts.</remarks>
    /// <param name="paramName">Name of the param.</param>
    /// <param name="type">The type to check.</param>
    /// <param name="interfaceType">The type of the interface to check for.</param>
    /// <exception cref="System.ArgumentException">type</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="interfaceType" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="type" /> does not implement the <paramref name="interfaceType" />.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static void ImplementsInterface(string paramName, Type type, Type interfaceType)
    {
        NotNull("type", type);
        NotNull("interfaceType", interfaceType);

        foreach (Type typeToCheck in type.GetInterfacesEx())
        {
            if (typeToCheck == interfaceType)
                return;
        }

        throw new ArgumentException(string.Format("Type '{0}' should implement interface '{1}', but does not", type.Name, interfaceType.Name), "type");
    }

    /// <summary>
    /// Checks whether the specified <paramref name="instance" /> implements at least one of the specified <paramref name="interfaceTypes" />.
    /// </summary>
    /// <remarks>This method cannot be used with Code Contracts.</remarks>
    /// <param name="paramName">Name of the param.</param>
    /// <param name="instance">The instance to check.</param>
    /// <param name="interfaceTypes">The types of the interfaces to check for.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="interfaceTypes" /> is <c>null</c> or an empty array.</exception>
    /// <exception cref="ArgumentException">The <paramref name="instance" /> does not implement at least one of the <paramref name="interfaceTypes" />.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static void ImplementsOneOfTheInterfaces(string paramName, object instance, Type[] interfaceTypes)
    {
        NotNull("instance", instance);

        ImplementsOneOfTheInterfaces(paramName, instance.GetType(), interfaceTypes);
    }

    /// <summary>
    /// Checks whether the specified <paramref name="type" /> implements at least one of the the specified <paramref name="interfaceTypes" />.
    /// </summary>
    /// <remarks>This method cannot be used with Code Contracts.</remarks>
    /// <param name="paramName">Name of the param.</param>
    /// <param name="type">The type to check.</param>
    /// <param name="interfaceTypes">The types of the interfaces to check for.</param>
    /// <exception cref="System.ArgumentException">type</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="interfaceTypes" /> is <c>null</c> or an empty array.</exception>
    /// <exception cref="ArgumentException">The <paramref name="type" /> does not implement the <paramref name="interfaceTypes" />.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static void ImplementsOneOfTheInterfaces(string paramName, Type type, Type[] interfaceTypes)
    {
        NotNull("type", type);
        NotNullOrEmpty("interfaceTypes", interfaceTypes);

        foreach (Type typeToCheck in interfaceTypes)
        {
            foreach (Type iType in type.GetInterfacesEx())
            {
                if (iType == typeToCheck) return;
            }
        }

        var errorBuilder = new StringBuilder();
        errorBuilder.AppendLine("Type '{0}' should implement at least one of the following interfaces, but does not:");
        foreach (var interfaceType in interfaceTypes)
        {
            errorBuilder.AppendLine("  * " + interfaceType.FullName);
        }

        throw new ArgumentException(errorBuilder.ToString(), "type");
    }
    #endregion


}

internal static class TypeExtensions
{
    /// <summary>
    /// Gets the interfaces implemented by type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A collection of Type.</returns>
    /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
    public static IEnumerable<Type> GetInterfacesEx(this Type type)
    {
        Argument.NotNull("type", type);
        
#if NETFX_CORE || PORTABLE
        // Use extension method explicitly for .net 2 compatibility (avoid the using System.Linq)
        return System.Linq.Enumerable.ToArray(type.GetTypeInfo().ImplementedInterfaces);
#else
        return type.GetInterfaces();
#endif
    }

    /// <summary>
    /// The get base type of the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Type.</returns>
    /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
    public static Type GetBaseTypeEx(this Type type)
    {
        Argument.NotNull("type", type);

#if NETFX_CORE || PORTABLE
        return type.GetTypeInfo().BaseType;
#else
        return type.BaseType;
#endif
    }
}