namespace LiveBoost.Toolkit.Tools;

public static class BrushHelper
{
    /// <summary>
    /// 根据种子生成随机颜色。
    /// </summary>
    /// <param name="seed">用于生成颜色的种子。</param>
    /// <returns>生成的随机颜色的 Brush。</returns>
    public static SolidColorBrush GetRandomColorBrush(int seed)
    {
        // 创建 Random 对象，并使用传入的种子
        var random = new Random(seed);

        // 生成随机的RGB颜色
        var red = (byte)random.Next(256);
        var green = (byte)random.Next(256);
        var blue = (byte)random.Next(256);

        // 创建Color对象
        var randomColor = Color.FromRgb(red, green, blue);

        // 创建SolidColorBrush对象
        var randomColorBrush = new SolidColorBrush(randomColor);

        return randomColorBrush;
    }
    /// <summary>
    /// 获取字符串的哈希值映射为一个有符号整数。
    /// </summary>
    /// <param name="input">输入字符串。</param>
    /// <returns>哈希值的有符号整数表示。</returns>
    public static int GetHashAsInt(this string input)
    {
        // 创建SHA256哈希算法实例
        using var sha256 = SHA256.Create();
        // 将字符串转换为字节数组
        var inputBytes = Encoding.UTF8.GetBytes(input);

        // 计算哈希值
        var hashBytes = sha256.ComputeHash(inputBytes);

        // 取前四个字节并转换为int
        var hashValue = BitConverter.ToInt32(hashBytes, 0);

        return hashValue;
    }
}
