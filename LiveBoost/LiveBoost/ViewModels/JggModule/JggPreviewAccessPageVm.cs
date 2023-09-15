// 创建时间：2023-09-15-14:56
// 修改时间：2023-09-15-14:56

using LiveBoost.Controls;

namespace LiveBoost.ViewModels;

public partial class JggMainWindowVm
{
    #region Properties
    public List<CombinationItem> RecordItems{ get; set; } = new()
    {
        new CombinationItem(),
        new CombinationItem(),
        new CombinationItem(),
        new CombinationItem(),
        new CombinationItem(),
        new CombinationItem(),
        new CombinationItem(),
        new CombinationItem(),
        new CombinationItem()
    };
    #endregion
}
