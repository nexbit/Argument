// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Adds support for extension methods targeting .Net 2.0 when using a C# compiler >= 3.0
    /// </summary>
    /// <remarks></remarks>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class |
        AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute { }
}