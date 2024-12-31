namespace Ballware.Meta.Data.Ef.Internal;

public static class EntityExtensions
{
    public static R As<T,R>(this T source, Func<T,R> converter)
    {
      return converter(source);
    }
}
