// 创建时间：2023-09-21-14:18
// 修改时间：2023-10-13-16:16

namespace LiveBoost.Views;

public partial class JggLayoutPage
{
    public JggLayoutPage()
    {
        InitializeComponent();
        var color = Color.FromScRgb(255, 160, 160, 160);
        DrawGrid(2, 2, color, 2, drawingCanvas); // 创建2x2的格子
        DrawGrid(3, 3, color, 2, drawingCanvas1); // 创建3x3的格子
        DrawGrid(4, 4, color, 2, drawingCanvas2); // 创建4x4的格子
    }

    /// <summary>
    ///     在指定的Canvas上绘制网格。
    /// </summary>
    /// <param name="rows"> 网格的行数。 </param>
    /// <param name="columns"> 网格的列数。 </param>
    /// <param name="color"> 网格线的颜色。 </param>
    /// <param name="lineWidth"> 网格线的宽度。 </param>
    /// <param name="canvas"> 要绘制网格的Canvas。 </param>
    private void DrawGrid(int rows, int columns, Color color, double lineWidth, Canvas canvas)
    {
        // 检查传入的Canvas是否为空，如果为空则抛出异常
        if (canvas == null)
        {
            throw new ArgumentNullException(nameof(canvas));
        }

        // 清除之前的绘制，准备绘制新的网格
        canvas.Children.Clear();

        // 防止无效的行列数
        if ((rows <= 0) || (columns <= 0))
        {
            return;
        }

        // 计算单元格的宽度和高度
        var cellWidth = canvas.Width / columns;
        var cellHeight = canvas.Height / rows;

        // 创建一个绘图组，用于存储网格的绘制元素
        var drawingGroup = new DrawingGroup();

        for (var row = 0; row < rows; row++)
        for (var col = 0; col < columns; col++)
        {
            // 创建一个矩形区域表示单元格
            var rect = new Rect(col * cellWidth, row * cellHeight, cellWidth, cellHeight);

            // 创建矩形几何对象
            var rectangle = new RectangleGeometry(rect);

            // 创建几何图形绘制对象，设置填充为透明，边框颜色和宽度
            var geometryDrawing = new GeometryDrawing(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(color), lineWidth), rectangle);

            // 将绘制对象添加到绘图组
            drawingGroup.Children.Add(geometryDrawing);
        }

        // 创建绘图图像
        var drawingImage = new DrawingImage(drawingGroup);

        // 创建图像控件并设置图像源
        var image = new Image
        {
            Source = drawingImage
        };

        // 将图像控件添加到Canvas
        canvas.Children.Add(image);
    }
}