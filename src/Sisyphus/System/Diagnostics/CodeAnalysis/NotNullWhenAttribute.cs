// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class NotNullWhenAttribute : Attribute {
    public bool ReturnValue { [UsedImplicitly] get; }

    public NotNullWhenAttribute(bool returnValue) {
        ReturnValue = returnValue;
    }
}
