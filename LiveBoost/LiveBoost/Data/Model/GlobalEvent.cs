// 创建时间：2023-09-04-17:47
// 修改时间：2023-09-05-17:59

namespace LiveBoost.Data;

public class GlobalEvent
{
    private readonly IEventAggregator _event;

#region Global

    private static readonly Lazy<GlobalEvent> GlobalEventLazy = new(() => new GlobalEvent());

    /// <summary>
    ///     Prevents
    ///     a
    ///     default
    ///     instance
    ///     of
    ///     the
    ///     <see
    ///         cref = "GlobalEvent" />
    ///     class
    ///     from
    ///     being
    ///     created.
    ///     构造方法私有化
    /// </summary>
    private GlobalEvent() => _event = new EventAggregator();

    public static IEventAggregator Instance => GlobalEventLazy.Value._event;

#endregion
}
