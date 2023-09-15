// 创建时间：2023-09-15-14:56
// 修改时间：2023-09-15-15:41

#region

using LiveBoost.Controls;

#endregion

namespace LiveBoost.ViewModels;

public partial class JggMainWindowVm
{
#region Properties

    public List<CombinationItem> RecordItems { get; set; } = new()
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
