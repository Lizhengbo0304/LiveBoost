// 创建时间：2023-10-10-11:17
// 修改时间：2023-10-11-11:00

namespace LiveBoost.Toolkit.Data;

public class Resolution
{
    static Resolution() =>
        Resolutions = new List<Resolution>
        {
            new(displayName: "自动"),
            new("720", "480", "720x480(NTSC)", "720x480"),
            new("720", "576", "720x480(PAL)", "720x576"),
            new("1280", "720", "1280x720(720p)", "1280x720"),
            new("1440", "1080", "1440x1080", "1440x1080"),
            new("1920", "1080", "1920x1080", "1920x1080"),
            new("2560", "1440", "2560x1440(2K)", "2560x1440"),
            new("3840", "2160", "3840x2160", "3840x2160"),
            new("4096", "2160", "4096x2160(4K)", "4096x2160"),
            new(displayName: "其他", resolutionValue: "其他")
        };

    public Resolution(string width = "", string height = "", string displayName = "", string resolutionValue = "")
    {
        ResolutionValue = resolutionValue;
        Width = width;
        Height = height;
        DisplayName = displayName;
    }
    public static List<Resolution> Resolutions { get; private set; }
#region Properties

    public string Width { get; set; }
    public string Height { get; set; }
    public string DisplayName { get; set; }
    public string ResolutionValue { get; set; }

#endregion
}
