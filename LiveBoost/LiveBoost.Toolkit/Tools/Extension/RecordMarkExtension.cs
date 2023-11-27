namespace LiveBoost.ToolKit.Tools;

public static class RecordMarkExtension
{
    /// <summary>
    ///     隐式转换操作符，将RecordMark对象转换为RecordFile对象
    /// </summary>
    /// <param name="mark"> 要转换的RecordMark对象 </param>
    public static  RecordFile CastToRecordFile(this RecordMark mark)
    {
        // 如果mark.Parent为null，则使用mark的一部分属性初始化RecordFile对象
        // 否则，使用mark和mark.Parent的一部分属性初始化RecordFile对象
        var recordFile = new RecordFile
        {
            Id = mark.Parent is null ? mark.Id : mark.RecordId,
            IsSub = true,
            Url = mark.Parent?.Url ?? mark.Url,
            Thumb = mark.Thumb,
            Name = mark.Name,
            Type = mark.Parent?.Type ?? 2,
            Status = mark.Parent?.Status ?? 0,
            // 尝试解析时间戳并赋值到RecordFile对象的相应属性
            InPoint = (mark.InPoint?.Substring(0, 8)).ParseTimeSpan(),
            RealInPoint = mark.InPoint.ParseTimeSpan(),
            OutPoint = (mark.OutPoint?.Substring(0, 8)).ParseTimeSpan(),
            RealOutPoint = mark.OutPoint.ParseTimeSpan()
        };

        // 如果mark.Parent不为null，则将mark.Parent的一部分属性赋值到RecordFile对象
        if (mark.Parent is null)
        {
            return recordFile;
        }

        recordFile.CreateDate = mark.Parent.CreateDate;
        recordFile.CreateUser = mark.Parent.CreateUser;
        recordFile.Stream = mark.Parent.Stream;
        recordFile.ParentId = mark.Parent.ParentId;
        recordFile.ParentIds = mark.Parent.ParentIds;

        return recordFile;
    }
}
