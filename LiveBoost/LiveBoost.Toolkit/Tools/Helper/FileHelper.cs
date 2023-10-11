// 创建时间：2023-09-07-15:33
// 修改时间：2023-10-11-11:00

namespace LiveBoost.Toolkit.Tools;

public static class FileHelper
{
    /// <summary>
    ///     检查文件名是否包含非法字符
    /// </summary>
    /// <param name = "fileName" > 文件名 </param>
    /// <returns> 如果文件名包含非法字符，则返回 true；否则返回 false </returns>
    public static bool HasInvalidSymbol(this string fileName) =>
        fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
        fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0;

    /// <summary>
    ///     获取新增文件（夹）检测重名后的新名称
    /// </summary>
    /// <param name = "path" > 目标路径 </param>
    /// <param name = "itemType" > 1=文件夹, 2=文件 </param>
    /// <param name = "name" > 默认名称 </param>
    /// <param name = "extension" > type==2 时使用，文件的扩展名 </param>
    /// <returns> 最新名称 </returns>
    public static string GetNewNameInPath(this string path, int itemType = 1, string name = "新建文件夹", string? extension = null)
    {
        if ( !Directory.Exists(path) )
        {
            // 如果目标路径不存在，则直接返回默认名称和扩展名（如果有）
            return $"{name}{extension}";
        }

        IEnumerable<string> names = itemType == 1
            ? Directory.GetDirectories(path) // 获取目标路径下的文件夹列表
            : Directory.GetFiles(path); // 获取目标路径下的文件列表

        var index = 0;
        var newName = itemType == 1
            ? $"{name}"
            : $"{name}{extension}";

        // 检查是否存在重名的文件或文件夹，如果存在，则在名称后添加索引直到找到一个可用的名称
        while ( names.Any(it => string.Equals(Path.GetFileName(it), newName, StringComparison.OrdinalIgnoreCase)) )
        {
            index++;
            newName = itemType == 1
                ? $"{name}({index})"
                : $"{name}({index}){extension}";
        }

        return newName;
    }

    /// <summary>
    ///     异步获取新增文件（夹）检测重名后的新名称
    /// </summary>
    /// <param name = "path" > 目标路径 </param>
    /// <param name = "itemType" > 1=文件夹, 2=文件 </param>
    /// <param name = "name" > 默认名称 </param>
    /// <param name = "extension" > type==2 时使用，文件的扩展名 </param>
    /// <returns> 最新名称的异步任务 </returns>
    public static async Task<string> GetNewNameInPathAsync(this string path, int itemType = 1, string name = "新建文件夹", string? extension = null)
    {
        return await Task.Run(() => path.GetNewNameInPath(itemType, name, extension));
    }

    /// <summary>
    ///     获取在给定路径中不带扩展名的新名称（异步版本）。
    /// </summary>
    /// <param name = "path" > 给定的路径。 </param>
    /// <param name = "type" > 类型（默认为1）。 </param>
    /// <param name = "name" > 名称（默认为"新建文件夹"）。 </param>
    /// <param name = "extension" > 扩展名。 </param>
    /// <returns> 不带扩展名的新名称。 </returns>
    public static async Task<string> GetNewNameInPathWithoutExtensionAsync(this string path, int type = 1,
        string name = "新建文件夹", string? extension = null)
    {
        return await Task.Run(() => path.GetNewNameInPathWithoutExtension(type, name, extension));
    }

    /// <summary>
    ///     获取在给定路径中不带扩展名的新名称。
    /// </summary>
    /// <param name = "path" > 给定的路径。 </param>
    /// <param name = "type" > 类型（默认为1）。 </param>
    /// <param name = "name" > 名称（默认为"新建文件夹"）。 </param>
    /// <param name = "extension" > 扩展名。 </param>
    /// <returns> 不带扩展名的新名称。 </returns>
    public static string GetNewNameInPathWithoutExtension(this string path, int type = 1, string name = "新建文件夹",
        string? extension = null)
    {
        if ( !Directory.Exists(path) )
        {
            // 路径不存在，直接返回给定的名称
            return name;
        }
        IEnumerable<string> names = type == 1
            ? Directory.GetDirectories(path) // 获取目标路径下的文件夹列表
            : Directory.GetFiles(path); // 获取目标路径下的文件列表

        var index = 0;
        var newName = type == 1
            ? $"{name}"
            : $"{name}{extension}";

        // 检查是否存在重名的文件或文件夹，如果存在，则在名称后添加索引直到找到一个可用的名称
        while ( names.Any(it => string.Equals(Path.GetFileName(it), newName, StringComparison.OrdinalIgnoreCase)) )
        {
            index++;
            newName = type == 1
                ? $"{name}({index})"
                : $"{name}({index}){extension}";
        }

        return Path.GetFileNameWithoutExtension(newName);
    }

    /// <summary>
    ///     异步获取文件夹的大小（包括所有子文件夹和文件）
    /// </summary>
    /// <param name = "folderPath" > 文件夹路径 </param>
    /// <returns> 文件夹的大小（字节数） </returns>
    public static async Task<double> GetFolderSizeAsync(this string folderPath) => await Task.Run(folderPath.GetFolderSize);

    /// <summary>
    ///     获取文件夹的大小（包括所有子文件夹和文件）
    /// </summary>
    /// <param name = "folderPath" > 文件夹路径 </param>
    /// <returns> 文件夹的大小（字节数） </returns>
    public static long GetFolderSize(this string folderPath)
    {
        long size = 0;

        try
        {
            // 获取文件夹下的所有文件
            var files = Directory.GetFiles(folderPath);

            // 累加文件大小，使用并行处理提高性能
            Parallel.ForEach(files, file =>
            {
                var fileInfo = new FileInfo(file);
                Interlocked.Add(ref size, fileInfo.Length);
            });

            // 获取文件夹下的所有子文件夹
            string[] subFolders = Directory.GetDirectories(folderPath);

            // 递归遍历子文件夹并累加文件大小
            Parallel.ForEach(subFolders, subFolder =>
            {
                Interlocked.Add(ref size, GetFolderSize(subFolder));
            });
        }
        catch ( Exception )
        {
            return size;
        }

        return size;
    }


    /// <summary>
    ///     获取传入路径的文件大小（同步方法）
    /// </summary>
    /// <param name = "filePath" > 文件路径 </param>
    /// <returns> 文件的大小（字节数） </returns>
    public static double GetFileSize(this string filePath)
    {
        try
        {
            var file = new FileInfo(filePath);
            return file.Length;
        }
        catch ( Exception ex )
        {
            ex.LogFileError("获取传入路径空间异常");
            return 0;
        }
    }

    /// <summary>
    ///     异步获取传入路径的文件大小
    /// </summary>
    /// <param name = "filePath" > 文件路径 </param>
    /// <returns> 文件的大小（字节数） </returns>
    public static async Task<double> GetFileSizeAsync(this string filePath) => await Task.Run(filePath.GetFileSize);

    /// <summary>
    ///     判断文件是否被占用
    /// </summary>
    /// <param name = "fileName" > 文件地址 </param>
    /// <returns> 是否被占用（true表示正在使用，false表示没有使用） </returns>
    public static bool IsFileInUse(this string fileName)
    {
        // 默认文件被占用
        var inUse = true;

        try
        {
            // 使用 using 语句创建 FileStream 对象，确保资源在使用完毕后被正确释放
            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            // 如果成功打开文件，则表示文件没有被占用
            inUse = false;
        }
        catch
        {
            // 忽略异常，继续执行
        }

        return inUse;
    }
}
