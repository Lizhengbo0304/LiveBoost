// 创建时间：2023-09-15-14:43
// 修改时间：2023-09-19-14:01

namespace LiveBoost.Views;

public partial class JggPreviewAccessPage
{
    public JggPreviewAccessPage()
    {
        InitializeComponent();
        DrawGrid(2, 2, Colors.Blue, 2, drawingCanvas); // 创建2x2的蓝色格子
        DrawGrid(3, 3, Colors.Blue, 2, drawingCanvas1); // 创建2x2的蓝色格子
        DrawGrid(4, 4, Colors.Blue, 2, drawingCanvas2); // 创建2x2的蓝色格子
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        JggListView.ItemsPanel = AppConfig.Instance.PreviewIndex switch
        {
            0 => FindResource("twoItemsPanelStyle") as ItemsPanelTemplate,
            1 => FindResource("threeItemsPanelStyle") as ItemsPanelTemplate,
            2 => FindResource("fourItemsPanelStyle") as ItemsPanelTemplate,
            _ => JggListView.ItemsPanel
        };
    }

    private void DrawGrid(int rows, int columns, Color color, double lineWidth, Canvas canvas)
    {
        canvas.Children.Clear(); // 清除之前的绘制

        var cellWidth = canvas.Width / columns;
        var cellHeight = canvas.Height / rows;

        var drawingGroup = new DrawingGroup();

        for ( var row = 0; row < rows; row++ )
        {
            for ( var col = 0; col < columns; col++ )
            {
                var rect = new Rect(col * cellWidth, row * cellHeight, cellWidth, cellHeight);
                drawingGroup.Children.Add(DrawRectangle(rect, color, lineWidth));
            }
        }
        var drawingImage = new DrawingImage(drawingGroup);

        var image = new Image
        {
            Source = drawingImage
        };

        canvas.Children.Add(image);
    }

    private GeometryDrawing DrawRectangle(Rect rect, Color color, double lineWidth)
    {
        var rectangle = new RectangleGeometry(rect);
        return new GeometryDrawing(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(color), lineWidth), rectangle);
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ( JggListView is not null )
        {
            JggListView.ItemsPanel = AppConfig.Instance.PreviewIndex switch
            {
                0 => FindResource("twoItemsPanelStyle") as ItemsPanelTemplate,
                1 => FindResource("threeItemsPanelStyle") as ItemsPanelTemplate,
                2 => FindResource("fourItemsPanelStyle") as ItemsPanelTemplate,
                _ => JggListView.ItemsPanel
            };
        }
    }
}
