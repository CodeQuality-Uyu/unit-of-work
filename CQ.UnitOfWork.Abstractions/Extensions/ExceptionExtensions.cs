using System.Reflection;

namespace CQ.UnitOfWork.Abstractions.Extensions;

internal static class ExceptionExtensions
{
    public static void SetInnerException(this Exception exception, Exception innerException)
    {
        var type = typeof(Exception);
        var fieldInfo = type.GetField("_innerException", BindingFlags.Instance | BindingFlags.NonPublic);

        fieldInfo?.SetValue(exception, innerException);
    }
}
