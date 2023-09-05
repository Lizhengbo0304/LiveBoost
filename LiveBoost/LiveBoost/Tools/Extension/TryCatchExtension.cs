// 创建时间：2023-09-04-17:23
// 修改时间：2023-09-05-17:59

namespace LiveBoost.Tools;

public static class TryCatchExtension
{
    public static Func<TResult> TryCatchFunc<TResult>(this Func<TResult> tryFunc, Func<Exception, TResult> catchFunc)
    {
        return () =>
        {
            try
            {
                return tryFunc();
            }
            catch ( Exception ex )
            {
                return catchFunc(ex);
            }
        };
    }
    public static Action TryCatchAction(this Action tryAction, Action<Exception> catchAction)
    {
        return () =>
        {
            try
            {
                tryAction();
            }
            catch ( Exception ex )
            {
                catchAction(ex);
            }
        };
    }
    public static async Task<T> TryCatchFunc<T>(this Func<Task<T>> func, Func<Exception, T> catchFunc)
    {
        try
        {
            return await func();
        }
        catch ( Exception ex )
        {
            return catchFunc(ex);
        }
    }

    public static async Task TryCatchAction(this Func<Task> action, Action<Exception> catchAction)
    {
        try
        {
            await action();
        }
        catch ( Exception e )
        {
            catchAction(e);
        }
    }
}
