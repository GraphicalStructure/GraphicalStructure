﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;


namespace GraphicalStructure
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        private Canvas canvas;
        private Line centerLine;
        private SolidColorBrush color = new SolidColorBrush();
        private double thickness = 1;
        private Point startPosition;
        private System.Windows.Shapes.Path insertShape;
        private double currentX;
        private double currentY;
        private double allWidth = 0;
        private double maxHeight = 0;
        private StackPanel stackpanel;
        private Shape currentShape;
        private ArrayList isRotate;
        private ArrayList components;
        public Components currentComp;
        private static string saveFileName = null;
        private static bool isHaveLeftEndCap = false;
        private static bool isHaveRightEndCap = false;
        private static bool isHaveCentralTube = false;

        private Label totalWidthLabel;
        private Label totalHeightLabel;

        private ArrayList explosions;
        private Hashtable explosionData;

        private ArrayList centralCubes;
        private StackPanel centralCubePanel;
        private int centralCubeIndex;

        private int curLayerNum = 0;

        private ContextMenu aMenu;

        private List<Dictionary<string, Dictionary<string, string>>> DataBaseMaterials = new List<Dictionary<string, Dictionary<string, string>>>();

        //双击进入编辑界面是段还是中心管
        private int changeTitle = 0;

        public MainWindow()
        {
            InitializeComponent();
            initWindow();
        }

        private void NewProgramButton_Click(object sender, RoutedEventArgs e)
        {
            initWindow();
            //初始化默认添加一个段
            addCylindrical_Click(null, null);
            
        }

        private void ColseProgramButton_Click(object sender, RoutedEventArgs e)
        {
            if (components != null && components.Count != 0)
            {
                if (saveFileName == null)
                {
                    MessageBoxResult result = MessageBox.Show("文件还未保存，是否保存?",
                    "警告", MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.OK)
                    {
                        //保存文件
                        Store_Click(sender, e);
                    }
                    else
                    {
                        initWindow();
                    }
                }
                else
                {
                    initWindow();
                }
            }
            else
            {
                initWindow();
            }
        }

        private void initWindow()
        {
            if (canvas != null && canvas.Children.Count != 0)
            {
                canvas.Children.Clear();
            }
            canvas = new Canvas();
            canvas = front_canvas;
            canvas.Width = this.Width - 20;
            canvas.Height = this.Height - 28;
            canvas.ClipToBounds = true;

            //中心线
            centerLine = new Line();
            centerLine.X1 = 10;
            centerLine.Y1 = canvas.Height / 2;
            centerLine.X2 = canvas.Width - 20;
            centerLine.Y2 = canvas.Height / 2;
            centerLine.Stroke = Brushes.Red;
            centerLine.StrokeThickness = 1;
            canvas.Children.Add(centerLine);

            stackpanel = new StackPanel();
            stackpanel.Margin = new Thickness(0, 0, 0, 0);
            stackpanel.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            stackpanel.Width = 0;
            stackpanel.Height = 100;
            stackpanel.Orientation = Orientation.Horizontal;
            canvas.Children.Add(stackpanel);

            isRotate = new ArrayList();

            components = new ArrayList();

            currentX = 0;
            currentY = 0;

            explosions = new ArrayList();
            explosionData = new Hashtable();
            centralCubes = new ArrayList();
            centralCubePanel = new StackPanel();
            centralCubePanel.Width = 0;
            centralCubePanel.Height = 270;
            centralCubePanel.Orientation = Orientation.Horizontal;
            canvas.Children.Add(centralCubePanel);

            saveFileName = null;
            isHaveLeftEndCap = false;
            isHaveRightEndCap = false;
            isHaveCentralTube = false;

            insertShape = null;
            curLayerNum = 0;

            ColorProc.clearMap();

            //
            aMenu = new ContextMenu();
            //MenuItem addCompMenu = new MenuItem();
            //addCompMenu.Header = "添加段";
            //addCompMenu.Click += addCylindrical_Click;
            //aMenu.Items.Add(addCompMenu);
            //MenuItem CoverMenu = new MenuItem();
            //CoverMenu.Header = "端盖";
            // aMenu.Items.Add(CoverMenu);
            MenuItem LeftCoverMenu = new MenuItem();
            LeftCoverMenu.Header = "左端盖";
            //LeftCoverMenu.Click += ShapeButton_Click;
            aMenu.Items.Add(LeftCoverMenu);
            MenuItem addLeftCoverMenu = new MenuItem();
            addLeftCoverMenu.Header = "添加左端盖";
            addLeftCoverMenu.Click += ShapeButton_Click;
            LeftCoverMenu.Items.Add(addLeftCoverMenu);
            MenuItem delLeftCoverMenu = new MenuItem();
            delLeftCoverMenu.Header = "删除左端盖";
            delLeftCoverMenu.Click += del_Cover;
            LeftCoverMenu.Items.Add(delLeftCoverMenu);
            MenuItem RightCoverMenu = new MenuItem();
            RightCoverMenu.Header = "右端盖";
            //RightCoverMenu.Click += ShapeButton_Click;
            aMenu.Items.Add(RightCoverMenu);
            MenuItem addRightCoverMenu = new MenuItem();
            addRightCoverMenu.Header = "添加右端盖";
            addRightCoverMenu.Click += ShapeButton_Click;
            RightCoverMenu.Items.Add(addRightCoverMenu);
            MenuItem delRightCoverMenu = new MenuItem();
            delRightCoverMenu.Header = "删除右端盖";
            delRightCoverMenu.Click += del_Cover;
            RightCoverMenu.Items.Add(delRightCoverMenu);
            //CoverMenu.Items.Add(addLeftCoverMenu);
            //CoverMenu.Items.Add(addRightCoverMenu);
            MenuItem CentralTubeMenu = new MenuItem();
            CentralTubeMenu.Header = "中心管";
            aMenu.Items.Add(CentralTubeMenu);
            MenuItem addCentralTubeMenu = new MenuItem();
            addCentralTubeMenu.Header = "添加中心管";
            addCentralTubeMenu.Click += addCentralTubeMenu_Click;
            CentralTubeMenu.Items.Add(addCentralTubeMenu);
            MenuItem deleteCentralTubeMenu = new MenuItem();
            //deleteCentralTubeMenu.Header = "删除空心管";
            //deleteCentralTubeMenu.Click += deleteCentralTubeMenu_Click;
            //CentralTubeMenu.Items.Add(deleteCentralTubeMenu);
            MenuItem editCentralTubeMenu = new MenuItem();
            editCentralTubeMenu.Header = "编辑中心管";
            editCentralTubeMenu.Click += editCubeMenu_Click;
            CentralTubeMenu.Items.Add(editCentralTubeMenu);
            MenuItem explosionMenu = new MenuItem();
            explosionMenu.Header = "起爆点";
            aMenu.Items.Add(explosionMenu);
            MenuItem addExplosionMenu = new MenuItem();
            addExplosionMenu.Header = "添加起爆点";
            addExplosionMenu.Click += addExplosionMenu_Click;
            explosionMenu.Items.Add(addExplosionMenu);
            //MenuItem deleteExplosionMenu = new MenuItem();
            //deleteExplosionMenu.Header = "删除起爆点";
            //deleteExplosionMenu.Click += deleteExplosionMenu_Click;
            //explosionMenu.Items.Add(deleteExplosionMenu);
            MenuItem editExplosionMenu = new MenuItem();
            editExplosionMenu.Header = "编辑起爆点";
            editExplosionMenu.Click += editExplosionMenu_Click;
            explosionMenu.Items.Add(editExplosionMenu);
            //MenuItem addCopyMenu = new MenuItem();
            //addCopyMenu.Header = "截图";
            //addCopyMenu.Click += addCopy_Click;
            //aMenu.Items.Add(addCopyMenu);

            this.canvas.ContextMenu = aMenu;


            autoResize();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // calculates incorrect when window is maximized
            canvas.Width = this.ActualWidth - 20;
            canvas.Height = this.ActualHeight - 28;

            centerLine.X1 = 10;
            centerLine.Y1 = canvas.Height / 2;
            centerLine.X2 = canvas.Width - 20;
            centerLine.Y2 = canvas.Height / 2;
            if (e.HeightChanged)
            {
                double hChanged = e.PreviousSize.Height - e.NewSize.Height;
                for (int i = 0; i < canvas.Children.Count; i++)
                {
                    if (canvas.Children[i] is System.Windows.Shapes.Path)
                    {
                        System.Windows.Shapes.Path path = (System.Windows.Shapes.Path)canvas.Children[i];
                        //Canvas.SetTop(path, Canvas.GetTop(path) - hChanged);
                        ColorProc.moveVertical(path, -hChanged / 2.0);
                    }
                }
            }
            autoResize();
        }

        //canvas 随滚轮变换大小
        private double _zoomValue = 1.0;
        private void UIElement_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                _zoomValue += 0.1;
            }
            else
            {
                if (_zoomValue <= 0.2)
                {
                    return;
                }
                _zoomValue -= 0.1;
            }

            ScaleTransform scale = new ScaleTransform(_zoomValue, _zoomValue);
            scale.CenterX = this.canvas.Width / 2.0;
            scale.CenterY = this.canvas.Height / 2.0;
            canvas.LayoutTransform = scale;
            e.Handled = true;
            //var element = sender as UIElement;
            //var position = e.GetPosition(element);
            //var transform = element.RenderTransform as MatrixTransform;
            //var matrix = transform.Matrix;
            //var scale = e.Delta >= 0 ? 1.01 : 1/1.01; // choose appropriate scaling factor
            //matrix.ScaleAtPrepend(scale, scale, position.X, position.Y);
            //transform.Matrix = matrix;
            //e.Handled = true;
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            color = (SolidColorBrush)((Rectangle)(sender as RadioButton).Content).Fill;
            insertShape.Fill = color;
        }

        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            if (mi.Header.ToString() == "添加左端盖")
            {
                Components cps;
                //保持左右连接  如果存在第一个段获取它的高度
                if (stackpanel.Children.Count != 0)
                {
                    double height = 0;
                    GeometryGroup geometryGroup = (GeometryGroup)((System.Windows.Shapes.Path)stackpanel.Children[0]).Data;
                    if (geometryGroup.Children.Count == 1)
                    {
                        PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
                        PathFigure curPf = curPg.Figures.ElementAt(0);
                        height = ((LineSegment)curPf.Segments[0]).Point.Y - curPf.StartPoint.Y;
                    }
                    else
                    {
                        PathGeometry t_curPg = (PathGeometry)geometryGroup.Children[geometryGroup.Children.Count - 2];
                        PathFigure t_curPf = t_curPg.Figures.ElementAt(0);
                        PathGeometry b_curPg = (PathGeometry)geometryGroup.Children[geometryGroup.Children.Count - 1];
                        PathFigure b_curPf = b_curPg.Figures.ElementAt(0);
                        if (t_curPf.Segments[2] is LineSegment)
                        {
                            height = ((LineSegment)b_curPf.Segments[0]).Point.Y - ((LineSegment)t_curPf.Segments[2]).Point.Y;
                        }
                        else if (t_curPf.Segments[2] is ArcSegment)
                        {
                            height = ((LineSegment)b_curPf.Segments[0]).Point.Y - ((ArcSegment)t_curPf.Segments[2]).Point.Y;
                        }
                        else
                        {
                            height = ((LineSegment)b_curPf.Segments[0]).Point.Y - ((PolyLineSegment)t_curPf.Segments[2]).Points[((PolyLineSegment)t_curPf.Segments[2]).Points.Count - 1].Y;
                        }
                    }

                    cps = new Components(new Point(0, 100), new Point(0, 100 + height), new Point(40, 100 + height), new Point(40, 100));
                }
                else
                {
                    cps = new Components(new Point(0, 100), new Point(0, 220), new Point(40, 220), new Point(40, 100));
                }

                SolidColorBrush blackBrush = new SolidColorBrush();
                blackBrush.Color = Colors.Blue;
                cps.newPath.Fill = blackBrush;
                components.Insert(0, cps);
                cps.newPath.MouseRightButtonDown += viewbox_MouseRightButtonDown;
                cps.newPath.MouseDown += Img1_MouseLeftButtonDown;
                cps.newPath.MouseUp += Img1_MouseLeftButtonUp;
                cps.isLeftCover = true;
                cps.isRightCover = false;
                stackpanel.Children.Insert(0, cps.newPath);

                isHaveLeftEndCap = true;

                if (isHaveLeftEndCap)
                {
                    mi.IsEnabled = false;
                }
            }
            else
            {
                Components cps;
                //保持左右连接  如果存在最后一个段获取它的高度
                if (stackpanel.Children.Count != 0)
                {
                    double height = 0;
                    //height = ((System.Windows.Shapes.Path)stackpanel.Children[stackpanel.Children.Count - 1]).Height;
                    GeometryGroup geometryGroup = (GeometryGroup)((System.Windows.Shapes.Path)stackpanel.Children[stackpanel.Children.Count - 1]).Data;

                    if (geometryGroup.Children.Count == 1)
                    {
                        PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
                        PathFigure curPf = curPg.Figures.ElementAt(0);
                        //height = ((LineSegment)curPf.Segments[1]).Point.Y - ((LineSegment)curPf.Segments[2]).Point.Y;
                        if (curPf.Segments[1] is LineSegment)
                        {
                            height = ((LineSegment)curPf.Segments[1]).Point.Y - ((LineSegment)curPf.Segments[2]).Point.Y;
                        }
                        else if (curPf.Segments[1] is ArcSegment)
                        {
                            height = ((ArcSegment)curPf.Segments[1]).Point.Y - ((LineSegment)curPf.Segments[2]).Point.Y;
                        }
                        else
                        {
                            height = ((PolyLineSegment)curPf.Segments[1]).Points[((PolyLineSegment)curPf.Segments[1]).Points.Count - 1].Y - ((LineSegment)curPf.Segments[2]).Point.Y;
                        }
                    }
                    else
                    {
                        PathGeometry t_curPg = (PathGeometry)geometryGroup.Children[geometryGroup.Children.Count - 2];
                        PathFigure t_curPf = t_curPg.Figures.ElementAt(0);
                        PathGeometry b_curPg = (PathGeometry)geometryGroup.Children[geometryGroup.Children.Count - 1];
                        PathFigure b_curPf = b_curPg.Figures.ElementAt(0);
                        if (b_curPf.Segments[1] is LineSegment)
                        {
                            height = ((LineSegment)b_curPf.Segments[1]).Point.Y - ((LineSegment)t_curPf.Segments[1]).Point.Y;
                        }
                        else if (b_curPf.Segments[1] is ArcSegment)
                        {
                            height = ((ArcSegment)b_curPf.Segments[1]).Point.Y - ((LineSegment)t_curPf.Segments[1]).Point.Y;
                        }
                        else
                        {
                            height = ((PolyLineSegment)b_curPf.Segments[1]).Points[((PolyLineSegment)b_curPf.Segments[1]).Points.Count - 1].Y - ((LineSegment)t_curPf.Segments[1]).Point.Y;
                        }
                    }

                    cps = new Components(new Point(0, 100), new Point(0, 100 + height), new Point(40, 100 + height), new Point(40, 100));
                }
                else
                {
                    cps = new Components(new Point(0, 100), new Point(0, 220), new Point(40, 220), new Point(40, 100));
                }

                SolidColorBrush blackBrush = new SolidColorBrush();
                blackBrush.Color = Colors.Green;
                cps.newPath.Fill = blackBrush;
                components.Add(cps);
                cps.newPath.MouseRightButtonDown += viewbox_MouseRightButtonDown;
                cps.newPath.MouseDown += Img1_MouseLeftButtonDown;
                cps.newPath.MouseUp += Img1_MouseLeftButtonUp;
                cps.isLeftCover = false;
                cps.isRightCover = true;
                stackpanel.Children.Add(cps.newPath);

                isHaveRightEndCap = true;

                if (isHaveRightEndCap)
                {
                    mi.IsEnabled = false;
                }
            }

            Console.WriteLine(startPosition.X);

            //自动调整图形位置
            autoResize();

            allWidth = 0;

            //ColorProc.processWhenMoveLayer(this.canvas, this.stackpanel);
        }

        private void del_Cover(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            if (mi.Header.ToString() == "删除左端盖")
            {
                if (isHaveLeftEndCap)
                {
                    stackpanel.Children.RemoveAt(0);
                    components.RemoveAt(0);

                    isHaveLeftEndCap = false;
                    ContextMenu c = this.canvas.ContextMenu;
                    MenuItem m = c.Items[0] as MenuItem;
                    MenuItem addLeftCoverMenu = m.Items[0] as MenuItem;
                    addLeftCoverMenu.IsEnabled = true;
                }
            }

            if (mi.Header.ToString() == "删除右端盖")
            {
                if (isHaveRightEndCap)
                {
                    stackpanel.Children.RemoveAt(stackpanel.Children.Count - 1);
                    components.RemoveAt(components.Count - 1);

                    isHaveRightEndCap = false;
                    ContextMenu c = this.canvas.ContextMenu;
                    MenuItem m = c.Items[1] as MenuItem;
                    MenuItem addRightCoverMenu = m.Items[0] as MenuItem;
                    addRightCoverMenu.IsEnabled = true;
                }
            }

            autoResize();

            //ColorProc.processWhenMoveLayer(this.canvas, this.stackpanel);
        }

        private void RadioButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.canvas.ContextMenu = aMenu;
        }

        //删除空心管
        void deleteCentralTubeMenu_Click(object sender, RoutedEventArgs e)
        {
            //如果存在空心管，删除
            if (centralCubePanel.Children.Count > 0)
            {
                int index = centralCubePanel.Children.IndexOf(insertShape);
                centralCubePanel.Children.RemoveAt(index);
                centralCubes.RemoveAt(index);

                ColorProc.processWhenDelCylindrical_CC(this.canvas, insertShape);
            }
            autoResize();
        }

        //更新空心管的位置
        private void updateCubePosition()
        {
            if (centralCubes.Count != 0)
            {
                double leftX = Canvas.GetLeft(stackpanel);
                double topY = Canvas.GetTop(stackpanel);
                double allWidth = 0;
                double maxHeight = 0;
                double left_centralCubeMaxOffset = 0;
                for (int i = 0; i < centralCubes.Count; i++)
                {
                    if (centralCubes[i] is Components)
                    {
                        Components cps = (Components)centralCubes[i];
                        cps.newPath.Fill = Brushes.White;
                        allWidth += cps.newPath.Width;
                        if (cps.newPath.Height > maxHeight)
                        {
                            maxHeight = cps.newPath.Height;
                        }
                        double duangai = 0;
                        if (isHaveLeftEndCap)
                        {
                            duangai = ((System.Windows.Shapes.Path)stackpanel.Children[0]).Width;
                        }

                        for (int j = 0; j < centralCubes.Count; j++)
                        {
                            if (cps.cubeOffset > left_centralCubeMaxOffset)
                            {
                                left_centralCubeMaxOffset = ((Components)centralCubes[j]).cubeOffset;
                            }
                        }
                        

                        Canvas.SetLeft(centralCubePanel, leftX + left_centralCubeMaxOffset + duangai);
                        Canvas.SetTop(centralCubePanel, topY + (stackpanel.Height - maxHeight) / 2);
                    }
                }

                centralCubePanel.Width = allWidth;
                centralCubePanel.Height = maxHeight;
            }
        }

        private void addCubeLayer_Click(object sender, RoutedEventArgs e)
        {
            //获取当前图形的路径
            GeometryGroup geometryGroup = (GeometryGroup)((System.Windows.Shapes.Path)centralCubePanel.Children[centralCubeIndex]).Data;

            PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
            PathFigure curPf = curPg.Figures.ElementAt(0);
            Point startP = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + 10);
            curPf.StartPoint = startP;
            ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + 10);
            ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + 10);
            ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + 10);
            ((LineSegment)curPf.Segments[3]).Point = new Point(((LineSegment)curPf.Segments[3]).Point.X, ((LineSegment)curPf.Segments[3]).Point.Y + 10);

            ((Components)centralCubes[centralCubeIndex]).startPoint.Y += 10;
            ((Components)centralCubes[centralCubeIndex]).point2.Y += 10;
            ((Components)centralCubes[centralCubeIndex]).point3.Y += 10;
            ((Components)centralCubes[centralCubeIndex]).point4.Y += 10;

            //创建上下层路径
            PathGeometry topPg = new PathGeometry();
            PathGeometry bottomPg = new PathGeometry();

            PathFigure topPf = new PathFigure();
            PathFigure bottomPf = new PathFigure();

            //绘制上层路径
            topPf.StartPoint = curPf.StartPoint;
            LineSegment ls_top = new LineSegment();
            ls_top.Point = ((LineSegment)curPf.Segments[2]).Point;
            topPf.Segments.Add(ls_top);

            LineSegment ls2_top = new LineSegment();
            Point thirdPoint_top = new Point(ls_top.Point.X, ls_top.Point.Y - 10);
            ls2_top.Point = thirdPoint_top;
            topPf.Segments.Add(ls2_top);

            LineSegment ls3_top = new LineSegment();
            Point forthPoint_top = new Point(curPf.StartPoint.X, curPf.StartPoint.Y - 10);
            ls3_top.Point = forthPoint_top;
            topPf.Segments.Add(ls3_top);
            topPg.Figures.Add(topPf);
            topPf.IsClosed = true;

            ColorProc.processWhenAddLayer_CC(this.canvas, this.centralCubePanel, (System.Windows.Shapes.Path)centralCubePanel.Children[centralCubeIndex], topPf, 0, (Color)ColorConverter.ConvertFromString("#FF787878"));

            //绘制下层路径
            bottomPf.StartPoint = ((LineSegment)curPf.Segments[0]).Point;
            LineSegment ls_bottom = new LineSegment();
            Point secondPoint_bottom = new Point(bottomPf.StartPoint.X, bottomPf.StartPoint.Y + 10);
            ls_bottom.Point = secondPoint_bottom;
            bottomPf.Segments.Add(ls_bottom);

            LineSegment ls2_bottom = new LineSegment();
            Point thirdPoint_bottom = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + 10);
            ls2_bottom.Point = thirdPoint_bottom;
            bottomPf.Segments.Add(ls2_bottom);

            LineSegment ls3_bottom = new LineSegment();
            Point forthPoint_bottom = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y);
            ls3_bottom.Point = forthPoint_bottom;
            bottomPf.Segments.Add(ls3_bottom);

            LineSegment ls4_bottom = new LineSegment();
            ls4_bottom.Point = new Point(bottomPf.StartPoint.X,bottomPf.StartPoint.Y);
            bottomPf.Segments.Add(ls4_bottom);

            bottomPg.Figures.Add(bottomPf);

            ColorProc.processWhenAddLayer_CC(this.canvas, this.centralCubePanel, (System.Windows.Shapes.Path)centralCubePanel.Children[centralCubeIndex], bottomPf, 1, (Color)ColorConverter.ConvertFromString("#FF787878"));


            //将上下层路径添加到组中
            geometryGroup.Children.Add(topPg);
            geometryGroup.Children.Add(bottomPg);

            ((System.Windows.Shapes.Path)centralCubePanel.Children[centralCubeIndex]).Height += 20;


            ((Components)centralCubes[centralCubeIndex]).layerNum += 2;
            ((Components)centralCubes[centralCubeIndex]).layerNums.Add(((Components)centralCubes[centralCubeIndex]).layerNum - 1);
            ((Components)centralCubes[centralCubeIndex]).layerNums.Add(((Components)centralCubes[centralCubeIndex]).layerNum);
            ((Components)centralCubes[centralCubeIndex]).layerType.Add("球");
            ((Components)centralCubes[centralCubeIndex]).layerType.Add("球");
            ((Components)centralCubes[centralCubeIndex]).layerMaterial.Add("钢");
            ((Components)centralCubes[centralCubeIndex]).layerMaterial.Add("钢");
            ((Components)centralCubes[centralCubeIndex]).layerSize.Add(((Components)centralCubes[centralCubeIndex]).layerNum - 1, new Hashtable());
            ((Components)centralCubes[centralCubeIndex]).layerSize.Add(((Components)centralCubes[centralCubeIndex]).layerNum, new Hashtable());
            ((Hashtable)(((Components)centralCubes[centralCubeIndex]).layerSize[((Components)centralCubes[centralCubeIndex]).layerNum - 1])).Add("diameter", 0);
            ((Hashtable)(((Components)centralCubes[centralCubeIndex]).layerSize[((Components)centralCubes[centralCubeIndex]).layerNum - 1])).Add("longLength", 0);
            ((Hashtable)(((Components)centralCubes[centralCubeIndex]).layerSize[((Components)centralCubes[centralCubeIndex]).layerNum - 1])).Add("width", 0);
            ((Hashtable)(((Components)centralCubes[centralCubeIndex]).layerSize[((Components)centralCubes[centralCubeIndex]).layerNum - 1])).Add("height", 0);
            ((Hashtable)(((Components)centralCubes[centralCubeIndex]).layerSize[((Components)centralCubes[centralCubeIndex]).layerNum - 1])).Add("ObliqueAngle", 0);
            ((Hashtable)(((Components)centralCubes[centralCubeIndex]).layerSize[((Components)centralCubes[centralCubeIndex]).layerNum])).Add("diameter", 0);
            ((Hashtable)(((Components)centralCubes[centralCubeIndex]).layerSize[((Components)centralCubes[centralCubeIndex]).layerNum])).Add("longLength", 0);
            ((Hashtable)(((Components)centralCubes[centralCubeIndex]).layerSize[((Components)centralCubes[centralCubeIndex]).layerNum])).Add("width", 0);
            ((Hashtable)(((Components)centralCubes[centralCubeIndex]).layerSize[((Components)centralCubes[centralCubeIndex]).layerNum])).Add("height", 0);
            ((Hashtable)(((Components)centralCubes[centralCubeIndex]).layerSize[((Components)centralCubes[centralCubeIndex]).layerNum])).Add("ObliqueAngle", 0);
            ((Components)centralCubes[centralCubeIndex]).layerLeftThickness.Add(10);
            ((Components)centralCubes[centralCubeIndex]).layerLeftThickness.Add(10);
            ((Components)centralCubes[centralCubeIndex]).layerRightThickness.Add(10);
            ((Components)centralCubes[centralCubeIndex]).layerRightThickness.Add(10);
        }

        void copyCentralTubeMenu_Click(object sender, RoutedEventArgs e)
        {
            int index = centralCubePanel.Children.IndexOf(insertShape);
            Components curComp = (Components)centralCubes[index];

            Components comp = new Components(curComp.startPoint, curComp.point2, curComp.point3, curComp.point4);
            comp.newPath.Height += (curComp.startPoint.Y > curComp.point4.Y ? curComp.point4.Y : curComp.startPoint.Y) - 100;
            comp.newPath.MouseRightButtonDown += cube_MouseRightButtonDown;
            comp.newPath.MouseDown += CentralCube_MouseLeftButtonDown;
            comp.newPath.MouseUp += Img1_MouseLeftButtonUp;
            centralCubes.Insert(index + 1, comp);
            comp.newPath.Fill = curComp.newPath.Fill;
            centralCubePanel.Children.Insert(index + 1, comp.newPath);

            //切换insertshape
            insertShape = comp.newPath;
            curLayerNum = 0;

            PathFigure compPf = ((PathGeometry)((GeometryGroup)curComp.newPath.Data).Children[0]).Figures[0];

            if (compPf.Segments[1] is ArcSegment)
            {
                if (((ArcSegment)compPf.Segments[1]).SweepDirection == SweepDirection.Counterclockwise)
                    changeLineSegmentToArcSegment(curComp.radius, 0);
                else
                    changeLineSegmentToArcSegment(curComp.radius, 1);
            }
            else if (compPf.Segments[1] is PolyLineSegment)
            {
                if (curComp.isChangeOgive)
                {
                    changeLineSegmentToArcSegment(curComp.radius, 0);
                }
                else
                {
                    changeLineSegmentToArcSegment(curComp.radius, 1);
                }
            }

            comp.radius = curComp.radius;
            insertShape = curComp.newPath;

            //判断是否存在层
            GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
            if (geometryGroup.Children.Count != 1)
            {
                GeometryGroup geometryGroup_ = (GeometryGroup)(comp).newPath.Data;
                if (comp.layerNum <= 2)
                {
                    double leftWidth = Double.Parse(curComp.layerLeftThickness[curComp.layerNum - 2].ToString());
                    double rightWidth = Double.Parse(curComp.layerRightThickness[curComp.layerNum - 2].ToString());
                    double maxCh_Width = leftWidth > rightWidth ? leftWidth : rightWidth;
                    PathGeometry curPg = (PathGeometry)geometryGroup_.Children[0];
                    PathFigure curPf = curPg.Figures.ElementAt(0);
                    Point startP = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + maxCh_Width);
                    curPf.StartPoint = startP;
                    ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + maxCh_Width);
                    if (curPf.Segments[1] is LineSegment)
                    {
                        ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + maxCh_Width);
                    }
                    else if (curPf.Segments[1] is ArcSegment)
                    {
                        ((ArcSegment)curPf.Segments[1]).Point = new Point(((ArcSegment)curPf.Segments[1]).Point.X, ((ArcSegment)curPf.Segments[1]).Point.Y + maxCh_Width);
                    }
                    else
                    {
                        for (int i = 0; i < ((PolyLineSegment)curPf.Segments[1]).Points.Count; i++)
                        {
                            ((PolyLineSegment)curPf.Segments[1]).Points[i] = new Point(((PolyLineSegment)curPf.Segments[1]).Points[i].X, ((PolyLineSegment)curPf.Segments[1]).Points[i].Y + maxCh_Width);
                        }
                    }
                    ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + maxCh_Width);
                    if (curPf.Segments[3] is LineSegment)
                    {
                        ((LineSegment)curPf.Segments[3]).Point = new Point(((LineSegment)curPf.Segments[3]).Point.X, ((LineSegment)curPf.Segments[3]).Point.Y + maxCh_Width);
                    }
                    else if (curPf.Segments[3] is ArcSegment)
                    {
                        ((ArcSegment)curPf.Segments[3]).Point = new Point(((ArcSegment)curPf.Segments[3]).Point.X, ((ArcSegment)curPf.Segments[3]).Point.Y + maxCh_Width);
                    }
                    else
                    {
                        for (int i = 0; i < ((PolyLineSegment)curPf.Segments[3]).Points.Count; i++)
                        {
                            ((PolyLineSegment)curPf.Segments[3]).Points[i] = new Point(((PolyLineSegment)curPf.Segments[3]).Points[i].X, ((PolyLineSegment)curPf.Segments[3]).Points[i].Y + maxCh_Width);
                        }
                    }

                    //创建上下层路径
                    PathGeometry topPg = new PathGeometry();
                    PathGeometry bottomPg = new PathGeometry();

                    PathFigure topPf = new PathFigure();
                    PathFigure bottomPf = new PathFigure();

                    //绘制上层路径
                    topPf.StartPoint = curPf.StartPoint;
                    if (curPf.Segments[3] is LineSegment)
                    {
                        LineSegment ls_top = new LineSegment();
                        ls_top.Point = ((LineSegment)curPf.Segments[2]).Point;
                        topPf.Segments.Add(ls_top);
                    }
                    else if (curPf.Segments[3] is ArcSegment)
                    {
                        ArcSegment ls_top = new ArcSegment();
                        ls_top.Point = ((LineSegment)curPf.Segments[2]).Point;
                        ls_top.Size = ((ArcSegment)curPf.Segments[3]).Size;
                        if (((ArcSegment)curPf.Segments[3]).SweepDirection == SweepDirection.Clockwise)
                        {
                            ls_top.SweepDirection = SweepDirection.Counterclockwise;
                        }
                        else
                        {
                            ls_top.SweepDirection = SweepDirection.Clockwise;
                        }
                        topPf.Segments.Add(ls_top);
                    }
                    else
                    {
                        PolyLineSegment pls_top = new PolyLineSegment();
                        for (int i = ((PolyLineSegment)curPf.Segments[3]).Points.Count - 1; i >= 0; i--)
                        {
                            pls_top.Points.Add(((PolyLineSegment)curPf.Segments[3]).Points[i]);
                        }
                        topPf.Segments.Add(pls_top);
                    }

                    LineSegment ls2_top = new LineSegment();
                    Point thirdPoint_top = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y - rightWidth);
                    ls2_top.Point = thirdPoint_top;
                    topPf.Segments.Add(ls2_top);

                    if (curPf.Segments[3] is LineSegment)
                    {
                        LineSegment ls3_top = new LineSegment();
                        Point forthPoint_top = new Point(curPf.StartPoint.X, curPf.StartPoint.Y - leftWidth);
                        ls3_top.Point = forthPoint_top;
                        topPf.Segments.Add(ls3_top);
                    }
                    else if (curPf.Segments[3] is ArcSegment)
                    {
                        ArcSegment ls3_top = new ArcSegment();
                        Point forthPoint_top = new Point(curPf.StartPoint.X, curPf.StartPoint.Y - leftWidth);
                        ls3_top.Point = forthPoint_top;
                        ls3_top.Size = ((ArcSegment)curPf.Segments[3]).Size;
                        //ls3_top.SweepDirection = SweepDirection.Counterclockwise;
                        ls3_top.SweepDirection = ((ArcSegment)curPf.Segments[3]).SweepDirection;
                        topPf.Segments.Add(ls3_top);
                    }
                    else
                    {
                        PolyLineSegment pls2_top = new PolyLineSegment();
                        //pls2_top.Points = ((PolyLineSegment)curPf.Segments[3]).Points;
                        for (int i = 0; i < ((PolyLineSegment)curPf.Segments[3]).Points.Count; i++)
                        {
                            pls2_top.Points.Add(new Point(((PolyLineSegment)curPf.Segments[3]).Points[i].X, ((PolyLineSegment)curPf.Segments[3]).Points[i].Y - leftWidth));
                        }
                        topPf.Segments.Add(pls2_top);
                    }
                    topPg.Figures.Add(topPf);
                    topPf.IsClosed = true;

                    ColorProc.processWhenAddLayer_CC(this.canvas, this.centralCubePanel, (System.Windows.Shapes.Path)centralCubePanel.Children[centralCubeIndex + 1], topPf, 0, (Color)ColorConverter.ConvertFromString("#FF787878"));

                    //绘制下层路径
                    bottomPf.StartPoint = ((LineSegment)curPf.Segments[0]).Point;
                    LineSegment ls_bottom = new LineSegment();
                    Point secondPoint_bottom = new Point(bottomPf.StartPoint.X, bottomPf.StartPoint.Y + leftWidth);
                    ls_bottom.Point = secondPoint_bottom;
                    bottomPf.Segments.Add(ls_bottom);

                    if (curPf.Segments[1] is LineSegment)
                    {
                        LineSegment ls2_bottom = new LineSegment();
                        Point thirdPoint_bottom = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + rightWidth);
                        ls2_bottom.Point = thirdPoint_bottom;
                        bottomPf.Segments.Add(ls2_bottom);
                    }
                    else if (curPf.Segments[1] is ArcSegment)
                    {
                        ArcSegment ls2_bottom = new ArcSegment();
                        Point thirdPoint_bottom = new Point(((ArcSegment)curPf.Segments[1]).Point.X, ((ArcSegment)curPf.Segments[1]).Point.Y + rightWidth);
                        ls2_bottom.Point = thirdPoint_bottom;
                        ls2_bottom.Size = ((ArcSegment)curPf.Segments[1]).Size;
                        //ls2_bottom.SweepDirection = SweepDirection.Counterclockwise;
                        ls2_bottom.SweepDirection = ((ArcSegment)curPf.Segments[1]).SweepDirection;
                        bottomPf.Segments.Add(ls2_bottom);
                    }
                    else
                    {
                        PolyLineSegment pls_bottom = new PolyLineSegment();
                        for (int i = 0; i < ((PolyLineSegment)curPf.Segments[1]).Points.Count; i++)
                        {
                            pls_bottom.Points.Add(new Point(((PolyLineSegment)curPf.Segments[1]).Points[i].X, ((PolyLineSegment)curPf.Segments[1]).Points[i].Y + rightWidth));
                        }
                        bottomPf.Segments.Add(pls_bottom);
                    }

                    LineSegment ls3_bottom = new LineSegment();
                    if (curPf.Segments[1] is LineSegment)
                    {
                        Point forthPoint_bottom = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y);
                        ls3_bottom.Point = forthPoint_bottom;
                    }
                    else if (curPf.Segments[1] is ArcSegment)
                    {
                        Point forthPoint_bottom = new Point(((ArcSegment)curPf.Segments[1]).Point.X, ((ArcSegment)curPf.Segments[1]).Point.Y);
                        ls3_bottom.Point = forthPoint_bottom;
                    }
                    else
                    {
                        Point forthPoint_bottom = new Point(((PolyLineSegment)curPf.Segments[1]).Points[((PolyLineSegment)curPf.Segments[1]).Points.Count - 1].X, ((PolyLineSegment)curPf.Segments[1]).Points[((PolyLineSegment)curPf.Segments[1]).Points.Count - 1].Y);
                        ls3_bottom.Point = forthPoint_bottom;
                    }
                    bottomPf.Segments.Add(ls3_bottom);

                    if (curPf.Segments[1] is LineSegment)
                    {
                        //LineSegment ls4_bottom = new LineSegment();
                        //ls4_bottom.Point = bottomPf.StartPoint;
                        //bottomPf.Segments.Add(ls4_bottom);
                    }
                    else if (curPf.Segments[1] is ArcSegment)
                    {
                        ArcSegment ls4_bottom = new ArcSegment();
                        Point forthPoint_bottom = bottomPf.StartPoint;
                        ls4_bottom.Point = forthPoint_bottom;
                        ls4_bottom.Size = ((ArcSegment)curPf.Segments[1]).Size;
                        //ls4_bottom.SweepDirection = SweepDirection.Clockwise;
                        if (((ArcSegment)curPf.Segments[1]).SweepDirection == SweepDirection.Clockwise)
                        {
                            ls4_bottom.SweepDirection = SweepDirection.Counterclockwise;
                        }
                        else
                        {
                            ls4_bottom.SweepDirection = SweepDirection.Clockwise;
                        }
                        bottomPf.Segments.Add(ls4_bottom);
                    }
                    else
                    {
                        PolyLineSegment pls2_bottom = new PolyLineSegment();

                        for (int i = ((PolyLineSegment)curPf.Segments[1]).Points.Count - 1; i >= 0; i--)
                        {
                            pls2_bottom.Points.Add(((PolyLineSegment)curPf.Segments[1]).Points[i]);
                        }
                        bottomPf.Segments.Add(pls2_bottom);
                    }
                    bottomPg.Figures.Add(bottomPf);

                    ColorProc.processWhenAddLayer_CC(this.canvas, this.centralCubePanel, (System.Windows.Shapes.Path)centralCubePanel.Children[centralCubeIndex + 1], bottomPf, 1, (Color)ColorConverter.ConvertFromString("#FF787878"));

                    //将上下层路径添加到组中
                    geometryGroup_.Children.Add(topPg);
                    geometryGroup_.Children.Add(bottomPg);

                    comp.newPath.Height += 2 * (leftWidth > rightWidth ? leftWidth : rightWidth);
                    comp.height = comp.newPath.Height;


                    //自动调整图形位置
                    autoResize();
                }
            }

            ((Components)centralCubes[index + 1]).layerNum += 2;
            ((Components)centralCubes[index + 1]).layerNums.Add(((Components)centralCubes[index + 1]).layerNum - 1);
            ((Components)centralCubes[index + 1]).layerNums.Add(((Components)centralCubes[index + 1]).layerNum);
            ((Components)centralCubes[index + 1]).layerType.Add(curComp.layerType[0]);
            ((Components)centralCubes[index + 1]).layerType.Add(curComp.layerType[1]);
            ((Components)centralCubes[index + 1]).layerMaterial.Add(curComp.layerType[0]);
            ((Components)centralCubes[index + 1]).layerMaterial.Add(curComp.layerType[1]);
            ((Components)centralCubes[index + 1]).layerSize.Add(((Components)centralCubes[index]).layerNum - 1, new Hashtable());
            ((Components)centralCubes[index + 1]).layerSize.Add(((Components)centralCubes[index]).layerNum, new Hashtable());
            ((Hashtable)(((Components)centralCubes[index + 1]).layerSize[((Components)centralCubes[index + 1]).layerNum - 1])).Add("diameter", ((Hashtable)(((Components)centralCubes[index]).layerSize[((Components)centralCubes[index]).layerNum - 1]))["diameter"]);
            ((Hashtable)(((Components)centralCubes[index + 1]).layerSize[((Components)centralCubes[index + 1]).layerNum - 1])).Add("longLength", ((Hashtable)(((Components)centralCubes[index]).layerSize[((Components)centralCubes[index]).layerNum - 1]))["longLength"]);
            ((Hashtable)(((Components)centralCubes[index + 1]).layerSize[((Components)centralCubes[index + 1]).layerNum - 1])).Add("width", ((Hashtable)(((Components)centralCubes[index]).layerSize[((Components)centralCubes[index]).layerNum - 1]))["width"]);
            ((Hashtable)(((Components)centralCubes[index + 1]).layerSize[((Components)centralCubes[index + 1]).layerNum - 1])).Add("height", ((Hashtable)(((Components)centralCubes[index]).layerSize[((Components)centralCubes[index]).layerNum - 1]))["height"]);
            ((Hashtable)(((Components)centralCubes[index + 1]).layerSize[((Components)centralCubes[index + 1]).layerNum - 1])).Add("ObliqueAngle", ((Hashtable)(((Components)centralCubes[index]).layerSize[((Components)centralCubes[index]).layerNum - 1]))["ObliqueAngle"]);
            ((Hashtable)(((Components)centralCubes[index + 1]).layerSize[((Components)centralCubes[index + 1]).layerNum])).Add("diameter", ((Hashtable)(((Components)centralCubes[index]).layerSize[((Components)centralCubes[index]).layerNum]))["diameter"]);
            ((Hashtable)(((Components)centralCubes[index + 1]).layerSize[((Components)centralCubes[index + 1]).layerNum])).Add("longLength", ((Hashtable)(((Components)centralCubes[index]).layerSize[((Components)centralCubes[index]).layerNum]))["longLength"]);
            ((Hashtable)(((Components)centralCubes[index + 1]).layerSize[((Components)centralCubes[index + 1]).layerNum])).Add("width", ((Hashtable)(((Components)centralCubes[index]).layerSize[((Components)centralCubes[index]).layerNum]))["width"]);
            ((Hashtable)(((Components)centralCubes[index + 1]).layerSize[((Components)centralCubes[index + 1]).layerNum])).Add("height", ((Hashtable)(((Components)centralCubes[index]).layerSize[((Components)centralCubes[index]).layerNum]))["height"]);
            ((Hashtable)(((Components)centralCubes[index + 1]).layerSize[((Components)centralCubes[index + 1]).layerNum])).Add("ObliqueAngle", ((Hashtable)(((Components)centralCubes[index]).layerSize[((Components)centralCubes[index]).layerNum]))["ObliqueAngle"]);
            ((Components)centralCubes[index + 1]).layerLeftThickness.Add(((Components)centralCubes[index]).layerLeftThickness[0]);
            ((Components)centralCubes[index + 1]).layerLeftThickness.Add(((Components)centralCubes[index]).layerLeftThickness[1]);
            ((Components)centralCubes[index + 1]).layerRightThickness.Add(((Components)centralCubes[index]).layerRightThickness[0]);
            ((Components)centralCubes[index + 1]).layerRightThickness.Add(((Components)centralCubes[index]).layerRightThickness[1]);

            //自动调整图形位置
            autoResize();

            allWidth = 0;
        }

        //增加空心管
        void addCentralTubeMenu_Click(object sender, RoutedEventArgs e)
        {
            double leftX = Canvas.GetLeft(stackpanel);
            double topY = Canvas.GetTop(stackpanel);

            Components cps = new Components(new Point(0, 100), new Point(0, 150), new Point(100, 150), new Point(100, 100));
            cps.layerNum = 0;
            centralCubes.Add(cps);
            cps.newPath.Fill = Brushes.White;
            cps.newPath.MouseDown += CentralCube_MouseLeftButtonDown;
            cps.newPath.MouseRightButtonDown += cube_MouseRightButtonDown;
            cps.newPath.MouseUp += Img1_MouseLeftButtonUp;
            centralCubePanel.Children.Add(cps.newPath);
            centralCubeIndex = centralCubePanel.Children.IndexOf(cps.newPath);
            
            
            double offset = 0;
            if (isHaveLeftEndCap)
            {
                System.Windows.Shapes.Path leftEndCap = (System.Windows.Shapes.Path)stackpanel.Children[0];
                offset = leftEndCap.Width;
            }

            //判断当前的中心管之前是否有中心管有offset
            double left_centralCubeMaxOffset = 0;
            if (centralCubes.Count != 1)
            {
                for (int i = 0; i < centralCubes.Count; i++)
                {
                    if (((Components)centralCubes[i]).cubeOffset > left_centralCubeMaxOffset)
                    {
                        left_centralCubeMaxOffset = ((Components)centralCubes[i]).cubeOffset;
                    }
                }
            }

            Canvas.SetLeft(centralCubePanel, leftX + offset + left_centralCubeMaxOffset);
            Canvas.SetTop(centralCubePanel, topY + (stackpanel.Height - centralCubePanel.Height) / 2);
            
            //添加层
            addCubeLayer_Click(sender, e);

            isHaveCentralTube = true;

            autoResize();
        }

        void cube_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu aMenu = new ContextMenu();
            MenuItem editCubeMenu = new MenuItem();
            editCubeMenu.Header = "编辑中心管厚度";
            editCubeMenu.Click += editCubeMenu_Click;
            MenuItem deleteCentralTubeMenu = new MenuItem();
            deleteCentralTubeMenu.Header = "删除中心管";
            deleteCentralTubeMenu.Click += deleteCentralTubeMenu_Click;
            MenuItem copyCentralTubeMenu = new MenuItem();
            copyCentralTubeMenu.Header = "复制中心管";
            copyCentralTubeMenu.Click += copyCentralTubeMenu_Click;
            aMenu.Items.Add(editCubeMenu);
            aMenu.Items.Add(copyCentralTubeMenu);
            aMenu.Items.Add(deleteCentralTubeMenu);

            insertShape = sender as System.Windows.Shapes.Path;
            int index = centralCubePanel.Children.IndexOf(insertShape);
            centralCubeIndex = index;

            ((System.Windows.Shapes.Path)centralCubePanel.Children[index]).ContextMenu = aMenu;
        }


        void editCubeMenu_Click(object sender, RoutedEventArgs e)
        {
            changeTitle = 2;
            btEdit_CentralCube_Click(sender, e);
        }

        //编辑起爆点
        void editExplosionMenu_Click(object sender, RoutedEventArgs e)
        {
            explosionEdit ew = new explosionEdit();
            ew.Owner = this;
            ew.explosions = explosions;
            ew.explosionData = explosionData;
            ew.showComponentInfo();
            ew.ChangePositionEvent += autoResize;
            ew.Show();
        }

        //删除起爆点
        void deleteExplosionMenu_Click(object sender, RoutedEventArgs e)
        {
            //如果存在起爆点，删除
            if (explosions.Count != 0 && explosions != null)
            {
                if (currentShape is Ellipse)
                {
                    canvas.Children.Remove(currentShape);
                    int index = explosions.IndexOf(currentShape);
                    explosions.Remove(currentShape);
                    explosionData.Remove(index + 1);
                }
            }
        }

        //添加起爆点
        private void addExplosionMenu_Click(object sender, RoutedEventArgs e)
        {
            double leftX = Canvas.GetLeft(stackpanel);
            double topY = Canvas.GetTop(stackpanel);

            Ellipse el = new Ellipse();
            el.Width = 20;
            el.Height = 20;
            el.Fill = Brushes.Red;
            el.Stroke = Brushes.Black;
            el.MouseDown += explosion_MouseRightButtonDown;
            el.MouseDown += Img1_MouseLeftButtonDown;
            el.MouseUp += Img1_MouseLeftButtonUp;
            canvas.Children.Add(el);

            explosions.Add(el);
            explosionData.Add(explosions.Count, new Hashtable());
            ((Hashtable)(explosionData[explosions.Count])).Add("type", "点");
            ((Hashtable)(explosionData[explosions.Count])).Add("startPosition", "0");
            ((Hashtable)(explosionData[explosions.Count])).Add("radius", "0");
            ((Hashtable)(explosionData[explosions.Count])).Add("pointNums", "0");

            double offset = 0;
            if (isHaveLeftEndCap)
            {
                System.Windows.Shapes.Path leftEndCap = (System.Windows.Shapes.Path)stackpanel.Children[0];
                offset = leftEndCap.Width;
            }

            Canvas.SetLeft(el, leftX + offset - 10);
            Canvas.SetTop(el, topY + (stackpanel.Height - el.Height) / 2);
        }

        //更新起爆点的位置
        private void updateExplosionPosition()
        {
            if (explosions.Count != 0)
            {
                double leftX = Canvas.GetLeft(stackpanel);
                double topY = Canvas.GetTop(stackpanel);

                for (int i = 0; i < explosions.Count; i++)
                {
                    if (explosions[i] is Ellipse)
                    {
                        Ellipse el = (Ellipse)explosions[i];
                        double xLeft = Double.Parse(((Hashtable)(explosionData[explosions.Count - i]))["startPosition"].ToString());
                        double yTop = Double.Parse(((Hashtable)(explosionData[explosions.Count - i]))["radius"].ToString());
                        double duangai = 0;
                        if (isHaveLeftEndCap)
                        {
                            duangai = ((System.Windows.Shapes.Path)stackpanel.Children[0]).Width;
                        }
                        Canvas.SetLeft(el, leftX - el.Width / 2 + xLeft + duangai);
                        Canvas.SetTop(el, topY + (stackpanel.Height - el.Height) / 2 + yTop);
                    }
                }
            }
        }

        //实现复制到剪切板
        private void addCopy_Click(object sender, RoutedEventArgs e)
        {
            double width = canvas.Width;
            double height = canvas.Height;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);
            Clipboard.SetImage(bmpCopied);
        }

        //添加段点击事件
        private void addCylindrical_Click(object sender, RoutedEventArgs e)
        {
            Components cps;
            if (isHaveRightEndCap == false)
            {
                //判断前面是否连接
                if (stackpanel.Children.Count != 0)
                {
                    //获取前面path的右高度
                    System.Windows.Shapes.Path leftPath = (System.Windows.Shapes.Path)stackpanel.Children[stackpanel.Children.Count - 1];

                    GeometryGroup geometryGroup = (GeometryGroup)leftPath.Data;
                    PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
                    PathFigure curPf = curPg.Figures.ElementAt(0);

                    double height = 200;
                    if (curPf.Segments[1] is LineSegment)
                    {
                        //获取left的右侧高度
                        double topR = ((LineSegment)curPf.Segments[1]).Point.Y;
                        double bottomR = ((LineSegment)curPf.Segments[2]).Point.Y;
                        height = Math.Abs(topR - bottomR);
                    } else if (curPf.Segments[1] is ArcSegment) {
                        double topR = ((ArcSegment)curPf.Segments[1]).Point.Y;
                        double bottomR = ((LineSegment)curPf.Segments[2]).Point.Y;
                        height = Math.Abs(topR - bottomR);
                    }
                    else {
                        double topR = ((PolyLineSegment)curPf.Segments[1]).Points[((PolyLineSegment)curPf.Segments[1]).Points.Count - 1].Y;
                        double bottomR = ((LineSegment)curPf.Segments[2]).Point.Y;
                        height = Math.Abs(topR - bottomR);
                    }

                    cps = new Components(new Point(0, 100), new Point(0, 100 + height), new Point(200, 100 + height), new Point(200, 100));
                }
                else
                {
                    cps = new Components(new Point(0, 100), new Point(0, 300), new Point(200, 300), new Point(200, 100));
                }
                cps.layerNum = 0;
                components.Add(cps);
                cps.newPath.MouseRightButtonDown += viewbox_MouseRightButtonDown;
                cps.newPath.MouseDown += Img1_MouseLeftButtonDown;
                cps.newPath.MouseUp += Img1_MouseLeftButtonUp;
                stackpanel.Children.Add(cps.newPath);

                //自动调整图形位置
                autoResize();

                allWidth = 0;

                if (isHaveRightEndCap)
                {
                    isHaveRightEndCap = false;
                }

                //ColorProc.processWhenMoveLayer(this.canvas, this.stackpanel);
            }
            else
            {
                //判断前面是否连接
                if (stackpanel.Children.Count != 0)
                {
                    //获取前面path的右高度
                    System.Windows.Shapes.Path leftPath = (System.Windows.Shapes.Path)stackpanel.Children[stackpanel.Children.Count - 2];

                    GeometryGroup geometryGroup = (GeometryGroup)leftPath.Data;
                    PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
                    PathFigure curPf = curPg.Figures.ElementAt(0);

                    double height = 200;
                    if (curPf.Segments[1] is LineSegment)
                    {
                        //获取left的右侧高度
                        double topR = ((LineSegment)curPf.Segments[1]).Point.Y;
                        double bottomR = ((LineSegment)curPf.Segments[2]).Point.Y;
                        height = Math.Abs(topR - bottomR);
                    }
                    else if (curPf.Segments[1] is ArcSegment)
                    {
                        double topR = ((ArcSegment)curPf.Segments[1]).Point.Y;
                        double bottomR = ((LineSegment)curPf.Segments[2]).Point.Y;
                        height = Math.Abs(topR - bottomR);
                    }
                    else
                    {
                        double topR = ((PolyLineSegment)curPf.Segments[1]).Points[((PolyLineSegment)curPf.Segments[1]).Points.Count - 1].Y;
                        double bottomR = ((LineSegment)curPf.Segments[2]).Point.Y;
                        height = Math.Abs(topR - bottomR);
                    }

                    cps = new Components(new Point(0, 100), new Point(0, 100 + height), new Point(200, 100 + height), new Point(200, 100));
                }
                else
                {
                    cps = new Components(new Point(0, 100), new Point(0, 300), new Point(200, 300), new Point(200, 100));
                }
                cps.layerNum = 0;
                components.Insert(components.Count - 1,cps);
                cps.newPath.MouseRightButtonDown += viewbox_MouseRightButtonDown;
                cps.newPath.MouseDown += Img1_MouseLeftButtonDown;
                cps.newPath.MouseUp += Img1_MouseLeftButtonUp;
                stackpanel.Children.Insert(stackpanel.Children.Count - 1, cps.newPath);

                //自动调整图形位置
                makeRightCoverConnectSkillfully();
                autoResize();
                //MessageBox.Show("已有右端盖，不能添加段！", "警告");
            }
        }

        private void drawCylindrical(float width, float height)
        {
            Polygon Cylindrical = new Polygon();
            Cylindrical.StrokeThickness = 2;
            double x, y;
            if (stackpanel.Children.Count == 0)
            {
                x = (canvas.Width - width) / 2;
                y = (canvas.Height - height) / 2;
            }
            else
            {
                x = (canvas.Width - width - stackpanel.Width) / 2;
                y = (canvas.Width - ((height > stackpanel.Height) ? height : stackpanel.Height)) / 2;
            }
            Point Point1 = new Point(x, y);
            Point Point2 = new Point(x, y + height);
            Point Point3 = new Point(x + width, y + height + 10);
            Point Point4 = new Point(x + width, y - 10);
            PointCollection pointCollection = new PointCollection();
            pointCollection.Add(Point1);
            pointCollection.Add(Point2);
            pointCollection.Add(Point3);
            pointCollection.Add(Point4);
            Cylindrical.Points = pointCollection;
            Cylindrical.Fill = Brushes.DarkOrange;
            canvas.Children.Add(Cylindrical);

            Console.WriteLine(startPosition.X);
            Cylindrical.MouseRightButtonDown += viewbox_MouseRightButtonDown;
            Cylindrical.MouseDown += Img1_MouseLeftButtonDown;
            Cylindrical.MouseUp += Img1_MouseLeftButtonUp;
            //自动调整图形位置
            autoResize();

            allWidth = 0;
        }

        //删除起爆点
        private void explosion_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu aMenu = new ContextMenu();
            MenuItem explosionMenu = new MenuItem();
            explosionMenu.Header = "删除起爆点";
            explosionMenu.Click += deleteExplosionMenu_Click;
            aMenu.Items.Add(explosionMenu);

            if (sender is Ellipse)
            {
                currentShape = sender as Shape;
                currentShape.ContextMenu = aMenu;
            }
        }

        private void viewbox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Shapes.Path)
            {
                insertShape = sender as System.Windows.Shapes.Path;
                insertShape.Stroke = Brushes.Red;
                insertShape.StrokeThickness = 2;
                int index = stackpanel.Children.IndexOf(insertShape);
                if (isHaveLeftEndCap)
                {
                    if (index == 0) {
                        e.Handled = true;
                        return;
                    }
                }
                if (isHaveRightEndCap) {
                    if (index == stackpanel.Children.Count - 1)
                    {
                        e.Handled = true;
                        return;
                    }
                }

                Console.WriteLine(index);
                currentComp = (Components)components[index];
                curLayerNum = currentComp.layerNum;

                for (int i = 0; i < stackpanel.Children.Count; i++)
                {
                    if (i != index)
                    {
                        System.Windows.Shapes.Path path = stackpanel.Children[i] as System.Windows.Shapes.Path;
                        path.Stroke = Brushes.Blue;
                    }
                }

                insertShape.CaptureMouse();
            }
            else
            {
                currentShape = sender as Shape;
                currentShape.CaptureMouse();
            }

            changeTitle = 0;

            ContextMenu aMenu = new ContextMenu();
            MenuItem compMenu = new MenuItem();
            compMenu.Header = "段";
            aMenu.Items.Add(compMenu);
            MenuItem addMenu = new MenuItem();
            addMenu.Header = "增加段";
            addMenu.Click += addCylindrical_Click;
            MenuItem deleteMenu = new MenuItem();
            deleteMenu.Header = "删除段";
            deleteMenu.Click += delMenu_Click;
            //aMenu.Items.Add(deleteMenu);
            MenuItem copyCompMenu = new MenuItem();
            copyCompMenu.Header = "复制段";
            copyCompMenu.Click += copyCompMenu_Click;
            MenuItem editCompMenu = new MenuItem();
            editCompMenu.Header = "编辑段";
            editCompMenu.Click += btEdit_Click;
            compMenu.Items.Add(addMenu);
            compMenu.Items.Add(deleteMenu);
            compMenu.Items.Add(copyCompMenu);
            compMenu.Items.Add(editCompMenu);
            MenuItem layerMenu = new MenuItem();
            layerMenu.Header = "层";
            aMenu.Items.Add(layerMenu);
            MenuItem addLayerMenu = new MenuItem();
            addLayerMenu.Header = "添加层";
            addLayerMenu.Click += addLayerMenu_Click;
            //aMenu.Items.Add(addLayerMenu);
            MenuItem deleteLayer = new MenuItem();
            deleteLayer.Header = "删除层";
            deleteLayer.Click += deleteLayer_Click;
            //aMenu.Items.Add(deleteLayer);
            //MenuItem addCyliMenu = new MenuItem();
            //addCyliMenu.Header = "添加球";
            //addCyliMenu.Click += addCyliMenu_Click;
            //aMenu.Items.Add(addCyliMenu);
            MenuItem editLayerMenu = new MenuItem();
            editLayerMenu.Header = "编辑层";
            editLayerMenu.Click += editLayerMenu_Click;
            if (insertShape != null && ((Components)(components[stackpanel.Children.IndexOf(insertShape)])).layerNum == 0)
            {
                editLayerMenu.IsEnabled = false;
            }
            else
            {
                editLayerMenu.IsEnabled = true;
            }
            //aMenu.Items.Add(editLayerMenu);
            layerMenu.Items.Add(addLayerMenu);
            layerMenu.Items.Add(deleteLayer);
            layerMenu.Items.Add(editLayerMenu);

            if (sender is System.Windows.Shapes.Path)
            {
                insertShape = sender as System.Windows.Shapes.Path;
                int index = stackpanel.Children.IndexOf(insertShape);
                currentComp = (Components)components[index];
                curLayerNum = currentComp.layerNum;
                Console.WriteLine(index);
                insertShape.ContextMenu = aMenu;
            }
            else
            {
                currentShape = sender as Shape;
                currentShape.ContextMenu = aMenu;
            }

            e.Handled = true;
        }

        void copyCompMenu_Click(object sender, RoutedEventArgs e)
        {
            int index = stackpanel.Children.IndexOf(insertShape);
            Components curComp = (Components)components[index];

            Components comp = new Components(curComp.startPoint, curComp.point2, curComp.point3, curComp.point4);
            comp.newPath.Height += (curComp.startPoint.Y > curComp.point4.Y ? curComp.point4.Y : curComp.startPoint.Y) - 100;
            comp.newPath.MouseRightButtonDown += viewbox_MouseRightButtonDown;
            comp.newPath.MouseDown += Img1_MouseLeftButtonDown;
            comp.newPath.MouseUp += Img1_MouseLeftButtonUp;
            if (stackpanel.Children.Contains(insertShape))
                components.Insert(index + 1, comp);
            else
                centralCubes.Insert(index + 1, comp);
            comp.newPath.Fill = curComp.newPath.Fill;

            stackpanel.Children.Insert(index + 1, comp.newPath);

            //判断是否存在层
            GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
            if (geometryGroup.Children.Count != 1)
            {
                for (int i = 1; i < geometryGroup.Children.Count; i++)
                {
                    // layerNum 总层数
                    ((Components)components[index + 1]).layerNum += 1;
                    // layerNums 
                    ((Components)components[index + 1]).layerNums.Add(curComp.layerNums[i - 1]);
                    ((Components)components[index + 1]).layerType.Add(curComp.layerType[i - 1]);
                    ((Components)components[index + 1]).layerMaterial.Add(curComp.layerMaterial[i - 1]);
                    ((Components)components[index + 1]).layerSize.Add(Int32.Parse(curComp.layerNums[i - 1].ToString()), new Hashtable());
                    ((Hashtable)(((Components)components[index + 1]).layerSize[Int32.Parse(curComp.layerNums[i - 1].ToString().ToString())])).Add("diameter", ((Hashtable)curComp.layerSize[Int32.Parse(curComp.layerNums[i - 1].ToString().ToString())])["diameter"]);
                    ((Hashtable)(((Components)components[index + 1]).layerSize[Int32.Parse(curComp.layerNums[i - 1].ToString())])).Add("longLength", ((Hashtable)curComp.layerSize[Int32.Parse(curComp.layerNums[i - 1].ToString().ToString())])["longLength"]);
                    ((Hashtable)(((Components)components[index + 1]).layerSize[Int32.Parse(curComp.layerNums[i - 1].ToString())])).Add("width", ((Hashtable)curComp.layerSize[Int32.Parse(curComp.layerNums[i - 1].ToString().ToString())])["width"]);
                    ((Hashtable)(((Components)components[index + 1]).layerSize[Int32.Parse(curComp.layerNums[i - 1].ToString())])).Add("height", ((Hashtable)curComp.layerSize[Int32.Parse(curComp.layerNums[i - 1].ToString().ToString())])["height"]);
                    ((Hashtable)(((Components)components[index + 1]).layerSize[Int32.Parse(curComp.layerNums[i - 1].ToString())])).Add("ObliqueAngle", ((Hashtable)curComp.layerSize[Int32.Parse(curComp.layerNums[i - 1].ToString().ToString())])["ObliqueAngle"]);
                    ((Components)components[index + 1]).layerLeftThickness.Add(curComp.layerLeftThickness[i - 1]);
                    ((Components)components[index + 1]).layerRightThickness.Add(curComp.layerRightThickness[i - 1]);
                }

                GeometryGroup geometryGroup_ = (GeometryGroup)(comp).newPath.Data;
                if (comp.layerNum <= 2)
                {
                    double leftWidth = Double.Parse(comp.layerLeftThickness[comp.layerNum - 2].ToString());
                    double rightWidth = Double.Parse(comp.layerRightThickness[comp.layerNum - 2].ToString());
                    double maxCh_Width = leftWidth > rightWidth ? leftWidth : rightWidth;
                    PathGeometry curPg = (PathGeometry)geometryGroup_.Children[0];
                    PathFigure curPf = curPg.Figures.ElementAt(0);
                    Point startP = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + maxCh_Width);
                    curPf.StartPoint = startP;
                    ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + maxCh_Width);
                    ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + maxCh_Width);
                    ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + maxCh_Width);
                    ((LineSegment)curPf.Segments[3]).Point = new Point(((LineSegment)curPf.Segments[3]).Point.X, ((LineSegment)curPf.Segments[3]).Point.Y + maxCh_Width);

                    //创建上下层路径
                    PathGeometry topPg = new PathGeometry();
                    PathGeometry bottomPg = new PathGeometry();

                    PathFigure topPf = new PathFigure();
                    PathFigure bottomPf = new PathFigure();

                    //绘制上层路径
                    topPf.StartPoint = curPf.StartPoint;
                    LineSegment ls_top = new LineSegment();
                    ls_top.Point = ((LineSegment)curPf.Segments[2]).Point;
                    topPf.Segments.Add(ls_top);

                    LineSegment ls2_top = new LineSegment();
                    Point thirdPoint_top = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y - rightWidth);
                    ls2_top.Point = thirdPoint_top;
                    topPf.Segments.Add(ls2_top);

                    LineSegment ls3_top = new LineSegment();
                    Point forthPoint_top = new Point(curPf.StartPoint.X, curPf.StartPoint.Y - leftWidth);
                    ls3_top.Point = forthPoint_top;
                    topPf.Segments.Add(ls3_top);
                    
                    topPg.Figures.Add(topPf);
                    topPf.IsClosed = true;

                    Color color = new Color();
                    if (DataBaseMaterials != null)
                    {
                        bool flag = false;
                        for (int j = 0; j < DataBaseMaterials.Count; j++)
                        {
                            Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["color"];
                            Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["materialName"];
                            string mColor = cdic["content"];
                            string materialName = mdic["content"];
                            if (materialName == ((Components)components[index + 1]).layerMaterial[((Components)components[index + 1]).layerNum - 2].ToString())
                            {
                                flag = true;
                                color = (Color)ColorConverter.ConvertFromString(mColor);
                            }
                        }
                        if (!flag)
                        {
                            color = (Color)ColorConverter.ConvertFromString("#FFFFA500");
                        }
                    }

                    ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, comp.newPath, topPf, 0, color);

                    //绘制下层路径
                    bottomPf.StartPoint = ((LineSegment)curPf.Segments[0]).Point;
                    LineSegment ls_bottom = new LineSegment();
                    Point secondPoint_bottom = new Point(bottomPf.StartPoint.X, bottomPf.StartPoint.Y + leftWidth);
                    ls_bottom.Point = secondPoint_bottom;
                    bottomPf.Segments.Add(ls_bottom);

                    LineSegment ls2_bottom = new LineSegment();
                    Point thirdPoint_bottom = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + rightWidth);
                    ls2_bottom.Point = thirdPoint_bottom;
                    bottomPf.Segments.Add(ls2_bottom);
                    
                    LineSegment ls3_bottom = new LineSegment();
                    Point forthPoint_bottom = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y);
                    ls3_bottom.Point = forthPoint_bottom;
                    bottomPf.Segments.Add(ls3_bottom);

                    if (curPf.Segments.Count > 3)
                    {
                        LineSegment ls4_bottom = new LineSegment();
                        ls4_bottom.Point = bottomPf.StartPoint;
                        bottomPf.Segments.Add(ls4_bottom);
                    }

                    bottomPg.Figures.Add(bottomPf);

                    if (DataBaseMaterials != null)
                    {
                        bool flag = false;
                        for (int j = 0; j < DataBaseMaterials.Count; j++)
                        {
                            Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["color"];
                            Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["materialName"];
                            string mColor = cdic["content"];
                            string materialName = mdic["content"];

                            if (materialName == ((Components)components[index + 1]).layerMaterial[((Components)components[index + 1]).layerNum - 2].ToString())
                            {
                                flag = true;
                                color = (Color)ColorConverter.ConvertFromString(mColor);
                            }
                        }
                        if (!flag)
                        {
                            color = (Color)ColorConverter.ConvertFromString("#FFFFA500");
                        }
                    }

                    ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, comp.newPath, bottomPf, 1, color);

                    //将上下层路径添加到组中
                    geometryGroup_.Children.Add(topPg);
                    geometryGroup_.Children.Add(bottomPg);

                    comp.newPath.Height += 2 * (leftWidth > rightWidth ? leftWidth : rightWidth);
                    comp.height = comp.newPath.Height;

                    //自动调整图形位置
                    autoResize();

                }
                else
                {
                    for (int k = 1; k < geometryGroup.Children.Count; k += 2)
                    {
                        double leftWidth = Double.Parse(comp.layerLeftThickness[geometryGroup_.Children.Count - 1].ToString());
                        double rightWidth = Double.Parse(comp.layerRightThickness[geometryGroup_.Children.Count - 1].ToString());
                        double maxCh_Width = leftWidth > rightWidth ? leftWidth : rightWidth;
                        int i;
                        PathGeometry curPg_ = (PathGeometry)geometryGroup_.Children[0];
                        PathFigure curPf_ = curPg_.Figures.ElementAt(0);
                        Point startP_ = new Point(curPf_.StartPoint.X, curPf_.StartPoint.Y + maxCh_Width);
                        curPf_.StartPoint = startP_;
                        ((LineSegment)curPf_.Segments[0]).Point = new Point(((LineSegment)curPf_.Segments[0]).Point.X, ((LineSegment)curPf_.Segments[0]).Point.Y + maxCh_Width);
                        ((LineSegment)curPf_.Segments[1]).Point = new Point(((LineSegment)curPf_.Segments[1]).Point.X, ((LineSegment)curPf_.Segments[1]).Point.Y + maxCh_Width);
                        ((LineSegment)curPf_.Segments[2]).Point = new Point(((LineSegment)curPf_.Segments[2]).Point.X, ((LineSegment)curPf_.Segments[2]).Point.Y + maxCh_Width);
                        if (curPf_.Segments.Count > 3)
                        {
                            ((LineSegment)curPf_.Segments[3]).Point = new Point(((LineSegment)curPf_.Segments[3]).Point.X, ((LineSegment)curPf_.Segments[3]).Point.Y + maxCh_Width);
                        }
                        //层
                        for (i = 2; i < geometryGroup_.Children.Count; i += 2)
                        {
                            PathGeometry curPg = (PathGeometry)geometryGroup_.Children[i - 1];
                            PathFigure curPf = curPg.Figures.ElementAt(0);
                            Point startP = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + maxCh_Width);
                            curPf.StartPoint = startP;
                            ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + maxCh_Width);
                            ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + maxCh_Width);
                            ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + maxCh_Width);

                            PathGeometry curPg2 = (PathGeometry)geometryGroup_.Children[i];
                            PathFigure curPf2 = curPg2.Figures.ElementAt(0);
                            Point startP2 = new Point(curPf2.StartPoint.X, curPf2.StartPoint.Y + maxCh_Width);
                            curPf2.StartPoint = startP2;
                            ((LineSegment)curPf2.Segments[0]).Point = new Point(((LineSegment)curPf2.Segments[0]).Point.X, ((LineSegment)curPf2.Segments[0]).Point.Y + maxCh_Width);
                            ((LineSegment)curPf2.Segments[1]).Point = new Point(((LineSegment)curPf2.Segments[1]).Point.X, ((LineSegment)curPf2.Segments[1]).Point.Y + maxCh_Width);
                            ((LineSegment)curPf2.Segments[2]).Point = new Point(((LineSegment)curPf2.Segments[2]).Point.X, ((LineSegment)curPf2.Segments[2]).Point.Y + maxCh_Width);
                            if (curPf2.Segments.Count > 3)
                            {
                                ((LineSegment)curPf2.Segments[3]).Point = new Point(((LineSegment)curPf2.Segments[3]).Point.X, ((LineSegment)curPf2.Segments[3]).Point.Y + maxCh_Width);
                            }
                        }

                        i -= 2;

                        if (i == 0)
                        {
                            //创建上下层路径
                            PathGeometry topPg = new PathGeometry();
                            PathGeometry bottomPg = new PathGeometry();

                            PathFigure topPf = new PathFigure();
                            PathFigure bottomPf = new PathFigure();

                            //绘制上层路径
                            topPf.StartPoint = curPf_.StartPoint;
                            LineSegment ls_top = new LineSegment();
                            ls_top.Point = ((LineSegment)curPf_.Segments[2]).Point;
                            topPf.Segments.Add(ls_top);

                            LineSegment ls2_top = new LineSegment();
                            Point thirdPoint_top = new Point(ls_top.Point.X, ls_top.Point.Y - rightWidth);
                            ls2_top.Point = thirdPoint_top;
                            topPf.Segments.Add(ls2_top);

                            LineSegment ls3_top = new LineSegment();
                            Point forthPoint_top = new Point(curPf_.StartPoint.X, curPf_.StartPoint.Y - leftWidth);
                            ls3_top.Point = forthPoint_top;
                            topPf.Segments.Add(ls3_top);
                           
                            topPg.Figures.Add(topPf);
                            topPf.IsClosed = true;

                            Color color = new Color();
                            if (DataBaseMaterials != null)
                            {
                                for (int j = 0; j < DataBaseMaterials.Count; j++)
                                {
                                    Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["color"];
                                    Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["materialName"];
                                    string mColor = cdic["content"];
                                    string materialName = mdic["content"];

                                    if (materialName == ((Components)components[index + 1]).layerMaterial[((Components)components[index + 1]).layerNum - 2].ToString())
                                    {
                                        color = (Color)ColorConverter.ConvertFromString(mColor);
                                    }
                                }
                            }

                            ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, comp.newPath, topPf, 0, color);

                            //绘制下层路径
                            bottomPf.StartPoint = ((LineSegment)curPf_.Segments[0]).Point;
                            LineSegment ls_bottom = new LineSegment();
                            Point secondPoint_bottom = new Point(bottomPf.StartPoint.X, bottomPf.StartPoint.Y + leftWidth);
                            ls_bottom.Point = secondPoint_bottom;
                            bottomPf.Segments.Add(ls_bottom);

                            LineSegment ls2_bottom = new LineSegment();
                            Point thirdPoint_bottom = new Point(((LineSegment)curPf_.Segments[1]).Point.X, ((LineSegment)curPf_.Segments[1]).Point.Y + rightWidth);
                            ls2_bottom.Point = thirdPoint_bottom;
                            bottomPf.Segments.Add(ls2_bottom);

                            LineSegment ls3_bottom = new LineSegment();
                            Point forthPoint_bottom = new Point(((LineSegment)curPf_.Segments[1]).Point.X, ((LineSegment)curPf_.Segments[1]).Point.Y);
                            ls3_bottom.Point = forthPoint_bottom;
                            bottomPf.Segments.Add(ls3_bottom);

                            if (curPf_.Segments.Count > 3)
                            {
                                LineSegment ls4_bottom = new LineSegment();
                                ls4_bottom.Point = bottomPf.StartPoint;
                                bottomPf.Segments.Add(ls4_bottom);
                            }
                            
                            bottomPg.Figures.Add(bottomPf);

                            if (DataBaseMaterials != null)
                            {
                                for (int j = 0; j < DataBaseMaterials.Count; j++)
                                {
                                    Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["color"];
                                    Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["materialName"];
                                    string mColor = cdic["content"];
                                    string materialName = mdic["content"];

                                    if (materialName == ((Components)components[index + 1]).layerMaterial[((Components)components[index + 1]).layerNum - 2].ToString())
                                    {
                                        color = (Color)ColorConverter.ConvertFromString(mColor);
                                    }

                                }
                            }

                            ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, comp.newPath, bottomPf, 1, color);

                            //将上下层路径添加到组中
                            geometryGroup_.Children.Add(topPg);
                            geometryGroup_.Children.Add(bottomPg);

                            comp.newPath.Height += 2 * (leftWidth > rightWidth ? leftWidth : rightWidth);
                            comp.height = comp.newPath.Height;


                            //自动调整图形位置
                            autoResize();
                        }
                        else
                        {
                            if (i < comp.layerNum)
                            {
                                PathGeometry curPg = (PathGeometry)geometryGroup_.Children[i - 1];
                                PathFigure curPf = curPg.Figures.ElementAt(0);
                                //创建上下层路径
                                PathGeometry topPg = new PathGeometry();
                                PathFigure topPf = new PathFigure();

                                //绘制上层路径
                                topPf.StartPoint = ((LineSegment)curPf.Segments[2]).Point;
                                LineSegment ls_top = new LineSegment();
                                ls_top.Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y);
                                topPf.Segments.Add(ls_top);

                                LineSegment ls2_top = new LineSegment();
                                Point thirdPoint_top = new Point(ls_top.Point.X, ls_top.Point.Y - rightWidth);
                                ls2_top.Point = thirdPoint_top;
                                topPf.Segments.Add(ls2_top);

                                LineSegment ls3_top = new LineSegment();
                                Point forthPoint_top = new Point(topPf.StartPoint.X, topPf.StartPoint.Y - leftWidth);
                                ls3_top.Point = forthPoint_top;
                                topPf.Segments.Add(ls3_top);
                               
                                topPg.Figures.Add(topPf);
                                topPf.IsClosed = true;

                                Color color = new Color();
                                if (DataBaseMaterials != null)
                                {
                                    for (int j = 0; j < DataBaseMaterials.Count; j++)
                                    {
                                        Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["color"];
                                        Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["materialName"];
                                        string mColor = cdic["content"];
                                        string materialName = mdic["content"];

                                        if (materialName == ((Components)components[index + 1]).layerMaterial[((Components)components[index + 1]).layerNum - 2].ToString())
                                        {
                                            color = (Color)ColorConverter.ConvertFromString(mColor);
                                        }

                                    }
                                }

                                ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, comp.newPath, topPf, 0, color);

                                //将上下层路径添加到组中
                                geometryGroup_.Children.Add(topPg);
                            }
                            if (i < comp.layerNum)
                            {
                                PathGeometry curPg2 = (PathGeometry)geometryGroup_.Children[i];
                                PathFigure curPf2 = curPg2.Figures.ElementAt(0);
                                PathGeometry bottomPg = new PathGeometry();
                                PathFigure bottomPf = new PathFigure();

                                //绘制下层路径
                                bottomPf.StartPoint = ((LineSegment)curPf2.Segments[0]).Point;
                                LineSegment ls_bottom = new LineSegment();
                                Point secondPoint_bottom = new Point(bottomPf.StartPoint.X, bottomPf.StartPoint.Y + leftWidth);
                                ls_bottom.Point = secondPoint_bottom;
                                bottomPf.Segments.Add(ls_bottom);

                                LineSegment ls2_bottom = new LineSegment();
                                Point thirdPoint_bottom = new Point(((LineSegment)curPf2.Segments[1]).Point.X, ((LineSegment)curPf2.Segments[1]).Point.Y + rightWidth);
                                ls2_bottom.Point = thirdPoint_bottom;
                                bottomPf.Segments.Add(ls2_bottom);
                                
                                LineSegment ls3_bottom = new LineSegment();
                                Point forthPoint_bottom = new Point(((LineSegment)curPf2.Segments[1]).Point.X, ((LineSegment)curPf2.Segments[1]).Point.Y);
                                ls3_bottom.Point = forthPoint_bottom;
                                bottomPf.Segments.Add(ls3_bottom);

                                if (curPf2.Segments.Count > 3)
                                {
                                    LineSegment ls4_bottom = new LineSegment();
                                    Point forthPoint_bottom2 = bottomPf.StartPoint;
                                    ls4_bottom.Point = forthPoint_bottom2;
                                    bottomPf.Segments.Add(ls4_bottom);
                                }

                                bottomPg.Figures.Add(bottomPf);
                                bottomPf.IsClosed = true;

                                Color color = new Color();
                                if (DataBaseMaterials != null)
                                {
                                    for (int j = 0; j < DataBaseMaterials.Count; j++)
                                    {
                                        Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["color"];
                                        Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["materialName"];
                                        string mColor = cdic["content"];
                                        string materialName = mdic["content"];

                                        if (materialName == ((Components)components[index + 1]).layerMaterial[((Components)components[index + 1]).layerNum - 2].ToString())
                                        {
                                            color = (Color)ColorConverter.ConvertFromString(mColor);
                                        }
                                    }
                                }

                                ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, comp.newPath, bottomPf, 1, color);


                                //将上下层路径添加到组中
                                geometryGroup_.Children.Add(bottomPg);

                            }
                            comp.newPath.Height += 2 * (leftWidth > rightWidth ? leftWidth : rightWidth);
                            comp.height = comp.newPath.Height;

                            //
                            autoResize();
                        }
                    }
                }

            }
            autoResize();
            //切换insertshape
            insertShape = comp.newPath;

            PathFigure compPf = ((PathGeometry)((GeometryGroup)curComp.newPath.Data).Children[0]).Figures[0];

            if (compPf.Segments[1] is PolyLineSegment)
            {
                if (curComp.isChangeOgive)
                {
                    changeLineSegmentToArcSegment(curComp.radius, 0);
                }
                else
                {
                    changeLineSegmentToArcSegment(curComp.radius, 1);
                }
            }

            comp.radius = curComp.radius;
            insertShape = curComp.newPath;

        }


        private void editLayerMenu_Click(object sender, RoutedEventArgs e)
        {
            layerEdit le = new layerEdit();
            le.list = DataBaseMaterials;
            le.currentLayerNum = curLayerNum;
            le.setComponent(currentComp);
            le.changeTitle = changeTitle;
            le.ChangeLayerSizeEvent += new ChangeLayerSizeHandler(autoResize);
            le.Show();
        }

        private void addCyliMenu_Click(object sender, RoutedEventArgs e)
        {
            arcSegMenu_Click(sender, e);
            //GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
            //PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
            //PathFigure curPf = curPg.Figures.ElementAt(0);

            //int num;
            //if (insertShape.Width % 20 == 0)
            //{
            //    num = (int)(insertShape.Width) / 20;
            //}
            //else
            //{
            //    num = (int)(insertShape.Width) / 20 + 1;
            //}

            //for (int i = 0; i < num;i++ )
            //{
            //    //创建上层球
            //    PathGeometry topPg = new PathGeometry();
            //    PathGeometry bottomPg = new PathGeometry();

            //    PathFigure topPf = new PathFigure();
            //    PathFigure bottomPf = new PathFigure();

            //    //绘制上层路径
            //    topPf.StartPoint = new Point(curPf.StartPoint.X + 20 * i, curPf.StartPoint.Y - 10);
            //    ArcSegment as_top = new ArcSegment();
            //    as_top.Point = new Point(curPf.StartPoint.X + 20 * i, curPf.StartPoint.Y - 10 -1);
            //    as_top.Size = new Size(10, 10);
            //    as_top.IsLargeArc = true;
            //    as_top.SweepDirection = SweepDirection.Counterclockwise;
            //    as_top.RotationAngle = 30;
            //    topPf.Segments.Add(as_top);
            //    topPg.Figures.Add(topPf);

            //    //绘制上层路径
            //    bottomPf.StartPoint = new Point(((LineSegment)curPf.Segments[0]).Point.X + 20 * i, ((LineSegment)curPf.Segments[0]).Point.Y + 10);
            //    ArcSegment as_bottom = new ArcSegment();
            //    as_bottom.Point = new Point(((LineSegment)curPf.Segments[0]).Point.X + 20 * i, ((LineSegment)curPf.Segments[0]).Point.Y + 9);
            //    as_bottom.Size = new Size(10, 10);
            //    as_bottom.IsLargeArc = true;
            //    as_bottom.SweepDirection = SweepDirection.Counterclockwise;
            //    as_bottom.RotationAngle = 30;
            //    bottomPf.Segments.Add(as_bottom);
            //    bottomPg.Figures.Add(bottomPf);

            //    geometryGroup.Children.Add(topPg);
            //    geometryGroup.Children.Add(bottomPg);
            //}
        }

        private void deleteLayer_Click(object sender, RoutedEventArgs e)
        {
            GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
            int count = geometryGroup.Children.Count;
            int index = stackpanel.Children.IndexOf(insertShape);
            if (geometryGroup.Children.Count > 1)
            {
                ((Components)components[index]).layerNum -= 2;
                ((Components)components[index]).layerNums.RemoveAt(((Components)components[index]).layerNums.Count - 1);
                ((Components)components[index]).layerNums.RemoveAt(((Components)components[index]).layerNums.Count - 1);
                ((Components)components[index]).layerType.RemoveAt(((Components)components[index]).layerType.Count - 1);
                ((Components)components[index]).layerType.RemoveAt(((Components)components[index]).layerType.Count - 1);
                ((Components)components[index]).layerMaterial.RemoveAt(((Components)components[index]).layerMaterial.Count - 1);
                ((Components)components[index]).layerMaterial.RemoveAt(((Components)components[index]).layerMaterial.Count - 1);
                ((Components)components[index]).layerSize.Remove(((Components)components[index]).layerSize.Count);
                ((Components)components[index]).layerSize.Remove(((Components)components[index]).layerSize.Count);
                ((Components)components[index]).layerLeftThickness.RemoveAt(((Components)components[index]).layerLeftThickness.Count - 1);
                ((Components)components[index]).layerLeftThickness.RemoveAt(((Components)components[index]).layerLeftThickness.Count - 1);
                ((Components)components[index]).layerRightThickness.RemoveAt(((Components)components[index]).layerRightThickness.Count - 1);
                ((Components)components[index]).layerRightThickness.RemoveAt(((Components)components[index]).layerRightThickness.Count - 1);

                for (int i = count - 1; i > count - 3; i--)
                {
                    ColorProc.processWhenDelLayer(front_canvas, ((PathGeometry)geometryGroup.Children[i]).Figures[0]);
                    geometryGroup.Children.RemoveAt(i);
                }
                makeLeftCoverConnectSkillfully();
                makeRightCoverConnectSkillfully();
                autoResize();
            }
        }

        //void changeWidthMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    int tempNum = 20;

        //    GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
        //    Console.WriteLine(geometryGroup.Children.Count);

        //    //先将基本构件宽度增加
        //    PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
        //    PathFigure curPf = curPg.Figures.ElementAt(0);
        //    if (curPf.Segments[1] is LineSegment)
        //    {
        //        ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X + tempNum, ((LineSegment)curPf.Segments[1]).Point.Y);
        //        currentComp.point3 = ((LineSegment)curPf.Segments[1]).Point;
        //    }
        //    else
        //    {
        //        ((ArcSegment)curPf.Segments[1]).Point = new Point(((ArcSegment)curPf.Segments[1]).Point.X + tempNum, ((ArcSegment)curPf.Segments[1]).Point.Y);
        //        currentComp.point3 = ((ArcSegment)curPf.Segments[1]).Point;
        //    }
        //    ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X + tempNum, ((LineSegment)curPf.Segments[2]).Point.Y);
        //    currentComp.point4 = ((LineSegment)curPf.Segments[2]).Point;

        //    //如果存在层，将层增加宽度  目前一层
        //    if(geometryGroup.Children.Count == 3)
        //    {
        //        PathGeometry top_Pg = (PathGeometry)geometryGroup.Children[1];
        //        PathFigure top_Pf = top_Pg.Figures.ElementAt(0);

        //        //判断直线还是曲线
        //        if (top_Pf.Segments[0] is LineSegment)
        //        {
        //            ((LineSegment)top_Pf.Segments[0]).Point = new Point(((LineSegment)top_Pf.Segments[0]).Point.X + tempNum, ((LineSegment)top_Pf.Segments[0]).Point.Y);
        //        }
        //        else
        //        {
        //            ((ArcSegment)top_Pf.Segments[0]).Point = new Point(((ArcSegment)top_Pf.Segments[0]).Point.X + tempNum, ((ArcSegment)top_Pf.Segments[0]).Point.Y);
        //        }

        //        if (top_Pf.Segments[1] is LineSegment)
        //        {
        //            ((LineSegment)top_Pf.Segments[1]).Point = new Point(((LineSegment)top_Pf.Segments[1]).Point.X + tempNum, ((LineSegment)top_Pf.Segments[1]).Point.Y);
        //        }
        //        else
        //        {
        //            ((ArcSegment)top_Pf.Segments[1]).Point = new Point(((ArcSegment)top_Pf.Segments[1]).Point.X + tempNum, ((ArcSegment)top_Pf.Segments[1]).Point.Y);
        //        }

        //        PathGeometry bottom_Pg = (PathGeometry)geometryGroup.Children[2];
        //        PathFigure bottom_Pf = bottom_Pg.Figures.ElementAt(0);

        //        //判断直线还是曲线
        //        if (bottom_Pf.Segments[1] is LineSegment)
        //        {
        //            ((LineSegment)bottom_Pf.Segments[1]).Point = new Point(((LineSegment)bottom_Pf.Segments[1]).Point.X + tempNum, ((LineSegment)bottom_Pf.Segments[1]).Point.Y);
        //        }
        //        else
        //        {
        //            ((ArcSegment)bottom_Pf.Segments[1]).Point = new Point(((ArcSegment)bottom_Pf.Segments[1]).Point.X + tempNum, ((ArcSegment)bottom_Pf.Segments[1]).Point.Y);
        //        }

        //        if (bottom_Pf.Segments[2] is LineSegment)
        //        {
        //            ((LineSegment)bottom_Pf.Segments[2]).Point = new Point(((LineSegment)bottom_Pf.Segments[2]).Point.X + tempNum, ((LineSegment)bottom_Pf.Segments[2]).Point.Y);
        //        }
        //        else
        //        {
        //            ((ArcSegment)bottom_Pf.Segments[2]).Point = new Point(((ArcSegment)bottom_Pf.Segments[2]).Point.X + tempNum, ((ArcSegment)bottom_Pf.Segments[2]).Point.Y);
        //        }
        //    }

        //    insertShape.Width += tempNum;
        //    autoResize();
        //}

        void arcSegMenu_Click(object sender, RoutedEventArgs e)
        {
            GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
            PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
            PathFigure curPf = curPg.Figures.ElementAt(0);

            //初始化曲线
            PolyLineSegment as_top = new PolyLineSegment();
            //Point Point1 = new Point(10, 90);
            //Point Point2 = new Point(20, 85);
            //Point Point3 = new Point(30, 82);
            //Point Point4 = new Point(40, 80);
            //Point Point5 = new Point(50, 81);
            //Point Point6 = new Point(60, 82);
            //Point Point7 = new Point(80, 85);
            Point Point1 = new Point(10, 110);
            Point Point2 = new Point(20, 115);
            Point Point3 = new Point(30, 118);
            Point Point4 = new Point(40, 120);
            Point Point5 = new Point(50, 119);
            Point Point6 = new Point(60, 118);
            Point Point7 = new Point(80, 115);
            as_top.Points.Add(((LineSegment)curPf.Segments[2]).Point);
            as_top.Points.Add(Point7);
            as_top.Points.Add(Point6);
            as_top.Points.Add(Point5);
            as_top.Points.Add(Point4);
            as_top.Points.Add(Point3);
            as_top.Points.Add(Point2);
            as_top.Points.Add(Point1);
            as_top.Points.Add(curPf.StartPoint);

            PolyLineSegment as_bottom = new PolyLineSegment();
            Point point1 = new Point(10, ((LineSegment)curPf.Segments[0]).Point.Y - 10);
            Point point2 = new Point(20, ((LineSegment)curPf.Segments[0]).Point.Y - 15);
            Point point3 = new Point(30, ((LineSegment)curPf.Segments[0]).Point.Y - 18);
            Point point4 = new Point(40, ((LineSegment)curPf.Segments[0]).Point.Y - 20);
            Point point5 = new Point(50, ((LineSegment)curPf.Segments[0]).Point.Y - 19);
            Point point6 = new Point(60, ((LineSegment)curPf.Segments[0]).Point.Y - 18);
            Point point7 = new Point(80, ((LineSegment)curPf.Segments[0]).Point.Y - 15);
            as_bottom.Points.Add(((LineSegment)curPf.Segments[0]).Point);
            as_bottom.Points.Add(point1);
            as_bottom.Points.Add(point2);
            as_bottom.Points.Add(point3);
            as_bottom.Points.Add(point4);
            as_bottom.Points.Add(point5);
            as_bottom.Points.Add(point6);
            as_bottom.Points.Add(point7);
            as_bottom.Points.Add(((LineSegment)curPf.Segments[1]).Point);


            //曲线
            curPf.Segments[3] = as_top;
            curPf.Segments[1] = as_bottom;
        }

        /**
         * 返回可能的两个圆心
         */
        public Point[] calcuCentralPoints(Point p1, Point p2, double r)
        {
            Point[] points = new Point[2];
            // p1与p2的距离
            double dis_p1_p2 = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            // p1与p2距离的一半
            double half_dis_p1_p2 = dis_p1_p2 / 2;
            // p1与p2的x轴距离
            double dis_x = Math.Abs(p1.X - p2.X);
            // p1与p2的y轴距离
            double dis_y = Math.Abs(p1.Y - p2.Y);
            // 圆心到p1p2线段中心的距离
            double dis_central_to_half_p1_p2 = Math.Sqrt(Math.Pow(r, 2) - Math.Pow(half_dis_p1_p2, 2));
            if (p1.Y > p2.Y)
            {
                // 
                double alpha = Math.Acos(half_dis_p1_p2 / r);
                //
                double beta = Math.Atan(dis_y / dis_x);
                //
                double theta = alpha - beta;
                points[0] = new Point();
                points[0].X = p1.X + r * Math.Cos(theta);
                points[0].Y = p1.Y + r * Math.Sin(theta);
                points[1] = new Point();
                double half_x = (p1.X + p2.X) / 2;
                double half_y = (p1.Y + p2.Y) / 2;
                points[1].X = 2 * half_x - points[0].X;
                points[1].Y = 2 * half_y - points[0].Y;
                return points;
            }
            else
            {
                // 
                double alpha = Math.Acos(half_dis_p1_p2 / r);
                //
                double beta = Math.Atan(dis_x / dis_y);
                //
                double theta = alpha - beta;
                points[0] = new Point();
                points[0].X = p1.X - r * Math.Sin(theta);
                points[0].Y = p1.Y + r * Math.Cos(theta);
                points[1] = new Point();
                double half_x = (p1.X + p2.X) / 2;
                double half_y = (p1.Y + p2.Y) / 2;
                points[1].X = 2 * half_x - points[0].X;
                points[1].Y = 2 * half_y - points[0].Y;
                return points;
            }

        }

        /*
         * 根据圆心，半径，x区域，点的个数，得到一系列点,p1
         * p1终点，p2为起始点
         */
        public PointCollection findPolyPointsByCircle(Point p0, double r, Point p1, Point p2, int num, int isConvex)
        {
            PointCollection points = new PointCollection();
            Point point;
            double step_x = Math.Abs(p1.X - p2.X) / num;
            double temp_x = p2.X;
            points.Add(p2);
            double temp_y = 0;
            if (isConvex == 0)
            {
                // 凸
                //if (p0.Y >= p1.Y)
                //{
                while ((temp_x -= step_x) >= p1.X)
                {
                    temp_y = p0.Y - Math.Sqrt(r * r - Math.Pow(temp_x - p0.X, 2));
                    point = new Point();
                    point.X = temp_x;
                    point.Y = temp_y;
                    points.Add(point);
                }
                //}

            }
            else
            {
                //凹

                while ((temp_x -= step_x) >= p1.X)
                {
                    temp_y = p0.Y + Math.Sqrt(r * r - Math.Pow(temp_x - p0.X, 2));
                    point = new Point();
                    point.X = temp_x;
                    point.Y = temp_y;
                    points.Add(point);
                }
            }
            points.Add(p1);
            return points;
        }

        /*
         * 根据上层圆弧上的点对称求下层的点
         * y_mid 为中间点y坐标
         */
        public PointCollection getSymmetricPoint(PointCollection pointsUp, double y_mid)
        {
            PointCollection points = new PointCollection();
            Point point;
            PointCollection.Enumerator pe = pointsUp.GetEnumerator();
            while (pe.MoveNext())
            {
                point = new Point();
                point.X = pe.Current.X;
                point.Y = 2 * y_mid - pe.Current.Y;
                points.Insert(0, point);
            }
            return points;
        }

        /**
         * reverse PointCollection
         */
        public PointCollection reversePointCollection(PointCollection pointCollection)
        {
            PointCollection points = new PointCollection();
            PointCollection.Enumerator pe = pointCollection.GetEnumerator();
            Point point;
            while (pe.MoveNext())
            {
                point = new Point();
                point.X = pe.Current.X;
                point.Y = pe.Current.Y;
                points.Insert(0, point);
            }
            return points;
        }


        /*
         * 把直线的段变为弧形，radius为圆弧的半径，isConvex为凸与凹的标志
         * if isConvex = 0 凸 else 凹
         */
        public void changeLineSegmentToArcSegment(double radius, int isConvex)
        {
            //绘制上段
            Point p1, p2;
            GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
            geometryGroup.FillRule = FillRule.Nonzero;
            int index;
            if (changeTitle == 0 || changeTitle == 1)
            {
                index = stackpanel.Children.IndexOf(insertShape);
                if (isConvex == 0)
                {
                    ((Components)components[index]).isChangeOgive = true;
                    ((Components)components[index]).isChangeIOgive = false;
                }
                else
                {
                    ((Components)components[index]).isChangeOgive = false;
                    ((Components)components[index]).isChangeIOgive = true;
                }
            }
            else if (changeTitle == 2)
            {
                index = centralCubePanel.Children.IndexOf(insertShape);
                if (isConvex == 0)
                {
                    ((Components)centralCubes[index]).isChangeOgive = true;
                    ((Components)centralCubes[index]).isChangeIOgive = false;
                }
                else
                {
                    ((Components)centralCubes[index]).isChangeOgive = false;
                    ((Components)centralCubes[index]).isChangeIOgive = true;
                }
            }

            //未添加层时，直接根据段添加弧形
            PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
            PathFigure curPf = curPg.Figures.ElementAt(0);
            p1 = new Point(curPf.StartPoint.X, curPf.StartPoint.Y);
            p2 = ((LineSegment)curPf.Segments[2]).Point;

            ArcSegment arcSegment = new ArcSegment();
            PolyLineSegment polyLineSegment = new PolyLineSegment();
            PointCollection points = new PointCollection();
            Point[] circle_centers = calcuCentralPoints(p1, p2, radius);

            Point point_in_x1_x2 = new Point(Math.Abs((p1.X - p2.X) / 2), Math.Abs((p1.Y - p2.Y) / 2));
            double distance_x1_x2 = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            double radius1, radius2 = 0;
            if (radius < distance_x1_x2 / 2)
            {
                return;
            }
            else
            {
                radius1 = distance_x1_x2 / 2;
                radius2 = radius - Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(radius1, 2));
            }
            arcSegment.Size = new Size(radius1, radius2);
            arcSegment.Point = p1;

            if (isConvex == 0)
            {   //凸
                points = findPolyPointsByCircle(circle_centers[0], radius, p1, p2, 1000, isConvex);
                polyLineSegment.Points = points;
                arcSegment.SweepDirection = SweepDirection.Counterclockwise;
            }
            else
            {   //凹
                points = findPolyPointsByCircle(circle_centers[1], radius, p1, p2, 1000, isConvex);
                polyLineSegment.Points = points;
                arcSegment.SweepDirection = SweepDirection.Clockwise;
            }
            
            curPf.Segments[3] = polyLineSegment;

            if (curLayerNum != 0)
            {
                //return;
                //有层时，根据层数绘制弧形
                //每层默认height 20
                    //有层且层为polysegment处理
                    Point _p1, _p2, _p3, _p4;
                    PathGeometry _curPg;
                    PathFigure _curPf;
                    PolyLineSegment pls1, pls2;
                    for (int i = 1; i <= curLayerNum; i += 2)
                    {
                        _curPg = (PathGeometry)geometryGroup.Children[i];
                        _curPf = _curPg.Figures.ElementAt(0);
                        pls1 = new PolyLineSegment();
                        pls2 = new PolyLineSegment();
                        _p1 = _curPf.StartPoint;
                        if (_curPf.Segments[0] is LineSegment)
                        {
                            _p2 = ((LineSegment)_curPf.Segments[0]).Point;
                        }
                        else
                        {
                            _p2 = new Point();
                        }
                        _p3 = ((LineSegment)_curPf.Segments[1]).Point;

                        if (_curPf.Segments[2] is LineSegment)
                        {
                            _p4 = ((LineSegment)_curPf.Segments[2]).Point;
                        }
                        else
                        {
                            _p4 = new Point();
                        }
                        if (isConvex == 0)
                        {
                            Point[] _circle_centers = calcuCentralPoints(_p1, _p2, radius);
                            PointCollection _points = findPolyPointsByCircle(_circle_centers[0], radius, _p1, _p2, 1000, isConvex);
                            _points = reversePointCollection(_points);
                            pls1.Points = _points;

                            PointCollection _downPoints = getSymmetricPoint(_points, Math.Abs(p1.Y + ((LineSegment)curPf.Segments[0]).Point.Y) / 2);
                            PathGeometry _downPg = (PathGeometry)geometryGroup.Children[i + 1];
                            PathFigure _downPf = _downPg.Figures.ElementAt(0);
                            PolyLineSegment _polyLineSegment = new PolyLineSegment();
                            _polyLineSegment.Points = _downPoints;
                            if (_downPf.Segments.Count < 4)
                            {
                                _downPf.Segments.Add(_polyLineSegment);
                            }
                            _downPf.Segments[3] = _polyLineSegment;
                        }
                        else
                        {
                            Point[] _circle_centers = calcuCentralPoints(_p1, _p2, radius);
                            PointCollection _points = findPolyPointsByCircle(_circle_centers[1], radius, _p1, _p2, 1000, isConvex);
                            _points = reversePointCollection(_points);
                            pls1.Points = _points;

                            PointCollection _downPoints = getSymmetricPoint(_points, Math.Abs(p1.Y + ((LineSegment)curPf.Segments[0]).Point.Y) / 2);
                            PathGeometry _downPg = (PathGeometry)geometryGroup.Children[i + 1];
                            PathFigure _downPf = _downPg.Figures.ElementAt(0);
                            PolyLineSegment _polyLineSegment = new PolyLineSegment();
                            _polyLineSegment.Points = _downPoints;
                            if (_downPf.Segments.Count < 4)
                            {
                                _downPf.Segments.Add(_polyLineSegment);
                            }
                            _downPf.Segments[3] = _polyLineSegment;
                        }

                        if (isConvex == 0)
                        {
                            Point[] _circle_centers = calcuCentralPoints(_p4, _p3, radius);
                            PointCollection _points = findPolyPointsByCircle(_circle_centers[0], radius, _p4, _p3, 1000, isConvex);
                            pls2.Points = _points;

                            PointCollection _downPoints = getSymmetricPoint(_points, Math.Abs(p1.Y + ((LineSegment)curPf.Segments[0]).Point.Y) / 2);
                            PathGeometry _downPg = (PathGeometry)geometryGroup.Children[i + 1];
                            PathFigure _downPf = _downPg.Figures.ElementAt(0);
                            _downPf.Segments[1] = new PolyLineSegment();
                            ((PolyLineSegment)_downPf.Segments[1]).Points = _downPoints;
                        }
                        else
                        {
                            Point[] _circle_centers = calcuCentralPoints(_p4, _p3, radius);
                            PointCollection _points = findPolyPointsByCircle(_circle_centers[1], radius, _p4, _p3, 1000, isConvex);
                            pls2.Points = _points;

                            PointCollection _downPoints = getSymmetricPoint(_points, Math.Abs(p1.Y + ((LineSegment)curPf.Segments[0]).Point.Y) / 2);
                            PathGeometry _downPg = (PathGeometry)geometryGroup.Children[i + 1];
                            PathFigure _downPf = _downPg.Figures.ElementAt(0);
                            _downPf.Segments[1] = new PolyLineSegment();
                            ((PolyLineSegment)_downPf.Segments[1]).Points = _downPoints;
                        }

                        _curPf.Segments[0] = pls1;
                        _curPf.Segments[2] = pls2;

                }
            }


            //绘制下段

            PointCollection downPoints = getSymmetricPoint(polyLineSegment.Points, Math.Abs(p1.Y + ((LineSegment)curPf.Segments[0]).Point.Y) / 2);
            PolyLineSegment downPolyLineSegment = new PolyLineSegment();
            downPolyLineSegment.Points = downPoints;
            p1 = ((LineSegment)curPf.Segments[0]).Point;
            if (curPf.Segments[1] is LineSegment)
            {
                p2 = ((LineSegment)curPf.Segments[1]).Point;
            }
            else if (curPf.Segments[1] is ArcSegment)
            {
                p2 = ((ArcSegment)curPf.Segments[1]).Point;
            }
            else
            {
                PointCollection pointCollection = ((PolyLineSegment)curPf.Segments[1]).Points;
                Point point = ((PolyLineSegment)curPf.Segments[1]).Points[pointCollection.Count - 1];
                p2 = new Point(point.X, point.Y);
            }

            arcSegment = new ArcSegment();
            arcSegment.Size = new Size(radius1, radius2);
            arcSegment.Point = p2;
            if (isConvex == 0)
            {//凸
                arcSegment.SweepDirection = SweepDirection.Counterclockwise;
            }
            else
            {//凹
                arcSegment.SweepDirection = SweepDirection.Clockwise;
            }
            curPf.Segments[1] = downPolyLineSegment;
            Console.WriteLine("curLayerNum: " + curLayerNum);
            
            if (changeTitle == 0)
            {
                ColorProc.processWhenChangeLayerShape(front_canvas, stackpanel, insertShape);
            }
            else if (changeTitle == 2)
            {
                ColorProc.processWhenChangeLayerShape_CC(front_canvas, centralCubePanel, insertShape);
            }
        }

        public void changeLineSegmentToPolySegmentForLayer(double radius, int isConvex) {
            //绘制上段
            Point p1, p2;
            GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
            geometryGroup.FillRule = FillRule.Nonzero;
            
            int index;
            if (changeTitle == 0 || changeTitle == 1)
            {
                index = stackpanel.Children.IndexOf(insertShape);
                if (isConvex == 0)
                {
                    ((Components)components[index]).isChangeOgive = true;
                    ((Components)components[index]).isChangeIOgive = false;
                }
                else
                {
                    ((Components)components[index]).isChangeOgive = false;
                    ((Components)components[index]).isChangeIOgive = true;
                }
            }
            else if (changeTitle == 2)
            {
                index = centralCubePanel.Children.IndexOf(insertShape);
                if (isConvex == 0)
                {
                    ((Components)centralCubes[index]).isChangeOgive = true;
                    ((Components)centralCubes[index]).isChangeIOgive = false;
                }
                else
                {
                    ((Components)centralCubes[index]).isChangeOgive = false;
                    ((Components)centralCubes[index]).isChangeIOgive = true;
                }
            }

            //未添加层时，直接根据段添加弧形
            PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
            PathFigure curPf = curPg.Figures.ElementAt(0);
            p1 = new Point(curPf.StartPoint.X, curPf.StartPoint.Y);
            p2 = ((LineSegment)curPf.Segments[2]).Point;

            ArcSegment arcSegment = new ArcSegment();
            PolyLineSegment polyLineSegment = new PolyLineSegment();
            PointCollection points = new PointCollection();
            Point[] circle_centers = calcuCentralPoints(p1, p2, radius);

            Point point_in_x1_x2 = new Point(Math.Abs((p1.X - p2.X) / 2), Math.Abs((p1.Y - p2.Y) / 2));
            double distance_x1_x2 = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            double radius1, radius2 = 0;
            if (radius < distance_x1_x2 / 2)
            {
                return;
            }
            else
            {
                radius1 = distance_x1_x2 / 2;
                radius2 = radius - Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(radius1, 2));
            }
            arcSegment.Size = new Size(radius1, radius2);
            arcSegment.Point = p1;

            if (isConvex == 0)
            {   //凸
                points = findPolyPointsByCircle(circle_centers[0], radius, p1, p2, 1000, isConvex);
                polyLineSegment.Points = points;
                arcSegment.SweepDirection = SweepDirection.Counterclockwise;
            }
            else
            {   //凹
                points = findPolyPointsByCircle(circle_centers[1], radius, p1, p2, 1000, isConvex);
                polyLineSegment.Points = points;
                arcSegment.SweepDirection = SweepDirection.Clockwise;
            }
            curPf.Segments[3] = polyLineSegment;

            if (curLayerNum != 0)
            {
                //return;
                //有层时，根据层数绘制弧形
                //每层默认height 20
                    //有层且层为polysegment处理
                    Point _p1, _p2, _p3, _p4;
                    PathGeometry _curPg;
                    PathFigure _curPf;
                    PolyLineSegment pls1, pls2;
                    for (int i = 1; i <= curLayerNum; i += 2)
                    {
                        _curPg = (PathGeometry)geometryGroup.Children[i];
                        _curPf = _curPg.Figures.ElementAt(0);
                        pls1 = new PolyLineSegment();
                        pls2 = new PolyLineSegment();
                        _p1 = _curPf.StartPoint;
                        if (_curPf.Segments[0] is LineSegment)
                        {
                            _p2 = ((LineSegment)_curPf.Segments[0]).Point;
                        }
                        else
                        {
                            _p2 = new Point();
                        }
                        _p3 = ((LineSegment)_curPf.Segments[1]).Point;

                        if (_curPf.Segments[2] is LineSegment)
                        {
                            _p4 = ((LineSegment)_curPf.Segments[2]).Point;
                        }
                        else
                        {
                            _p4 = new Point();
                        }
                        if (isConvex == 0)
                        {
                            Point[] _circle_centers = calcuCentralPoints(_p1, _p2, radius);
                            PointCollection _points = findPolyPointsByCircle(_circle_centers[0], radius, _p1, _p2, 1000, isConvex);
                            _points = reversePointCollection(_points);
                            pls1.Points = _points;

                            PointCollection _downPoints = getSymmetricPoint(_points, Math.Abs(p1.Y + ((LineSegment)curPf.Segments[0]).Point.Y) / 2);
                            PathGeometry _downPg = (PathGeometry)geometryGroup.Children[i + 1];
                            PathFigure _downPf = _downPg.Figures.ElementAt(0);
                            PolyLineSegment _polyLineSegment = new PolyLineSegment();
                            _polyLineSegment.Points = _downPoints;
                            if (_downPf.Segments.Count < 4)
                            {
                                _downPf.Segments.Add(_polyLineSegment);
                            }
                            _downPf.Segments[3] = _polyLineSegment;
                        }
                        else
                        {
                            Point[] _circle_centers = calcuCentralPoints(_p1, _p2, radius);
                            PointCollection _points = findPolyPointsByCircle(_circle_centers[1], radius, _p1, _p2, 1000, isConvex);
                            _points = reversePointCollection(_points);
                            pls1.Points = _points;

                            PointCollection _downPoints = getSymmetricPoint(_points, Math.Abs(p1.Y + ((LineSegment)curPf.Segments[0]).Point.Y) / 2);
                            PathGeometry _downPg = (PathGeometry)geometryGroup.Children[i + 1];
                            PathFigure _downPf = _downPg.Figures.ElementAt(0);
                            PolyLineSegment _polyLineSegment = new PolyLineSegment();
                            _polyLineSegment.Points = _downPoints;
                            if (_downPf.Segments.Count < 4)
                            {
                                _downPf.Segments.Add(_polyLineSegment);
                            }
                            _downPf.Segments[3] = _polyLineSegment;
                        }

                        if (isConvex == 0)
                        {
                            Point[] _circle_centers = calcuCentralPoints(_p4, _p3, radius);
                            PointCollection _points = findPolyPointsByCircle(_circle_centers[0], radius, _p4, _p3, 1000, isConvex);
                            pls2.Points = _points;

                            PointCollection _downPoints = getSymmetricPoint(_points, Math.Abs(p1.Y + ((LineSegment)curPf.Segments[0]).Point.Y) / 2);
                            PathGeometry _downPg = (PathGeometry)geometryGroup.Children[i + 1];
                            PathFigure _downPf = _downPg.Figures.ElementAt(0);
                            _downPf.Segments[1] = new PolyLineSegment();
                            ((PolyLineSegment)_downPf.Segments[1]).Points = _downPoints;
                        }
                        else
                        {
                            Point[] _circle_centers = calcuCentralPoints(_p4, _p3, radius);
                            PointCollection _points = findPolyPointsByCircle(_circle_centers[1], radius, _p4, _p3, 1000, isConvex);
                            pls2.Points = _points;

                            PointCollection _downPoints = getSymmetricPoint(_points, Math.Abs(p1.Y + ((LineSegment)curPf.Segments[0]).Point.Y) / 2);
                            PathGeometry _downPg = (PathGeometry)geometryGroup.Children[i + 1];
                            PathFigure _downPf = _downPg.Figures.ElementAt(0);
                            _downPf.Segments[1] = new PolyLineSegment();
                            ((PolyLineSegment)_downPf.Segments[1]).Points = _downPoints;
                        }

                        _curPf.Segments[0] = pls1;
                        _curPf.Segments[2] = pls2;
                }
            }


            //绘制下段

            PointCollection downPoints = getSymmetricPoint(polyLineSegment.Points, Math.Abs(p1.Y + ((LineSegment)curPf.Segments[0]).Point.Y) / 2);
            PolyLineSegment downPolyLineSegment = new PolyLineSegment();
            downPolyLineSegment.Points = downPoints;
            p1 = ((LineSegment)curPf.Segments[0]).Point;
            if (curPf.Segments[1] is LineSegment)
            {
                p2 = ((LineSegment)curPf.Segments[1]).Point;
            }
            else if (curPf.Segments[1] is ArcSegment)
            {
                p2 = ((ArcSegment)curPf.Segments[1]).Point;
            }
            else
            {
                PointCollection pointCollection = ((PolyLineSegment)curPf.Segments[1]).Points;
                Point point = ((PolyLineSegment)curPf.Segments[1]).Points[pointCollection.Count - 1];
                p2 = new Point(point.X, point.Y);
            }

            arcSegment = new ArcSegment();
            arcSegment.Size = new Size(radius1, radius2);
            arcSegment.Point = p2;
            if (isConvex == 0)
            {//凸
                arcSegment.SweepDirection = SweepDirection.Counterclockwise;
            }
            else
            {//凹
                arcSegment.SweepDirection = SweepDirection.Clockwise;
            }
            
                curPf.Segments[1] = downPolyLineSegment;
            
            Console.WriteLine("curLayerNum: " + curLayerNum);
            if(changeTitle == 0)
                ColorProc.processWhenChangeLayerShape(front_canvas, stackpanel, insertShape);
            else if(changeTitle == 2)
                ColorProc.processWhenChangeLayerShape_CC(front_canvas, centralCubePanel, insertShape);
        }


        public void changeArcSegmentToLineSegment(double a, int b)
        {
            int index;
            if (changeTitle == 0 || changeTitle == 1)
            {
                index = stackpanel.Children.IndexOf(insertShape);
                if (index >= 0)
                {
                    ((Components)components[index]).isChangeOgive = false;
                    ((Components)components[index]).isChangeIOgive = false;
                    GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
                    PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
                    PathFigure curPf = curPg.Figures.ElementAt(0);

                    //无层的时候  变换
                    Point p1, p2, p3, p4;
                    p1 = new Point(curPf.StartPoint.X, curPf.StartPoint.Y);
                    p2 = ((LineSegment)curPf.Segments[0]).Point;
                    if (curPf.Segments[1] is LineSegment)
                    {
                        p3 = ((LineSegment)curPf.Segments[1]).Point;
                    }
                    else if (curPf.Segments[1] is ArcSegment)
                    {
                        p3 = ((ArcSegment)curPf.Segments[1]).Point;
                    }
                    else
                    {
                        PointCollection pointCollection = ((PolyLineSegment)curPf.Segments[1]).Points;
                        Point point = ((PolyLineSegment)curPf.Segments[1]).Points[pointCollection.Count - 1];
                        p3 = new Point(point.X, point.Y);
                    }
                    p4 = ((LineSegment)curPf.Segments[2]).Point;
                    LineSegment lineSegment1 = new LineSegment();
                    lineSegment1.Point = p3;
                    curPf.Segments[1] = lineSegment1;
                    LineSegment lineSegment2 = new LineSegment();
                    lineSegment2.Point = p1;
                    curPf.Segments[3] = lineSegment2;

                    //有层
                    if (curLayerNum > 0)
                    {
                        for (int i = 1; i < curLayerNum; i += 2)
                        {
                            PathGeometry topPg = (PathGeometry)geometryGroup.Children[i];
                            PathFigure topPf = topPg.Figures.ElementAt(0);
                            if (topPf.Segments[0] is ArcSegment)
                            {
                                LineSegment ls_top = new LineSegment();
                                ls_top.Point = ((ArcSegment)topPf.Segments[0]).Point;
                                topPf.Segments[0] = ls_top;

                                LineSegment ls_top2 = new LineSegment();
                                ls_top2.Point = ((ArcSegment)topPf.Segments[2]).Point;
                                topPf.Segments[2] = ls_top2;
                            }
                            else if (topPf.Segments[0] is PolyLineSegment)
                            {
                                LineSegment ls_top = new LineSegment();
                                ls_top.Point = ((PolyLineSegment)topPf.Segments[0]).Points[((PolyLineSegment)topPf.Segments[0]).Points.Count - 1];
                                topPf.Segments[0] = ls_top;

                                LineSegment ls_top2 = new LineSegment();
                                ls_top2.Point = ((PolyLineSegment)topPf.Segments[2]).Points[((PolyLineSegment)topPf.Segments[2]).Points.Count - 1];
                                topPf.Segments[2] = ls_top2;
                            }



                            PathGeometry bottomPg = (PathGeometry)geometryGroup.Children[i + 1];
                            PathFigure bottomPf = bottomPg.Figures.ElementAt(0);

                            if (bottomPf.Segments[1] is ArcSegment)
                            {
                                LineSegment ls_bottom = new LineSegment();
                                ls_bottom.Point = ((ArcSegment)bottomPf.Segments[1]).Point;
                                bottomPf.Segments[1] = ls_bottom;

                                LineSegment ls_bottom2 = new LineSegment();
                                ls_bottom2.Point = ((ArcSegment)bottomPf.Segments[3]).Point;
                                bottomPf.Segments[3] = ls_bottom2;
                            }
                            else if (bottomPf.Segments[1] is PolyLineSegment)
                            {
                                LineSegment ls_bottom = new LineSegment();
                                ls_bottom.Point = ((PolyLineSegment)bottomPf.Segments[1]).Points[((PolyLineSegment)bottomPf.Segments[1]).Points.Count - 1];
                                bottomPf.Segments[1] = ls_bottom;

                                LineSegment ls_bottom2 = new LineSegment();
                                ls_bottom2.Point = ((PolyLineSegment)bottomPf.Segments[3]).Points[((PolyLineSegment)bottomPf.Segments[3]).Points.Count - 1];
                                bottomPf.Segments[3] = ls_bottom2;
                            }
                        }
                    }
                }
                ColorProc.processWhenChangeLayerShape(front_canvas, stackpanel, insertShape);
            }
            else if (changeTitle == 2)
            {
                index = centralCubePanel.Children.IndexOf(insertShape);
                if (index >= 0)
                {
                    ((Components)centralCubes[index]).isChangeOgive = false;
                    ((Components)centralCubes[index]).isChangeIOgive = false;
                    GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
                    PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
                    PathFigure curPf = curPg.Figures.ElementAt(0);

                    //无层的时候  变换
                    Point p1, p2, p3, p4;
                    p1 = new Point(curPf.StartPoint.X, curPf.StartPoint.Y);
                    p2 = ((LineSegment)curPf.Segments[0]).Point;
                    if (curPf.Segments[1] is LineSegment)
                    {
                        p3 = ((LineSegment)curPf.Segments[1]).Point;
                    }
                    else if (curPf.Segments[1] is ArcSegment)
                    {
                        p3 = ((ArcSegment)curPf.Segments[1]).Point;
                    }
                    else
                    {
                        PointCollection pointCollection = ((PolyLineSegment)curPf.Segments[1]).Points;
                        Point point = ((PolyLineSegment)curPf.Segments[1]).Points[pointCollection.Count - 1];
                        p3 = new Point(point.X, point.Y);
                    }
                    p4 = ((LineSegment)curPf.Segments[2]).Point;
                    LineSegment lineSegment1 = new LineSegment();
                    lineSegment1.Point = p3;
                    curPf.Segments[1] = lineSegment1;
                    LineSegment lineSegment2 = new LineSegment();
                    lineSegment2.Point = p1;
                    curPf.Segments[3] = lineSegment2;

                    //有层
                    if (curLayerNum > 0)
                    {
                        for (int i = 1; i < curLayerNum; i += 2)
                        {
                            PathGeometry topPg = (PathGeometry)geometryGroup.Children[i];
                            PathFigure topPf = topPg.Figures.ElementAt(0);
                            if (topPf.Segments[0] is ArcSegment)
                            {
                                LineSegment ls_top = new LineSegment();
                                ls_top.Point = ((ArcSegment)topPf.Segments[0]).Point;
                                topPf.Segments[0] = ls_top;

                                LineSegment ls_top2 = new LineSegment();
                                ls_top2.Point = ((ArcSegment)topPf.Segments[2]).Point;
                                topPf.Segments[2] = ls_top2;
                            }
                            else if (topPf.Segments[0] is PolyLineSegment)
                            {
                                LineSegment ls_top = new LineSegment();
                                ls_top.Point = ((PolyLineSegment)topPf.Segments[0]).Points[((PolyLineSegment)topPf.Segments[0]).Points.Count - 1];
                                topPf.Segments[0] = ls_top;

                                LineSegment ls_top2 = new LineSegment();
                                ls_top2.Point = ((PolyLineSegment)topPf.Segments[2]).Points[((PolyLineSegment)topPf.Segments[2]).Points.Count - 1];
                                topPf.Segments[2] = ls_top2;
                            }



                            PathGeometry bottomPg = (PathGeometry)geometryGroup.Children[i + 1];
                            PathFigure bottomPf = bottomPg.Figures.ElementAt(0);

                            if (bottomPf.Segments[1] is ArcSegment)
                            {
                                LineSegment ls_bottom = new LineSegment();
                                ls_bottom.Point = ((ArcSegment)bottomPf.Segments[1]).Point;
                                bottomPf.Segments[1] = ls_bottom;

                                LineSegment ls_bottom2 = new LineSegment();
                                ls_bottom2.Point = ((ArcSegment)bottomPf.Segments[3]).Point;
                                bottomPf.Segments[3] = ls_bottom2;
                            }
                            else if (bottomPf.Segments[1] is PolyLineSegment)
                            {
                                LineSegment ls_bottom = new LineSegment();
                                ls_bottom.Point = ((PolyLineSegment)bottomPf.Segments[1]).Points[((PolyLineSegment)bottomPf.Segments[1]).Points.Count - 1];
                                bottomPf.Segments[1] = ls_bottom;

                                LineSegment ls_bottom2 = new LineSegment();
                                ls_bottom2.Point = ((PolyLineSegment)bottomPf.Segments[3]).Points[((PolyLineSegment)bottomPf.Segments[3]).Points.Count - 1];
                                bottomPf.Segments[3] = ls_bottom2;
                            }
                        }
                    }
                }
                ColorProc.processWhenChangeLayerShape_CC(front_canvas, centralCubePanel, insertShape);
            }

            
        }


        private void changeOgive()
        {
            int index = stackpanel.Children.IndexOf(insertShape);
            ((Components)components[index]).isChangeOgive = true;
            GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
            PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
            PathFigure curPf = curPg.Figures.ElementAt(0);

            //初始化曲线
            ArcSegment as_bottom = new ArcSegment();
            if (curPf.Segments[1] is LineSegment)
            {
                as_bottom.Point = ((LineSegment)curPf.Segments[1]).Point;
            }
            else
            {
                as_bottom.Point = ((ArcSegment)curPf.Segments[1]).Point;
            }
            double width = insertShape.Width;
            as_bottom.Size = new Size(width / 2, width / 2);
            as_bottom.SweepDirection = SweepDirection.Counterclockwise;
            ArcSegment as_top = new ArcSegment();
            as_top.Point = curPf.StartPoint;
            as_top.Size = new Size(width / 2, width / 2);
            as_top.SweepDirection = SweepDirection.Counterclockwise;

            //删除直线 添加曲线
            curPf.Segments[1] = as_bottom;
            curPf.Segments[3] = as_top;
        }

        //添加层点击事件
        private void addLayerMenu_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //获取当前图形的路径
            GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
            int index = stackpanel.Children.IndexOf(insertShape);

            if (((Components)components[index]).layerNum == 0)
            {
                PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
                PathFigure curPf = curPg.Figures.ElementAt(0);
                Point startP = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + 20);
                curPf.StartPoint = startP;
                ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + 20);
                if (curPf.Segments[1] is LineSegment)
                {
                    ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + 20);
                }
                else if (curPf.Segments[1] is ArcSegment)
                {
                    ((ArcSegment)curPf.Segments[1]).Point = new Point(((ArcSegment)curPf.Segments[1]).Point.X, ((ArcSegment)curPf.Segments[1]).Point.Y + 20);
                }
                else
                {
                    for (int i = 0; i < ((PolyLineSegment)curPf.Segments[1]).Points.Count; i++)
                    {
                        ((PolyLineSegment)curPf.Segments[1]).Points[i] = new Point(((PolyLineSegment)curPf.Segments[1]).Points[i].X, ((PolyLineSegment)curPf.Segments[1]).Points[i].Y + 20);
                    }
                }
                ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + 20);
                if (curPf.Segments[3] is LineSegment)
                {
                    ((LineSegment)curPf.Segments[3]).Point = new Point(((LineSegment)curPf.Segments[3]).Point.X, ((LineSegment)curPf.Segments[3]).Point.Y + 20);
                }
                else if (curPf.Segments[3] is ArcSegment)
                {
                    ((ArcSegment)curPf.Segments[3]).Point = new Point(((ArcSegment)curPf.Segments[3]).Point.X, ((ArcSegment)curPf.Segments[3]).Point.Y + 20);
                }
                else
                {
                    for (int i = 0; i < ((PolyLineSegment)curPf.Segments[3]).Points.Count; i++)
                    {
                        ((PolyLineSegment)curPf.Segments[3]).Points[i] = new Point(((PolyLineSegment)curPf.Segments[3]).Points[i].X, ((PolyLineSegment)curPf.Segments[3]).Points[i].Y + 20);
                    }
                }

                //创建上下层路径
                PathGeometry topPg = new PathGeometry();
                PathGeometry bottomPg = new PathGeometry();

                PathFigure topPf = new PathFigure();
                PathFigure bottomPf = new PathFigure();

                //绘制上层路径
                topPf.StartPoint = curPf.StartPoint;
                if (curPf.Segments[3] is LineSegment)
                {
                    LineSegment ls_top = new LineSegment();
                    ls_top.Point = ((LineSegment)curPf.Segments[2]).Point;
                    topPf.Segments.Add(ls_top);
                }
                else if (curPf.Segments[3] is ArcSegment)
                {
                    ArcSegment ls_top = new ArcSegment();
                    ls_top.Point = ((LineSegment)curPf.Segments[2]).Point;
                    ls_top.Size = ((ArcSegment)curPf.Segments[3]).Size;
                    // add by hegc
                    if (((ArcSegment)curPf.Segments[3]).SweepDirection == SweepDirection.Clockwise)
                    {
                        ls_top.SweepDirection = SweepDirection.Counterclockwise;
                    }
                    else
                    {
                        ls_top.SweepDirection = SweepDirection.Clockwise;
                    }
                    //ls_top.SweepDirection = ((ArcSegment)curPf.Segments[3]).SweepDirection;
                    //add end

                    topPf.Segments.Add(ls_top);
                }
                else
                {
                    PolyLineSegment pls_top = new PolyLineSegment();
                    for (int i = ((PolyLineSegment)curPf.Segments[3]).Points.Count - 1; i >= 0; i--)
                    {
                        pls_top.Points.Add(((PolyLineSegment)curPf.Segments[3]).Points[i]);
                    }
                    topPf.Segments.Add(pls_top);
                }

                LineSegment ls2_top = new LineSegment();
                Point thirdPoint_top = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y - 20);
                ls2_top.Point = thirdPoint_top;
                topPf.Segments.Add(ls2_top);

                if (curPf.Segments[3] is LineSegment)
                {
                    LineSegment ls3_top = new LineSegment();
                    Point forthPoint_top = new Point(curPf.StartPoint.X, curPf.StartPoint.Y - 20);
                    ls3_top.Point = forthPoint_top;
                    topPf.Segments.Add(ls3_top);
                }
                else if (curPf.Segments[3] is ArcSegment)
                {
                    ArcSegment ls3_top = new ArcSegment();
                    Point forthPoint_top = new Point(curPf.StartPoint.X, curPf.StartPoint.Y - 20);
                    ls3_top.Point = forthPoint_top;
                    ls3_top.Size = ((ArcSegment)curPf.Segments[3]).Size;
                    //update by hegc
                    ls3_top.SweepDirection = ((ArcSegment)curPf.Segments[3]).SweepDirection;
                    //update end
                    topPf.Segments.Add(ls3_top);
                }
                else
                {
                    PolyLineSegment pls2_top = new PolyLineSegment();
                    //pls2_top.Points = ((PolyLineSegment)curPf.Segments[3]).Points;
                    for (int i = 0; i < ((PolyLineSegment)curPf.Segments[3]).Points.Count; i++)
                    {
                        pls2_top.Points.Add(new Point(((PolyLineSegment)curPf.Segments[3]).Points[i].X, ((PolyLineSegment)curPf.Segments[3]).Points[i].Y - 20));
                    }
                    topPf.Segments.Add(pls2_top);
                }

                topPg.Figures.Add(topPf);
                topPf.IsClosed = true;

                ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, insertShape, topPf, 0, (Color)ColorConverter.ConvertFromString("#FFFFA500"));


                //绘制下层路径
                bottomPf.StartPoint = ((LineSegment)curPf.Segments[0]).Point;
                LineSegment ls_bottom = new LineSegment();
                Point secondPoint_bottom = new Point(bottomPf.StartPoint.X, bottomPf.StartPoint.Y + 20);
                ls_bottom.Point = secondPoint_bottom;
                bottomPf.Segments.Add(ls_bottom);

                if (curPf.Segments[1] is LineSegment)
                {
                    LineSegment ls2_bottom = new LineSegment();
                    Point thirdPoint_bottom = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + 20);
                    ls2_bottom.Point = thirdPoint_bottom;
                    bottomPf.Segments.Add(ls2_bottom);
                }
                else if (curPf.Segments[1] is ArcSegment)
                {
                    ArcSegment ls2_bottom = new ArcSegment();
                    Point thirdPoint_bottom = new Point(((ArcSegment)curPf.Segments[1]).Point.X, ((ArcSegment)curPf.Segments[1]).Point.Y + 20);
                    ls2_bottom.Point = thirdPoint_bottom;
                    ls2_bottom.Size = ((ArcSegment)curPf.Segments[1]).Size;

                    // update by hegc
                    ls2_bottom.SweepDirection = ((ArcSegment)curPf.Segments[1]).SweepDirection;
                    // update end

                    bottomPf.Segments.Add(ls2_bottom);
                }
                else
                {
                    PolyLineSegment pls_bottom = new PolyLineSegment();
                    for (int i = 0; i < ((PolyLineSegment)curPf.Segments[1]).Points.Count; i++)
                    {
                        pls_bottom.Points.Add(new Point(((PolyLineSegment)curPf.Segments[1]).Points[i].X, ((PolyLineSegment)curPf.Segments[1]).Points[i].Y + 20));
                    }
                    bottomPf.Segments.Add(pls_bottom);
                }

                LineSegment ls3_bottom = new LineSegment();
                if (curPf.Segments[1] is LineSegment)
                {
                    Point forthPoint_bottom = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y);
                    ls3_bottom.Point = forthPoint_bottom;
                }
                else if (curPf.Segments[1] is ArcSegment)
                {
                    Point forthPoint_bottom = new Point(((ArcSegment)curPf.Segments[1]).Point.X, ((ArcSegment)curPf.Segments[1]).Point.Y);
                    ls3_bottom.Point = forthPoint_bottom;
                }
                else
                {
                    Point forthPoint_bottom = new Point(((PolyLineSegment)curPf.Segments[1]).Points[((PolyLineSegment)curPf.Segments[1]).Points.Count - 1].X, ((PolyLineSegment)curPf.Segments[1]).Points[((PolyLineSegment)curPf.Segments[1]).Points.Count - 1].Y);
                    ls3_bottom.Point = forthPoint_bottom;
                }

                bottomPf.Segments.Add(ls3_bottom);

                if (curPf.Segments[1] is LineSegment)
                {
                    LineSegment ls4_bottom = new LineSegment();
                    ls4_bottom.Point = bottomPf.StartPoint;
                    bottomPf.Segments.Add(ls4_bottom);
                }
                else if (curPf.Segments[1] is ArcSegment)
                {
                    ArcSegment ls4_bottom = new ArcSegment();
                    Point forthPoint_bottom = bottomPf.StartPoint;
                    ls4_bottom.Point = forthPoint_bottom;
                    ls4_bottom.Size = ((ArcSegment)curPf.Segments[1]).Size;
                    // update by hegc
                    if (((ArcSegment)curPf.Segments[1]).SweepDirection == SweepDirection.Clockwise)
                    {
                        ls4_bottom.SweepDirection = SweepDirection.Counterclockwise;
                    }
                    else
                    {
                        ls4_bottom.SweepDirection = SweepDirection.Clockwise;
                    }
                    //ls4_bottom.SweepDirection = SweepDirection.Counterclockwise;
                    // update end
                    bottomPf.Segments.Add(ls4_bottom);
                }
                else
                {
                    PolyLineSegment pls2_bottom = new PolyLineSegment();

                    for (int i = ((PolyLineSegment)curPf.Segments[1]).Points.Count - 1; i >= 0; i--)
                    {
                        pls2_bottom.Points.Add(((PolyLineSegment)curPf.Segments[1]).Points[i]);
                    }
                    bottomPf.Segments.Add(pls2_bottom);
                }


                bottomPg.Figures.Add(bottomPf);
                bottomPf.IsClosed = true;
                //将上下层路径添加到组中
                geometryGroup.Children.Add(topPg);
                geometryGroup.Children.Add(bottomPg);
                ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, insertShape, bottomPf, 1, (Color)ColorConverter.ConvertFromString("#FFFFA500"));

                insertShape.Height += 40;
                ((Components)components[index]).height = insertShape.Height;

            }
            //有层
            else
            {
                int i;
                PathGeometry curPg_ = (PathGeometry)geometryGroup.Children[0];
                PathFigure curPf_ = curPg_.Figures.ElementAt(0);
                Point startP_ = new Point(curPf_.StartPoint.X, curPf_.StartPoint.Y + 20);
                curPf_.StartPoint = startP_;
                ((LineSegment)curPf_.Segments[0]).Point = new Point(((LineSegment)curPf_.Segments[0]).Point.X, ((LineSegment)curPf_.Segments[0]).Point.Y + 20);
                if (curPf_.Segments[1] is LineSegment)
                {
                    ((LineSegment)curPf_.Segments[1]).Point = new Point(((LineSegment)curPf_.Segments[1]).Point.X, ((LineSegment)curPf_.Segments[1]).Point.Y + 20);
                }
                else if (curPf_.Segments[1] is ArcSegment)
                {
                    ((ArcSegment)curPf_.Segments[1]).Point = new Point(((ArcSegment)curPf_.Segments[1]).Point.X, ((ArcSegment)curPf_.Segments[1]).Point.Y + 20);
                }
                else
                {
                    for (int j = 0; j < (((PolyLineSegment)curPf_.Segments[1]).Points.Count); j++)
                    {
                        ((PolyLineSegment)curPf_.Segments[1]).Points[j] = new Point(((PolyLineSegment)curPf_.Segments[1]).Points[j].X, ((PolyLineSegment)curPf_.Segments[1]).Points[j].Y + 20);
                    }
                }
                ((LineSegment)curPf_.Segments[2]).Point = new Point(((LineSegment)curPf_.Segments[2]).Point.X, ((LineSegment)curPf_.Segments[2]).Point.Y + 20);
                if (curPf_.Segments[1] is LineSegment)
                {
                    ((LineSegment)curPf_.Segments[3]).Point = new Point(((LineSegment)curPf_.Segments[3]).Point.X, ((LineSegment)curPf_.Segments[3]).Point.Y + 20);
                }
                else if (curPf_.Segments[3] is ArcSegment)
                {
                    ((ArcSegment)curPf_.Segments[3]).Point = new Point(((ArcSegment)curPf_.Segments[3]).Point.X, ((ArcSegment)curPf_.Segments[3]).Point.Y + 20);
                }
                else
                {
                    for (int j = 0; j < (((PolyLineSegment)curPf_.Segments[3]).Points.Count); j++)
                    {
                        ((PolyLineSegment)curPf_.Segments[3]).Points[j] = new Point(((PolyLineSegment)curPf_.Segments[3]).Points[j].X, ((PolyLineSegment)curPf_.Segments[3]).Points[j].Y + 20);
                    }
                }
                //层
                for (i = 2; i <= ((Components)components[index]).layerNum; i += 2)
                {
                    PathGeometry curPg = (PathGeometry)geometryGroup.Children[i - 1];
                    PathFigure curPf = curPg.Figures.ElementAt(0);
                    Point startP = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + 20);
                    curPf.StartPoint = startP;
                    if (curPf.Segments[0] is LineSegment)
                    {
                        ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + 20);
                    }
                    else if (curPf.Segments[0] is ArcSegment)
                    {
                        ((ArcSegment)curPf.Segments[0]).Point = new Point(((ArcSegment)curPf.Segments[0]).Point.X, ((ArcSegment)curPf.Segments[0]).Point.Y + 20);
                    }
                    else
                    {
                        for (int j = 0; j < (((PolyLineSegment)curPf.Segments[0]).Points.Count); j++)
                        {
                            ((PolyLineSegment)curPf.Segments[0]).Points[j] = new Point(((PolyLineSegment)curPf.Segments[0]).Points[j].X, ((PolyLineSegment)curPf.Segments[0]).Points[j].Y + 20);
                        }
                    }
                    ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + 20);
                    if (curPf.Segments[2] is LineSegment)
                    {
                        ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + 20);
                    }
                    else if (curPf.Segments[2] is ArcSegment)
                    {
                        ((ArcSegment)curPf.Segments[2]).Point = new Point(((ArcSegment)curPf.Segments[2]).Point.X, ((ArcSegment)curPf.Segments[2]).Point.Y + 20);
                    }
                    else
                    {
                        for (int j = 0; j < (((PolyLineSegment)curPf.Segments[2]).Points.Count); j++)
                        {
                            ((PolyLineSegment)curPf.Segments[2]).Points[j] = new Point(((PolyLineSegment)curPf.Segments[2]).Points[j].X, ((PolyLineSegment)curPf.Segments[2]).Points[j].Y + 20);
                        }
                    }
                    //((LineSegment)curPf.Segments[3]).Point = new Point(((LineSegment)curPf.Segments[3]).Point.X, ((LineSegment)curPf.Segments[3]).Point.Y + 20);

                    PathGeometry curPg2 = (PathGeometry)geometryGroup.Children[i];
                    PathFigure curPf2 = curPg2.Figures.ElementAt(0);
                    Point startP2 = new Point(curPf2.StartPoint.X, curPf2.StartPoint.Y + 20);
                    curPf2.StartPoint = startP2;
                    ((LineSegment)curPf2.Segments[0]).Point = new Point(((LineSegment)curPf2.Segments[0]).Point.X, ((LineSegment)curPf2.Segments[0]).Point.Y + 20);
                    if (curPf2.Segments[1] is LineSegment)
                    {
                        ((LineSegment)curPf2.Segments[1]).Point = new Point(((LineSegment)curPf2.Segments[1]).Point.X, ((LineSegment)curPf2.Segments[1]).Point.Y + 20);
                    }
                    else if (curPf2.Segments[1] is ArcSegment)
                    {
                        ((ArcSegment)curPf2.Segments[1]).Point = new Point(((ArcSegment)curPf2.Segments[1]).Point.X, ((ArcSegment)curPf2.Segments[1]).Point.Y + 20);
                    }
                    else
                    {
                        for (int j = 0; j < (((PolyLineSegment)curPf2.Segments[1]).Points.Count); j++)
                        {
                            ((PolyLineSegment)curPf2.Segments[1]).Points[j] = new Point(((PolyLineSegment)curPf2.Segments[1]).Points[j].X, ((PolyLineSegment)curPf2.Segments[1]).Points[j].Y + 20);
                        }
                    }
                    ((LineSegment)curPf2.Segments[2]).Point = new Point(((LineSegment)curPf2.Segments[2]).Point.X, ((LineSegment)curPf2.Segments[2]).Point.Y + 20);
                    if (curPf2.Segments.Count > 3)
                    {
                        if (curPf2.Segments[3] is LineSegment)
                        {
                            ((LineSegment)curPf2.Segments[3]).Point = new Point(((LineSegment)curPf2.Segments[3]).Point.X, ((LineSegment)curPf2.Segments[3]).Point.Y + 20);
                        }
                        else if (curPf2.Segments[3] is ArcSegment)
                        {
                            ((ArcSegment)curPf2.Segments[3]).Point = new Point(((ArcSegment)curPf2.Segments[3]).Point.X, ((ArcSegment)curPf2.Segments[3]).Point.Y + 20);
                        }
                        else
                        {
                            for (int j = 0; j < (((PolyLineSegment)curPf2.Segments[3]).Points.Count); j++)
                            {
                                ((PolyLineSegment)curPf2.Segments[3]).Points[j] = new Point(((PolyLineSegment)curPf2.Segments[3]).Points[j].X, ((PolyLineSegment)curPf2.Segments[3]).Points[j].Y + 20);
                            }
                        }
                    }
                }

                i -= 2;
                if (i == ((Components)components[index]).layerNum)
                {
                    PathGeometry curPg = (PathGeometry)geometryGroup.Children[i - 1];
                    PathFigure curPf = curPg.Figures.ElementAt(0);
                    //创建上下层路径
                    PathGeometry topPg = new PathGeometry();
                    PathFigure topPf = new PathFigure();

                    //绘制上层路径
                    if (curPf.Segments[2] is LineSegment)
                    {
                        topPf.StartPoint = ((LineSegment)curPf.Segments[2]).Point;

                        LineSegment ls_top = new LineSegment();
                        ls_top.Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y);
                        topPf.Segments.Add(ls_top);

                        LineSegment ls2_top = new LineSegment();
                        Point thirdPoint_top = new Point(ls_top.Point.X, ls_top.Point.Y - 20);
                        ls2_top.Point = thirdPoint_top;
                        topPf.Segments.Add(ls2_top);

                        LineSegment ls3_top = new LineSegment();
                        Point forthPoint_top = new Point(topPf.StartPoint.X, topPf.StartPoint.Y - 20);
                        ls3_top.Point = forthPoint_top;
                        topPf.Segments.Add(ls3_top);
                    }
                    else if (curPf.Segments[2] is ArcSegment)
                    {
                        topPf.StartPoint = ((ArcSegment)curPf.Segments[2]).Point;

                        ArcSegment ls_top = new ArcSegment();
                        ls_top.Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y);
                        ls_top.Size = ((ArcSegment)curPf.Segments[2]).Size;
                        // update by hegc
                        //ls_top.SweepDirection = SweepDirection.Clockwise;
                        if (((ArcSegment)curPf.Segments[2]).SweepDirection == SweepDirection.Clockwise)
                        {
                            ls_top.SweepDirection = SweepDirection.Counterclockwise;
                        }
                        else
                        {
                            ls_top.SweepDirection = SweepDirection.Clockwise;
                        }
                        // update end
                        topPf.Segments.Add(ls_top);

                        LineSegment ls2_top = new LineSegment();
                        Point thirdPoint_top = new Point(ls_top.Point.X, ls_top.Point.Y - 20);
                        ls2_top.Point = thirdPoint_top;
                        topPf.Segments.Add(ls2_top);

                        ArcSegment ls3_top = new ArcSegment();
                        Point forthPoint_top = new Point(topPf.StartPoint.X, topPf.StartPoint.Y - 20);
                        ls3_top.Point = forthPoint_top;
                        ls3_top.Size = ((ArcSegment)curPf.Segments[2]).Size;
                        //update by hegc
                        //    ls3_top.SweepDirection = SweepDirection.Counterclockwise;
                        ls3_top.SweepDirection = ((ArcSegment)curPf.Segments[2]).SweepDirection;
                        // update end
                        topPf.Segments.Add(ls3_top);
                    }
                    else
                    {
                        topPf.StartPoint = ((PolyLineSegment)curPf.Segments[2]).Points[((PolyLineSegment)curPf.Segments[2]).Points.Count - 1];

                        PolyLineSegment pls_top = new PolyLineSegment();
                        for (int j = ((PolyLineSegment)curPf.Segments[2]).Points.Count - 1; j >= 0; j--)
                        {
                            pls_top.Points.Add(((PolyLineSegment)curPf.Segments[2]).Points[j]);
                        }
                        topPf.Segments.Add(pls_top);

                        LineSegment ls2_top = new LineSegment();
                        Point thirdPoint_top = new Point(pls_top.Points[pls_top.Points.Count - 1].X, pls_top.Points[pls_top.Points.Count - 1].Y - 20);
                        ls2_top.Point = thirdPoint_top;
                        topPf.Segments.Add(ls2_top);

                        PolyLineSegment pls2_top = new PolyLineSegment();
                        for (int j = 0; j <= ((PolyLineSegment)curPf.Segments[2]).Points.Count - 1; j++)
                        {
                            Point forthPoint_top = new Point(((PolyLineSegment)curPf.Segments[2]).Points[j].X, ((PolyLineSegment)curPf.Segments[2]).Points[j].Y - 20);
                            pls2_top.Points.Add(forthPoint_top);
                        }
                        topPf.Segments.Add(pls2_top);
                    }


                    topPg.Figures.Add(topPf);
                    topPf.IsClosed = true;

                    //将上下层路径添加到组中
                    geometryGroup.Children.Add(topPg);
                    ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, insertShape, topPf, 0, (Color)ColorConverter.ConvertFromString("#FFFFA500"));
                }
                if (i == ((Components)components[index]).layerNum)
                {
                    PathGeometry curPg2 = (PathGeometry)geometryGroup.Children[i];
                    PathFigure curPf2 = curPg2.Figures.ElementAt(0);
                    PathGeometry bottomPg = new PathGeometry();
                    PathFigure bottomPf = new PathFigure();

                    //绘制下层路径
                    bottomPf.StartPoint = ((LineSegment)curPf2.Segments[0]).Point;
                    LineSegment ls_bottom = new LineSegment();
                    Point secondPoint_bottom = new Point(bottomPf.StartPoint.X, bottomPf.StartPoint.Y + 20);
                    ls_bottom.Point = secondPoint_bottom;
                    bottomPf.Segments.Add(ls_bottom);

                    if (curPf2.Segments[1] is LineSegment)
                    {
                        LineSegment ls2_bottom = new LineSegment();
                        Point thirdPoint_bottom = new Point(((LineSegment)curPf2.Segments[1]).Point.X, ((LineSegment)curPf2.Segments[1]).Point.Y + 20);
                        ls2_bottom.Point = thirdPoint_bottom;
                        bottomPf.Segments.Add(ls2_bottom);
                    }
                    else if (curPf2.Segments[1] is ArcSegment)
                    {
                        ArcSegment ls2_bottom = new ArcSegment();
                        Point thirdPoint_bottom = new Point(((ArcSegment)curPf2.Segments[1]).Point.X, ((ArcSegment)curPf2.Segments[1]).Point.Y + 20);
                        ls2_bottom.Point = thirdPoint_bottom;
                        ls2_bottom.Size = ((ArcSegment)curPf2.Segments[1]).Size;
                        // update by hegc
                        //ls2_bottom.SweepDirection = SweepDirection.Counterclockwise;
                        ls2_bottom.SweepDirection = ((ArcSegment)curPf2.Segments[1]).SweepDirection;
                        // update end
                        bottomPf.Segments.Add(ls2_bottom);
                    }
                    else
                    {
                        PolyLineSegment pls_bottom = new PolyLineSegment();
                        for (int j = 0; j <= ((PolyLineSegment)curPf2.Segments[1]).Points.Count - 1; j++)
                        {
                            pls_bottom.Points.Add(new Point(((PolyLineSegment)curPf2.Segments[1]).Points[j].X, ((PolyLineSegment)curPf2.Segments[1]).Points[j].Y + 20));
                        }
                        bottomPf.Segments.Add(pls_bottom);
                    }

                    LineSegment ls3_bottom = new LineSegment();
                    if (curPf2.Segments[1] is LineSegment)
                    {
                        Point forthPoint_bottom = new Point(((LineSegment)curPf2.Segments[1]).Point.X, ((LineSegment)curPf2.Segments[1]).Point.Y);
                        ls3_bottom.Point = forthPoint_bottom;
                    }
                    else if (curPf2.Segments[1] is ArcSegment)
                    {
                        Point forthPoint_bottom = new Point(((ArcSegment)curPf2.Segments[1]).Point.X, ((ArcSegment)curPf2.Segments[1]).Point.Y);
                        ls3_bottom.Point = forthPoint_bottom;
                    }
                    else
                    {
                        Point forthPoint_bottom = new Point(((PolyLineSegment)curPf2.Segments[1]).Points[((PolyLineSegment)curPf2.Segments[1]).Points.Count - 1].X, ((PolyLineSegment)curPf2.Segments[1]).Points[((PolyLineSegment)curPf2.Segments[1]).Points.Count - 1].Y);
                        ls3_bottom.Point = forthPoint_bottom;
                    }
                    bottomPf.Segments.Add(ls3_bottom);

                    if (curPf2.Segments.Count > 3)
                    {
                        if (curPf2.Segments[3] is LineSegment)
                        {
                            LineSegment ls4_bottom = new LineSegment();
                            Point forthPoint_bottom2 = bottomPf.StartPoint;
                            ls4_bottom.Point = forthPoint_bottom2;

                            // update end
                            bottomPf.Segments.Add(ls4_bottom);
                        }
                        else if (curPf2.Segments[3] is ArcSegment)
                        {
                            ArcSegment ls4_bottom = new ArcSegment();
                            Point forthPoint_bottom2 = bottomPf.StartPoint;
                            ls4_bottom.Point = forthPoint_bottom2;
                            ls4_bottom.Size = ((ArcSegment)curPf2.Segments[1]).Size;
                            // update by hegc
                            if (((ArcSegment)curPf2.Segments[1]).SweepDirection == SweepDirection.Clockwise)
                            {
                                ls4_bottom.SweepDirection = SweepDirection.Counterclockwise;
                            }
                            else
                            {
                                ls4_bottom.SweepDirection = SweepDirection.Clockwise;
                            }
                            //ls4_bottom.SweepDirection = SweepDirection.Clockwise;
                            // update end
                            bottomPf.Segments.Add(ls4_bottom);
                        }
                        else
                        {
                            PolyLineSegment pls2_bottom = new PolyLineSegment();
                            for (int j = ((PolyLineSegment)curPf2.Segments[3]).Points.Count - 1; j >= 0; j--)
                            {
                                pls2_bottom.Points.Add(new Point(((PolyLineSegment)curPf2.Segments[1]).Points[j].X, ((PolyLineSegment)curPf2.Segments[1]).Points[j].Y));
                            }
                            bottomPf.Segments.Add(pls2_bottom);
                        }
                    }

                    bottomPg.Figures.Add(bottomPf);

                    ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, insertShape, bottomPf, 1, (Color)ColorConverter.ConvertFromString("#FFFFA500"));

                    //将上下层路径添加到组中
                    geometryGroup.Children.Add(bottomPg);
                }
                insertShape.Height += 40;
                ((Components)components[index]).height = insertShape.Height;
            }
            ((Components)components[index]).layerNum += 2;
            ((Components)components[index]).layerNums.Add(((Components)components[index]).layerNum - 1);
            ((Components)components[index]).layerNums.Add(((Components)components[index]).layerNum);
            ((Components)components[index]).layerType.Add("结构");
            ((Components)components[index]).layerType.Add("结构");
            ((Components)components[index]).layerMaterial.Add("空气");
            ((Components)components[index]).layerMaterial.Add("空气");
            ((Components)components[index]).layerSize.Add(((Components)components[index]).layerNum - 1, new Hashtable());
            ((Components)components[index]).layerSize.Add(((Components)components[index]).layerNum, new Hashtable());
            ((Hashtable)(((Components)components[index]).layerSize[((Components)components[index]).layerNum - 1])).Add("diameter", 20);
            ((Hashtable)(((Components)components[index]).layerSize[((Components)components[index]).layerNum - 1])).Add("longLength", 0);
            ((Hashtable)(((Components)components[index]).layerSize[((Components)components[index]).layerNum - 1])).Add("width", 0);
            ((Hashtable)(((Components)components[index]).layerSize[((Components)components[index]).layerNum - 1])).Add("height", 0);
            ((Hashtable)(((Components)components[index]).layerSize[((Components)components[index]).layerNum - 1])).Add("ObliqueAngle", 0);
            ((Hashtable)(((Components)components[index]).layerSize[((Components)components[index]).layerNum])).Add("diameter", 0);
            ((Hashtable)(((Components)components[index]).layerSize[((Components)components[index]).layerNum])).Add("longLength", 0);
            ((Hashtable)(((Components)components[index]).layerSize[((Components)components[index]).layerNum])).Add("width", 0);
            ((Hashtable)(((Components)components[index]).layerSize[((Components)components[index]).layerNum])).Add("height", 0);
            ((Hashtable)(((Components)components[index]).layerSize[((Components)components[index]).layerNum])).Add("ObliqueAngle", 0);
            ((Components)components[index]).layerLeftThickness.Add(20);
            ((Components)components[index]).layerLeftThickness.Add(20);
            ((Components)components[index]).layerRightThickness.Add(20);
            ((Components)components[index]).layerRightThickness.Add(20);

            makeLeftCoverConnectSkillfully();
            makeRightCoverConnectSkillfully();
            autoResize();
            //}
            //catch
            //{
            //MessageBox.Show("参数异常");
            // }
        }

        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            changeTitle = 0;
            editWindow ew = new editWindow();
            ew.list = DataBaseMaterials;
            ew.changeTitle = changeTitle;

            double leftWidth = 0;
            for (int i = 0; i < stackpanel.Children.IndexOf(insertShape); i++)
            {
                leftWidth += ((System.Windows.Shapes.Path)stackpanel.Children[i]).Width;
            }
            ew.left_X = leftWidth;
            ew.setComponent(currentComp);
            int index = components.IndexOf(currentComp);
            if (components.Count - 1 > index)
            {
                ew.rightCom = (Components)components[index + 1];
            }
            if (index > 0)
            {
                ew.leftCom = (Components)components[index - 1];
            }

            if (index == 0 && isHaveLeftEndCap)
            {
                ew.isLeftEndCap = Colors.Blue.ToString();
            }
            if (index == components.Count - 1 && isHaveRightEndCap)
            {
                ew.isRightEndCap = Colors.Green.ToString();
            }

            ew.Owner = this;
            ew.canvas = this.canvas;
            ew.sp = this.stackpanel;
            ew.ChangeTextEvent += new ChangeTextHandler(autoResize);
            //ew.ChangeCoverEvent += new ChangeCoverLocation(ColorProc.processWhenMoveLayer);
            ew.ChangeShapeEvent += new ChangeShapeHandler(changeLineSegmentToArcSegment);
            ew.ChangeShapeEvent2 += new ChangeShapeHandler(changeLineSegmentToArcSegment);
            ew.ChangeShapeEvent3 += new ChangeShapeHandler(changeArcSegmentToLineSegment);
            ew.Show();
        }

        //编辑空心管
        private void btEdit_CentralCube_Click(object sender, RoutedEventArgs e)
        {
            changeTitle = 2;
            editWindow ew = new editWindow();
            ew.list = DataBaseMaterials;
            ew.changeTitle = changeTitle;

            double leftWidth = 0;
            for (int i = 0; i < centralCubePanel.Children.IndexOf(insertShape); i++)
            {
                leftWidth += ((System.Windows.Shapes.Path)centralCubePanel.Children[i]).Width;
            }
            ew.left_X = leftWidth;
            ew.setComponent(currentComp);
            int index = centralCubes.IndexOf(currentComp);
            if (centralCubes.Count - 1 > index)
            {
                ew.rightCom = (Components)centralCubes[index + 1];
            }
            if (index > 0)
            {
                ew.leftCom = (Components)centralCubes[index - 1];
            }

            ew.Owner = this;
            ew.canvas = this.canvas;
            ew.sp = this.centralCubePanel;
            ew.ChangeTextEvent += new ChangeTextHandler(autoResize);
            //ew.ChangeCoverEvent += new ChangeCoverLocation(ColorProc.processWhenMoveLayer);
            ew.ChangeShapeEvent += new ChangeShapeHandler(changeLineSegmentToArcSegment);
            ew.ChangeShapeEvent2 += new ChangeShapeHandler(changeLineSegmentToArcSegment);
            ew.ChangeShapeEvent3 += new ChangeShapeHandler(changeArcSegmentToLineSegment);
            ew.Show();
        }


        //删除MenuItem点击事件
        private void delMenu_Click(object sender, RoutedEventArgs e)
        {
            int index = stackpanel.Children.IndexOf(insertShape);

            //判断左右两边是否连接
            if (index == 1 && index == stackpanel.Children.Count - 1 && isHaveLeftEndCap)
            {
                //删除最左边或最右边的那个，不需要判断
                MessageBox.Show("不能删除最后一个段！如果非要删除，请新建项目！", "警告");
                e.Handled = true;
                return;
            }
            if (index == 0 && stackpanel.Children.Count - 1 == 1 && isHaveRightEndCap) {
                MessageBox.Show("不能删除最后一个段！如果非要删除，请新建项目！", "警告");
                e.Handled = true;
                return;
            }
            if (index == 1 && isHaveRightEndCap && isHaveLeftEndCap && stackpanel.Children.Count == 3) {
                MessageBox.Show("不能删除最后一个段！如果非要删除，请新建项目！", "警告");
                e.Handled = true;
                return;
            }

            if(index != 0 && index + 1 <= stackpanel.Children.Count - 1)
            {
                //删除中间的，需要判断左右是否连续
                System.Windows.Shapes.Path leftPath = (System.Windows.Shapes.Path)stackpanel.Children[index - 1];
                System.Windows.Shapes.Path rightPath = (System.Windows.Shapes.Path)stackpanel.Children[index + 1];

                if (index == 1 && (isHaveRightEndCap|| isHaveLeftEndCap) && stackpanel.Children.Count < 3)
                {
                    MessageBox.Show("不能删除最后一个段！如果非要删除，请新建项目！", "警告");
                    e.Handled = true;
                    return;
                }
                

                GeometryGroup geometryGroup = (GeometryGroup)leftPath.Data;
                PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
                PathFigure curPf = curPg.Figures.ElementAt(0);


                double topR;
                double bottomR;
                double height;
                if (curPf.Segments[1] is LineSegment)
                {
                    //获取left的右侧高度
                    topR = ((LineSegment)curPf.Segments[1]).Point.Y;
                    bottomR = ((LineSegment)curPf.Segments[2]).Point.Y;

                    height = Math.Abs(topR - bottomR);
                    
                    
                    //if(isha)
                }
                else if (curPf.Segments[1] is ArcSegment)
                {
                    //获取left的右侧高度
                    topR = ((ArcSegment)curPf.Segments[1]).Point.Y;
                    bottomR = ((LineSegment)curPf.Segments[2]).Point.Y;
                    height = Math.Abs(topR - bottomR); 
                }else
                {
                    //获取left的右侧高度
                    topR = ((PolyLineSegment)curPf.Segments[1]).Points[((PolyLineSegment)curPf.Segments[1]).Points.Count - 1].Y;
                    bottomR = ((LineSegment)curPf.Segments[2]).Point.Y;
                    height = Math.Abs(topR - bottomR);
                }

                editWindow ew = new editWindow();
                ew.setComponent((Components)components[index + 1]);
                ew.Owner = this;
                ew.color = (Color)ColorConverter.ConvertFromString(((Components)components[index + 1]).newPath.Fill.ToString());
                if (isHaveRightEndCap && index == stackpanel.Children.Count - 2)
                {
                    ew.isLeftEndCap = "#FF0000FF";
                    ew.isRightEndCap = "#FF008000";
                }
                ew.leftD.Text = height.ToString();
                ew.rightD.Text = rightPath.Data.Bounds.Right.ToString();
                if (index + 1 == stackpanel.Children.Count - 1 && isHaveRightEndCap) {
                    ew.rightD.Text = rightPath.Data.Bounds.Height.ToString();
                }
                ew.radiusText.Text = ((Components)components[index + 1]).radius.ToString();
                ew.ChangeTextEvent += new ChangeTextHandler(autoResize);
                ew.canvas = this.canvas;
                ew.sp = this.stackpanel;
                //ew.ChangeCoverEvent += new ChangeCoverLocation(ColorProc.processWhenMoveLayer);
                ew.ChangeShapeEvent += new ChangeShapeHandler(changeLineSegmentToArcSegment);
                ew.ChangeShapeEvent2 += new ChangeShapeHandler(changeLineSegmentToArcSegment);
                ew.ChangeShapeEvent3 += new ChangeShapeHandler(changeArcSegmentToLineSegment);
                System.Windows.Shapes.Path originalInsertShape = getInsertShape();
                int originalCurLayerNum = getCurLayerNum();
                setInsertShape(((Components)components[index + 1]).newPath);
                setCurLayerNum(((Components)components[index + 1]).layerNum);
                ew.OKButton_Click(null, null);
                setInsertShape(originalInsertShape);
                setCurLayerNum(originalCurLayerNum);
            }
           
            if (index == 0 && isHaveLeftEndCap)
            {
                isHaveLeftEndCap = false;
                ContextMenu c = this.canvas.ContextMenu;
                MenuItem mi = c.Items[1] as MenuItem;
                MenuItem addLeftCoverMenu = mi.Items[0] as MenuItem;
                addLeftCoverMenu.IsEnabled = true;
            }

            if (index == stackpanel.Children.Count - 1 && isHaveRightEndCap)
            {
                isHaveRightEndCap = false;
                ContextMenu c = this.canvas.ContextMenu;
                MenuItem mi = c.Items[1] as MenuItem;
                MenuItem addRightCoverMenu = mi.Items[1] as MenuItem;
                addRightCoverMenu.IsEnabled = true;
            }

            components.RemoveAt(index);
            //删除浮层
            ColorProc.processWhenDelCylindrical(this.canvas, insertShape);

            stackpanel.Children.Remove(insertShape);

            makeLeftCoverConnectSkillfully();
            makeRightCoverConnectSkillfully();
            autoResize();

            //移动浮层
            //ColorProc.processWhenMoveLayer(this.canvas, this.stackpanel);
            //Console.WriteLine(stackpanel.Children.Capacity);
            //if (stackpanel.Children.Count == 0)
            //{
            //    NewProgramButton_Click(sender,e);
            //}
        }

        private bool mouseDown;
        private Point mouseXY;
        private Point imageXY;
        private void Img1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Shapes.Path)
            {
                insertShape = sender as System.Windows.Shapes.Path;
                insertShape.Stroke = Brushes.Red;
                insertShape.StrokeThickness = 2;
                int index = stackpanel.Children.IndexOf(insertShape);
                Console.WriteLine(index);
                currentComp = (Components)components[index];
                curLayerNum = currentComp.layerNum;

                for (int i = 0; i < stackpanel.Children.Count; i++)
                {
                    if (i != index)
                    {
                        System.Windows.Shapes.Path path = stackpanel.Children[i] as System.Windows.Shapes.Path;
                        path.Stroke = Brushes.Blue;
                    }
                }

                for (int i = 0; i < centralCubePanel.Children.Count; i++)
                {
                    System.Windows.Shapes.Path path = centralCubePanel.Children[i] as System.Windows.Shapes.Path;
                    path.Stroke = Brushes.Blue;
                }

                insertShape.CaptureMouse();
            }
            else
            {
                currentShape = sender as Shape;
                currentShape.CaptureMouse();
            }

            changeTitle = 0;
            mouseDown = true;
            mouseXY = e.GetPosition(canvas);               //得到当前鼠标在窗口的位置
            if (e.RightButton == MouseButtonState.Pressed)
            {
                return;
            }

            if (e.ClickCount == 2)
            {
                Console.WriteLine("双击，调用编辑界面！");

                bool flag = false;

                //是否存在层  调用层的编辑界面
                GeometryGroup geometryGroup = (GeometryGroup)insertShape.Data;
                int index = stackpanel.Children.IndexOf(insertShape);
                if (sender is System.Windows.Shapes.Path)
                {
                    currentComp = (Components)components[index];

                    changeTitle = 0;

                    if ((isHaveLeftEndCap && index == 0) || (isHaveRightEndCap && index == components.Count - 1))
                    {
                        changeTitle = 1;
                    }

                    if (changeTitle == 0 || changeTitle == 1)
                        btEdit_Click(sender, e);
                }
                else
                {

                }
                /*try
                {
                    if (geometryGroup.Children.Count > 1)
                    {

                        Components cps = (Components)components[index];
                        Console.Write(e.GetPosition(insertShape));

                        for (int i = 2; i <= ((Components)components[index]).layerNum; i += 2)
                        {
                            curLayerNum = i - 1;
                            PathGeometry curPg = (PathGeometry)geometryGroup.Children[i - 1];
                            PathFigure curPf = curPg.Figures.ElementAt(0);
                            Point startP = curPf.StartPoint;
                            Point p0 = ((LineSegment)curPf.Segments[0]).Point;
                            Point p1 = ((LineSegment)curPf.Segments[1]).Point;
                            Point p2 = ((LineSegment)curPf.Segments[2]).Point;

                            Point curP = e.GetPosition(insertShape);
                            double pwidth = p0.X - startP.X;
                            // 直线、平衡层
                            if (startP.Y == p0.Y && p1.Y == p2.Y)
                            {
                                if (curP.Y < startP.Y && curP.Y > p2.Y)
                                {
                                    editLayerMenu_Click(sender, e);
                                    flag = true;
                                }
                            }
                            // 直线
                            if (startP.Y == p0.Y && p1.Y != p2.Y)
                            {
                                if (p2.Y < p1.Y)
                                {
                                    double temp = p1.Y - p2.Y;

                                    if (curP.Y < startP.Y && curP.Y > p2.Y + curP.X / pwidth * temp)
                                    {
                                        editLayerMenu_Click(sender, e);
                                        flag = true;
                                    }
                                }

                                if (p2.Y > p1.Y)
                                {
                                    double temp = p2.Y - p1.Y;

                                    if (curP.Y < startP.Y && curP.Y > p2.Y - curP.X / pwidth * temp)
                                    {
                                        editLayerMenu_Click(sender, e);
                                        flag = true;
                                    }
                                }
                            }
                            //
                            if (startP.Y != p0.Y && p1.Y == p2.Y)
                            {
                                if (startP.Y < p0.Y)
                                {
                                    double temp = p0.Y - startP.Y;

                                    if (curP.Y < startP.Y + curP.X / pwidth * temp && curP.Y > p2.Y)
                                    {
                                        editLayerMenu_Click(sender, e);
                                        flag = true;
                                    }
                                }

                                if (startP.Y > p0.Y)
                                {
                                    double temp = startP.Y - p0.Y;

                                    if (curP.Y < startP.Y - curP.X / pwidth * temp && curP.Y > p2.Y)
                                    {
                                        editLayerMenu_Click(sender, e);
                                        flag = true;
                                    }
                                }
                            }
                            //
                            if (startP.Y != p0.Y && p1.Y != p2.Y)
                            {
                                if (startP.Y - p2.Y == p0.Y - p1.Y)
                                {
                                    if (startP.Y < p0.Y)
                                    {
                                        double temp = p0.Y - startP.Y;

                                        if (curP.Y < startP.Y + curP.X / pwidth * temp && curP.Y > p2.Y + curP.X / pwidth * temp)
                                        {
                                            editLayerMenu_Click(sender, e);
                                            flag = true;
                                        }
                                    }

                                    if (startP.Y > p0.Y)
                                    {
                                        double temp = startP.Y - p0.Y;

                                        if (curP.Y < startP.Y - curP.X / pwidth * temp && curP.Y > p2.Y - curP.X / pwidth * temp)
                                        {
                                            editLayerMenu_Click(sender, e);
                                            flag = true;
                                        }
                                    }
                                }
                                else
                                {
                                    double top = p2.Y - p1.Y;
                                    double bottom = startP.Y - p0.Y;

                                    if (curP.Y < startP.Y - curP.X / pwidth * bottom && curP.Y > p2.Y - curP.X / pwidth * top)
                                    {
                                        editLayerMenu_Click(sender, e);
                                        flag = true;
                                    }
                                }
                            }
                            curLayerNum = i;
                            PathGeometry curPg2 = (PathGeometry)geometryGroup.Children[i];
                            PathFigure curPf2 = curPg2.Figures.ElementAt(0);
                            Point startP2 = curPf2.StartPoint;
                            Point p2_0 = ((LineSegment)curPf2.Segments[0]).Point;
                            Point p2_1 = ((LineSegment)curPf2.Segments[1]).Point;
                            Point p2_2 = ((LineSegment)curPf2.Segments[2]).Point;

                            if (startP2.Y == p2_2.Y && p2_1.Y == p2_0.Y)
                            {
                                if (curP.Y > startP2.Y && curP.Y < p2_0.Y)
                                {
                                    editLayerMenu_Click(sender, e);
                                    flag = true;
                                }
                            }
                            //
                            if (startP2.Y == p2_2.Y && p2_1.Y != p2_0.Y)
                            {
                                if (p2_1.Y < p2_0.Y)
                                {
                                    double temp = p2_0.Y - p2_1.Y;

                                    if (curP.Y > startP2.Y && curP.Y < p2_0.Y - curP.X / pwidth * temp)
                                    {
                                        editLayerMenu_Click(sender, e);
                                        flag = true;
                                    }
                                }

                                if (p2_1.Y > p2_0.Y)
                                {
                                    double temp = p2_1.Y - p2_0.Y;

                                    if (curP.Y > startP2.Y && curP.Y < p2_0.Y + curP.X / pwidth * temp)
                                    {
                                        editLayerMenu_Click(sender, e);
                                        flag = true;
                                    }
                                }
                            }
                            //
                            if (startP2.Y != p2_2.Y && p2_1.Y == p2_0.Y)
                            {
                                if (startP2.Y < p2_2.Y)
                                {
                                    double temp = p2_2.Y - startP2.Y;

                                    if (curP.Y > startP2.Y + curP.X / pwidth * temp && curP.Y < p2_0.Y)
                                    {
                                        editLayerMenu_Click(sender, e);
                                        flag = true;
                                    }
                                }

                                if (startP2.Y > p2_2.Y)
                                {
                                    double temp = startP2.Y - p2_2.Y;

                                    if (curP.Y < startP2.Y - curP.X / pwidth * temp && curP.Y > p2_0.Y)
                                    {
                                        editLayerMenu_Click(sender, e);
                                        flag = true;
                                    }
                                }
                            }
                            //
                            if (startP2.Y != p2_2.Y && p2_1.Y != p2_0.Y)
                            {
                                if (p2_0.Y - startP2.Y == p2_1.Y - p2_2.Y)
                                {
                                    if (startP2.Y < p2_2.Y)
                                    {
                                        double temp = p2_2.Y - startP2.Y;

                                        if (curP.Y > startP2.Y + curP.X / pwidth * temp && curP.Y < p2_0.Y + curP.X / pwidth * temp)
                                        {
                                            editLayerMenu_Click(sender, e);
                                            flag = true;
                                        }
                                    }

                                    if (startP2.Y > p2_2.Y)
                                    {
                                        double temp = startP2.Y - p2_2.Y;

                                        if (curP.Y > startP2.Y - curP.X / pwidth * temp && curP.Y < p2_0.Y - curP.X / pwidth * temp)
                                        {
                                            editLayerMenu_Click(sender, e);
                                            flag = true;
                                        }
                                    }
                                }
                                else
                                {
                                    double top = startP2.Y - p2_2.Y;
                                    double bottom = p2_0.Y - p2_1.Y;

                                    if (curP.Y > startP2.Y - curP.X / pwidth * bottom && curP.Y < p2_0.Y - curP.X / pwidth * top)
                                    {
                                        editLayerMenu_Click(sender, e);
                                        flag = true;
                                    }
                                }
                            }
                        }
                    }


                    if (flag == false)
                    {
                        btEdit_Click(sender, e);
                    }
                }
                catch
                {
                    //MessageBox.Show("范围有误！", "警告");
                }*/
            }
        }

        private void CentralCube_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Shapes.Path)
            {
                insertShape = sender as System.Windows.Shapes.Path;
                int index = centralCubePanel.Children.IndexOf(insertShape);
                insertShape.Stroke = Brushes.Red;
                insertShape.StrokeThickness = 2;
                Console.WriteLine(index);
                currentComp = (Components)centralCubes[index];
                curLayerNum = currentComp.layerNum;

                for (int i = 0; i < centralCubePanel.Children.Count; i++)
                {
                    if (i != index)
                    {
                        System.Windows.Shapes.Path path = centralCubePanel.Children[i] as System.Windows.Shapes.Path;
                        path.Stroke = Brushes.Blue;
                    }
                }

                for (int i = 0; i < stackpanel.Children.Count; i++)
                {
                    System.Windows.Shapes.Path path = stackpanel.Children[i] as System.Windows.Shapes.Path;
                    path.Stroke = Brushes.Blue;
                }

                insertShape.CaptureMouse();
            }
            else
            {
                currentShape = sender as Shape;
                currentShape.CaptureMouse();
            }

            mouseDown = true;
            mouseXY = e.GetPosition(canvas);               //得到当前鼠标在窗口的位置

            if (e.ClickCount == 2)
            {
                Console.WriteLine("双击，调用编辑界面！");
                changeTitle = 2;
                btEdit_CentralCube_Click(sender, e);
            }
        }

        private void Img1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Shapes.Path)
            {
                insertShape = sender as System.Windows.Shapes.Path;
                insertShape.ReleaseMouseCapture();                   //鼠标左键释放时，释放鼠标
            }
            else
            {
                currentShape = sender as Shape;
                currentShape.ReleaseMouseCapture();
            }

            mouseDown = false;
        }

        //private void Img1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    insertShape = sender as Shape;
        //    imageXY = e.GetPosition(canvas);             //鼠标移动时，获取当前鼠标的位置
        //    Thickness margin_now = insertShape.Margin;          //此时图片还未移动，得到当前图片在窗体的位置。margin有四个属性，依次为Left,Top,Right,Bottom
        //    if (mouseDown)
        //    {
        //        double x = imageXY.X - mouseXY.X;        //计算出鼠标移动后，两次鼠标位置的差值
        //        double y = imageXY.Y - mouseXY.Y;
        //        //insertShape.Margin = new Thickness(margin_now.Left + x, margin_now.Top + y, margin_now.Right - x, margin_now.Bottom - y);//把鼠标位置的差值，转移到图片的位置参数上
        //        //Canvas.SetTop(insertShape, Canvas.GetTop(insertShape) + y);
        //        //Canvas.SetLeft(insertShape, Canvas.GetLeft(insertShape) + x);
        //        //pointX.Text = Convert.ToString(Canvas.GetLeft(insertShape));
        //        //pointY.Text = Convert.ToString(Canvas.GetTop(insertShape));
        //    }
        //    mouseXY = e.GetPosition(canvas);              //移动图片完毕后，再次获取鼠标的当前位置，以进行下次移动。不加这句，图片会飞掉，可以试试。
        //}

        //private Shape CreateShape()
        //{
        //    if (insertShape is Rectangle)
        //        return new Rectangle() { Fill = color, Stroke = Brushes.Black, StrokeThickness = thickness };
        //    else if (insertShape is Ellipse)
        //        return new Ellipse() { Fill = color, Stroke = Brushes.Black, StrokeThickness = thickness };
        //    else
        //        return new Line() { Stroke = (color.Color == Brushes.Transparent.Color ? Brushes.Black : color), StrokeThickness = thickness };
        //}

        private void left_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            //屏蔽非法按键，只能输入整数
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        public void autoResize()
        {
            //获取图形当前位置
            //stackpanel中有多少element
            ColorProc.mainCanvas = front_canvas;
            for (int i = 0; i < stackpanel.Children.Count; i++)
            {
                if (stackpanel.Children[i] is Image)
                {
                    Image shape = stackpanel.Children[i] as Image;
                    Console.WriteLine(shape.Width);
                    allWidth += shape.Width;
                    if (shape.Height > maxHeight)
                    {
                        maxHeight = shape.Height;
                    }
                }
                else if (stackpanel.Children[i] is System.Windows.Shapes.Path)
                {
                    System.Windows.Shapes.Path shape = stackpanel.Children[i] as System.Windows.Shapes.Path;
                    Console.WriteLine(shape.Width);
                    allWidth += shape.Width;
                    if (shape.Height > maxHeight)
                    {
                        maxHeight = shape.Height;
                    }
                }
            }
            stackpanel.Width = allWidth;
            stackpanel.Height = maxHeight;
            currentX = (canvas.Width - allWidth) / 2;
            currentY = (canvas.Height - maxHeight) / 2;
            startPosition = new Point(currentX, currentY);
            Canvas.SetLeft(stackpanel, startPosition.X);
            Canvas.SetTop(stackpanel, startPosition.Y);

            allWidth = 0;
            maxHeight = 0;

            //更新起爆点位置
            updateExplosionPosition();
            updateCubePosition();

            //移动浮层
            ColorProc.processWhenMoveLayer(canvas, stackpanel);
            ColorProc.processWhenMoveLayer_CC(canvas, centralCubePanel);
            showTotalSizeOnCanvas();
        }

        private void storeMaterialToFile(string path)
        {
            string[] file = path.Split('.');
            
            //写入文件.
            string filePath = file[0] + "_MaterialData.txt";
            FileStream myFs = new FileStream(filePath, FileMode.Create);
            

            if (DataBaseMaterials != null && DataBaseMaterials.Count != 0)
            {
                byte[] bt = MainWindow.Serializer(DataBaseMaterials);

                BinaryWriter bw = new BinaryWriter(myFs);
                bw.Write(bt);
                bw.Close();
            }
            else
            {
                byte[] bt = new byte[] { 0x01};
                BinaryWriter bw = new BinaryWriter(myFs);
                bw.Write(bt);
                bw.Close();
            }

            myFs.Close();
        }

        private List<Dictionary<string, Dictionary<string, string>>> getMaterialFromFile(string path)
        {
            string[] file = path.Split('.');
            string filePath = file[0] + "_MaterialData.txt";

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[] 
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();

            byte[] bt = new byte[] { 0x01};
            if (bytes.Length == bt.Length)
                return null;

            List<Dictionary<string, Dictionary<string, string>>> result = (List<Dictionary<string, Dictionary<string, string>>>)MainWindow.Deserialize(bytes);

            return result;
        }

        private void Store_Click(object sender, RoutedEventArgs e)
        {
            if (saveFileName == null || saveFileName == "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = ".zr";
                saveFileDialog.Filter = "Text documents (.zr)|*.zr";
                if (saveFileDialog.ShowDialog() == true)
                {
                    //File.WriteAllText(saveFileDialog.FileName, "");
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(saveFileDialog.FileName))
                    {
                        //保存数据库中材料
                        storeMaterialToFile(saveFileDialog.FileName);

                        int num = 0;
                        foreach (Components c in components)
                        {
                            if (isHaveLeftEndCap && num == 0)
                            {
                                num++;
                                continue;
                            }
                            if (isHaveRightEndCap && num == components.Count - 1)
                            {
                                continue;
                            }

                            //段相关数据
                            file.Write("段"); file.WriteLine();
                            //段大小  4个点坐标
                            file.Write(c.startPoint.X); file.Write(","); file.Write(c.startPoint.Y); file.Write('|');
                            file.Write(c.point2.X); file.Write(","); file.Write(c.point2.Y); file.Write('|');
                            file.Write(c.point3.X); file.Write(","); file.Write(c.point3.Y); file.Write('|');
                            file.Write(c.point4.X); file.Write(","); file.Write(c.point4.Y);
                            if (c.isChangeOgive)
                            {
                                file.Write('|');file.Write(c.radius); file.Write(","); file.Write(0);
                            }
                            if (c.isChangeIOgive)
                            {
                                file.Write('|');file.Write(c.radius); file.Write(","); file.Write(1);
                            }
                            file.WriteLine();
                            //段材料  对应颜色
                            file.Write(c.newPath.Fill.ToString()); file.WriteLine();
                            //层相关数据
                            GeometryGroup geometryGroup = (GeometryGroup)c.newPath.Data;

                            if (geometryGroup.Children.Count != 1)
                            {
                                file.Write("层 "); file.Write(geometryGroup.Children.Count - 1); file.WriteLine();

                                for (int i = 1; i < geometryGroup.Children.Count; i++)
                                {
                                    //层号
                                    file.Write(c.layerNums[i - 1]); file.Write('|');
                                    //层的类型
                                    file.Write(c.layerType[i - 1]); file.Write('|');
                                    //层的材料
                                    file.Write(c.layerMaterial[i - 1]); file.Write('|');
                                    //层的size
                                    file.Write(((Hashtable)(c.layerSize[i]))["diameter"]); file.Write('|');
                                    file.Write(((Hashtable)(c.layerSize[i]))["longLength"]); file.Write('|');
                                    file.Write(((Hashtable)(c.layerSize[i]))["width"]); file.Write('|');
                                    file.Write(((Hashtable)(c.layerSize[i]))["height"]); file.Write('|');
                                    file.Write(((Hashtable)(c.layerSize[i]))["ObliqueAngle"]); file.Write('|');
                                    //层的左厚度
                                    file.Write(c.layerLeftThickness[i - 1]); file.Write('|');
                                    //层的右厚度
                                    file.Write(c.layerRightThickness[i - 1]);
                                    file.WriteLine();
                                }
                            }
                            else
                            {
                                file.Write("无"); file.WriteLine();
                            }

                            num++;
                        }

                        //端盖相关数据
                        file.Write("左端盖"); file.WriteLine();
                        if (isHaveLeftEndCap)
                        {
                            Components c = (Components)components[0];
                            file.Write(c.startPoint.X); file.Write(","); file.Write(c.startPoint.Y); file.Write('|');
                            file.Write(c.point2.X); file.Write(","); file.Write(c.point2.Y); file.Write('|');
                            file.Write(c.point3.X); file.Write(","); file.Write(c.point3.Y); file.Write('|');
                            file.Write(c.point4.X); file.Write(","); file.Write(c.point4.Y);
                            file.WriteLine();
                            //端盖的材料
                            file.Write(c.newPath.Fill.ToString());
                            file.WriteLine();
                        }
                        else
                        {
                            file.Write("无");
                            file.WriteLine();
                        }

                        file.Write("右端盖"); file.WriteLine();
                        if (isHaveRightEndCap)
                        {
                            Components c = (Components)components[components.Count - 1];
                            file.Write(c.startPoint.X); file.Write(","); file.Write(c.startPoint.Y); file.Write('|');
                            file.Write(c.point2.X); file.Write(","); file.Write(c.point2.Y); file.Write('|');
                            file.Write(c.point3.X); file.Write(","); file.Write(c.point3.Y); file.Write('|');
                            file.Write(c.point4.X); file.Write(","); file.Write(c.point4.Y);
                            file.WriteLine();
                            //端盖的材料
                            file.Write(c.newPath.Fill.ToString());
                            file.WriteLine();
                        }
                        else
                        {
                            file.Write("无");
                            file.WriteLine();
                        }

                        //起爆点相关数据
                        file.Write("起爆点 "); file.Write(explosions.Count); file.WriteLine();
                        if (explosions.Count != 0)
                        {
                            //位置  大小  颜色
                            for (int i = 0; i < explosions.Count; i++)
                            {
                                if (explosions[i] is Ellipse)
                                {
                                    Ellipse el = (Ellipse)explosions[i];
                                    file.Write(Canvas.GetLeft(stackpanel)); file.Write('|');
                                    file.Write(Canvas.GetTop(stackpanel) + stackpanel.Height / 2); file.Write('|');
                                    file.Write(el.Width); file.Write('|');
                                    file.Write(el.Height); file.Write('|');
                                    file.Write(((Hashtable)(explosionData[i + 1]))["type"]); file.Write('|');
                                    file.Write(((Hashtable)(explosionData[i + 1]))["startPosition"]); file.Write('|');
                                    file.Write(((Hashtable)(explosionData[i + 1]))["radius"]); file.Write('|');
                                    file.Write(((Hashtable)(explosionData[i + 1]))["pointNums"]); file.Write('|');
                                    file.Write(el.Fill.ToString());
                                    file.WriteLine();
                                }
                            }
                        }
                        else
                        {
                            file.Write("无");
                            file.WriteLine();
                        }

                        //空心管相关数据
                        file.Write("空心管"); file.WriteLine();
                        if (isHaveCentralTube)
                        {
                            //位置  大小  颜色
                            for (int i = 0; i < centralCubePanel.Children.Count; i++)
                            {
                                if (i != 0)
                                {
                                    file.Write("空心管"); file.WriteLine();
                                }
                                if (centralCubePanel.Children[i] is System.Windows.Shapes.Path)
                                {
                                    Components c = (Components)centralCubes[i];
                                    System.Windows.Shapes.Path rect = centralCubePanel.Children[i] as System.Windows.Shapes.Path;
                                    file.Write(c.startPoint.X); file.Write(","); file.Write(c.startPoint.Y); file.Write('|');
                                    file.Write(c.point2.X); file.Write(","); file.Write(c.point2.Y); file.Write('|');
                                    file.Write(c.point3.X); file.Write(","); file.Write(c.point3.Y); file.Write('|');
                                    file.Write(c.point4.X); file.Write(","); file.Write(c.point4.Y);
                                    file.Write('|'); file.Write(c.cubeOffset); file.Write(","); file.Write(0);
                                    if (c.isChangeOgive)
                                    {
                                        file.Write('|'); file.Write(1); file.Write(","); file.Write(1); file.Write('|');
                                        file.Write(c.radius); file.Write(","); file.Write(0);
                                    }
                                    if (c.isChangeIOgive)
                                    {
                                        file.Write('|'); file.Write(2); file.Write(","); file.Write(2); file.Write('|');
                                        file.Write(c.radius); file.Write(","); file.Write(1);
                                    }
                                    
                                    file.WriteLine();


                                    GeometryGroup geometryGroup = (GeometryGroup)c.newPath.Data;

                                    if (geometryGroup.Children.Count != 1)
                                    {
                                        file.Write("空心层 "); file.Write(geometryGroup.Children.Count - 1); file.WriteLine();

                                        for (int j = 1; j < geometryGroup.Children.Count; j++)
                                        {
                                            //层号
                                            file.Write(c.layerNums[j - 1]); file.Write('|');
                                            //层的类型
                                            file.Write(c.layerType[j - 1]); file.Write('|');
                                            //层的材料
                                            file.Write(c.layerMaterial[j - 1]); file.Write('|');
                                            //层的size
                                            file.Write(((Hashtable)(c.layerSize[j]))["diameter"]); file.Write('|');
                                            file.Write(((Hashtable)(c.layerSize[j]))["longLength"]); file.Write('|');
                                            file.Write(((Hashtable)(c.layerSize[j]))["width"]); file.Write('|');
                                            file.Write(((Hashtable)(c.layerSize[j]))["height"]); file.Write('|');
                                            file.Write(((Hashtable)(c.layerSize[j]))["ObliqueAngle"]); file.Write('|');
                                            //层的左厚度
                                            file.Write(c.layerLeftThickness[j - 1]); file.Write('|');
                                            //层的右厚度
                                            file.Write(c.layerRightThickness[j - 1]);
                                            file.WriteLine();
                                        }
                                    }
                                    else
                                    {
                                        file.Write("无"); file.WriteLine();
                                    }
                                }
                            }
                        }
                        else
                        {
                            file.Write("无");
                            file.WriteLine();
                        }

                        saveFileName = saveFileDialog.FileName;
                    }
                }
            }
            else
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(saveFileName))
                {
                    //保存数据库中材料
                    storeMaterialToFile(saveFileName);

                    int num = 0;
                    foreach (Components c in components)
                    {
                        if (isHaveLeftEndCap && num == 0)
                        {
                            num++;
                            continue;
                        }
                        if (isHaveRightEndCap && num == components.Count - 1)
                        {
                            continue;
                        }

                        //段相关数据
                        file.Write("段"); file.WriteLine();
                        //段大小  4个点坐标
                        file.Write(c.startPoint.X); file.Write(","); file.Write(c.startPoint.Y); file.Write('|');
                        file.Write(c.point2.X); file.Write(","); file.Write(c.point2.Y); file.Write('|');
                        file.Write(c.point3.X); file.Write(","); file.Write(c.point3.Y); file.Write('|');
                        file.Write(c.point4.X); file.Write(","); file.Write(c.point4.Y);
                        if (c.isChangeOgive)
                        {
                            file.Write('|'); file.Write(c.radius); file.Write(","); file.Write(0);
                        }
                        if (c.isChangeIOgive)
                        {
                            file.Write('|'); file.Write(c.radius); file.Write(","); file.Write(1);
                        }
                        file.WriteLine();
                        //段材料  对应颜色
                        file.Write(c.newPath.Fill.ToString()); file.WriteLine();
                        //层相关数据
                        GeometryGroup geometryGroup = (GeometryGroup)c.newPath.Data;

                        if (geometryGroup.Children.Count != 1)
                        {
                            file.Write("层 "); file.Write(geometryGroup.Children.Count - 1); file.WriteLine();

                            for (int i = 1; i < geometryGroup.Children.Count; i++)
                            {
                                //层号
                                file.Write(c.layerNums[i - 1]); file.Write('|');
                                //层的类型
                                file.Write(c.layerType[i - 1]); file.Write('|');
                                //层的材料
                                file.Write(c.layerMaterial[i - 1]); file.Write('|');
                                //层的size
                                file.Write(((Hashtable)(c.layerSize[i]))["diameter"]); file.Write('|');
                                file.Write(((Hashtable)(c.layerSize[i]))["longLength"]); file.Write('|');
                                file.Write(((Hashtable)(c.layerSize[i]))["width"]); file.Write('|');
                                file.Write(((Hashtable)(c.layerSize[i]))["height"]); file.Write('|');
                                file.Write(((Hashtable)(c.layerSize[i]))["ObliqueAngle"]); file.Write('|');
                                //层的左厚度
                                file.Write(c.layerLeftThickness[i - 1]); file.Write('|');
                                //层的右厚度
                                file.Write(c.layerRightThickness[i - 1]);
                                file.WriteLine();
                            }
                        }
                        else
                        {
                            file.Write("无"); file.WriteLine();
                        }

                        num++;
                    }

                    //端盖相关数据
                    file.Write("左端盖"); file.WriteLine();
                    if (isHaveLeftEndCap)
                    {
                        Components c = (Components)components[0];
                        file.Write(c.startPoint.X); file.Write(","); file.Write(c.startPoint.Y); file.Write('|');
                        file.Write(c.point2.X); file.Write(","); file.Write(c.point2.Y); file.Write('|');
                        file.Write(c.point3.X); file.Write(","); file.Write(c.point3.Y); file.Write('|');
                        file.Write(c.point4.X); file.Write(","); file.Write(c.point4.Y);
                        file.WriteLine();
                        //端盖的材料
                        file.Write(c.newPath.Fill.ToString());
                        file.WriteLine();
                    }
                    else
                    {
                        file.Write("无");
                        file.WriteLine();
                    }

                    file.Write("右端盖"); file.WriteLine();
                    if (isHaveRightEndCap)
                    {
                        Components c = (Components)components[components.Count - 1];
                        file.Write(c.startPoint.X); file.Write(","); file.Write(c.startPoint.Y); file.Write('|');
                        file.Write(c.point2.X); file.Write(","); file.Write(c.point2.Y); file.Write('|');
                        file.Write(c.point3.X); file.Write(","); file.Write(c.point3.Y); file.Write('|');
                        file.Write(c.point4.X); file.Write(","); file.Write(c.point4.Y);
                        file.WriteLine();
                        //端盖的材料
                        file.Write(c.newPath.Fill.ToString());
                        file.WriteLine();
                    }
                    else
                    {
                        file.Write("无");
                        file.WriteLine();
                    }

                    //起爆点相关数据
                    file.Write("起爆点 "); file.Write(explosions.Count); file.WriteLine();
                    if (explosions.Count != 0)
                    {
                        //位置  大小  颜色
                        for (int i = 0; i < explosions.Count; i++)
                        {
                            if (explosions[i] is Ellipse)
                            {
                                Ellipse el = (Ellipse)explosions[i];
                                file.Write(Canvas.GetLeft(stackpanel)); file.Write('|');
                                file.Write(Canvas.GetTop(stackpanel) + stackpanel.Height / 2); file.Write('|');
                                file.Write(el.Width); file.Write('|');
                                file.Write(el.Height); file.Write('|');
                                file.Write(((Hashtable)(explosionData[i + 1]))["type"]); file.Write('|');
                                file.Write(((Hashtable)(explosionData[i + 1]))["startPosition"]); file.Write('|');
                                file.Write(((Hashtable)(explosionData[i + 1]))["radius"]); file.Write('|');
                                file.Write(((Hashtable)(explosionData[i + 1]))["pointNums"]); file.Write('|');
                                file.Write(el.Fill.ToString());
                                file.WriteLine();
                            }
                        }
                    }
                    else
                    {
                        file.Write("无");
                        file.WriteLine();
                    }

                    //空心管相关数据
                    file.Write("空心管"); file.WriteLine();
                    if (isHaveCentralTube)
                    {
                        //位置  大小  颜色
                        for (int i = 0; i < centralCubePanel.Children.Count; i++)
                        {
                            if (i != 0)
                            {
                                file.Write("空心管"); file.WriteLine();
                            }
                            if (centralCubePanel.Children[i] is System.Windows.Shapes.Path)
                            {
                                Components c = (Components)centralCubes[i];
                                System.Windows.Shapes.Path rect = centralCubePanel.Children[i] as System.Windows.Shapes.Path;
                                file.Write(c.startPoint.X); file.Write(","); file.Write(c.startPoint.Y); file.Write('|');
                                file.Write(c.point2.X); file.Write(","); file.Write(c.point2.Y); file.Write('|');
                                file.Write(c.point3.X); file.Write(","); file.Write(c.point3.Y); file.Write('|');
                                file.Write(c.point4.X); file.Write(","); file.Write(c.point4.Y);
                                file.Write('|'); file.Write(c.cubeOffset); file.Write(","); file.Write(0);
                                if (c.isChangeOgive)
                                {
                                    file.Write('|'); file.Write(1); file.Write(","); file.Write(1); file.Write('|');
                                    file.Write(c.radius); file.Write(","); file.Write(0);
                                }
                                if (c.isChangeIOgive)
                                {
                                    file.Write('|'); file.Write(2); file.Write(","); file.Write(2); file.Write('|');
                                    file.Write(c.radius); file.Write(","); file.Write(1);
                                }

                                file.WriteLine();


                                GeometryGroup geometryGroup = (GeometryGroup)c.newPath.Data;

                                if (geometryGroup.Children.Count != 1)
                                {
                                    file.Write("空心层 "); file.Write(geometryGroup.Children.Count - 1); file.WriteLine();

                                    for (int j = 1; j < geometryGroup.Children.Count; j++)
                                    {
                                        //层号
                                        file.Write(c.layerNums[j - 1]); file.Write('|');
                                        //层的类型
                                        file.Write(c.layerType[j - 1]); file.Write('|');
                                        //层的材料
                                        file.Write(c.layerMaterial[j - 1]); file.Write('|');
                                        //层的size
                                        file.Write(((Hashtable)(c.layerSize[j]))["diameter"]); file.Write('|');
                                        file.Write(((Hashtable)(c.layerSize[j]))["longLength"]); file.Write('|');
                                        file.Write(((Hashtable)(c.layerSize[j]))["width"]); file.Write('|');
                                        file.Write(((Hashtable)(c.layerSize[j]))["height"]); file.Write('|');
                                        file.Write(((Hashtable)(c.layerSize[j]))["ObliqueAngle"]); file.Write('|');
                                        //层的左厚度
                                        file.Write(c.layerLeftThickness[j - 1]); file.Write('|');
                                        //层的右厚度
                                        file.Write(c.layerRightThickness[j - 1]);
                                        file.WriteLine();
                                    }
                                }
                                else
                                {
                                    file.Write("无"); file.WriteLine();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Store_OtherPlace_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".zr";
            saveFileDialog.Filter = "Text documents (.zr)|*.zr";
            if (saveFileDialog.ShowDialog() == true)
            {
                //File.WriteAllText(saveFileDialog.FileName, "");
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(saveFileDialog.FileName))
                {
                    //保存数据库中材料
                    storeMaterialToFile(saveFileDialog.FileName);

                    int num = 0;
                    foreach (Components c in components)
                    {
                        if (isHaveLeftEndCap && num == 0)
                        {
                            num++;
                            continue;
                        }
                        if (isHaveRightEndCap && num == components.Count - 1)
                        {
                            continue;
                        }

                        //段相关数据
                        file.Write("段"); file.WriteLine();
                        //段大小  4个点坐标
                        file.Write(c.startPoint.X); file.Write(","); file.Write(c.startPoint.Y); file.Write('|');
                        file.Write(c.point2.X); file.Write(","); file.Write(c.point2.Y); file.Write('|');
                        file.Write(c.point3.X); file.Write(","); file.Write(c.point3.Y); file.Write('|');
                        file.Write(c.point4.X); file.Write(","); file.Write(c.point4.Y);
                        if (c.isChangeOgive)
                        {
                            file.Write('|'); file.Write(c.radius); file.Write(","); file.Write(0);
                        }
                        if (c.isChangeIOgive)
                        {
                            file.Write('|'); file.Write(c.radius); file.Write(","); file.Write(1);
                        }
                        file.WriteLine();
                        //段材料  对应颜色
                        file.Write(c.newPath.Fill.ToString()); file.WriteLine();
                        //层相关数据
                        GeometryGroup geometryGroup = (GeometryGroup)c.newPath.Data;

                        if (geometryGroup.Children.Count != 1)
                        {
                            file.Write("层 "); file.Write(geometryGroup.Children.Count - 1); file.WriteLine();

                            for (int i = 1; i < geometryGroup.Children.Count; i++)
                            {
                                //层号
                                file.Write(c.layerNums[i - 1]); file.Write('|');
                                //层的类型
                                file.Write(c.layerType[i - 1]); file.Write('|');
                                //层的材料
                                file.Write(c.layerMaterial[i - 1]); file.Write('|');
                                //层的size
                                file.Write(((Hashtable)(c.layerSize[i]))["diameter"]); file.Write('|');
                                file.Write(((Hashtable)(c.layerSize[i]))["longLength"]); file.Write('|');
                                file.Write(((Hashtable)(c.layerSize[i]))["width"]); file.Write('|');
                                file.Write(((Hashtable)(c.layerSize[i]))["height"]); file.Write('|');
                                file.Write(((Hashtable)(c.layerSize[i]))["ObliqueAngle"]); file.Write('|');
                                //层的左厚度
                                file.Write(c.layerLeftThickness[i - 1]); file.Write('|');
                                //层的右厚度
                                file.Write(c.layerRightThickness[i - 1]);
                                file.WriteLine();
                            }
                        }
                        else
                        {
                            file.Write("无"); file.WriteLine();
                        }

                        num++;
                    }

                    //端盖相关数据
                    file.Write("左端盖"); file.WriteLine();
                    if (isHaveLeftEndCap)
                    {
                        Components c = (Components)components[0];
                        file.Write(c.startPoint.X); file.Write(","); file.Write(c.startPoint.Y); file.Write('|');
                        file.Write(c.point2.X); file.Write(","); file.Write(c.point2.Y); file.Write('|');
                        file.Write(c.point3.X); file.Write(","); file.Write(c.point3.Y); file.Write('|');
                        file.Write(c.point4.X); file.Write(","); file.Write(c.point4.Y);
                        file.WriteLine();
                        //端盖的材料
                        file.Write(c.newPath.Fill.ToString());
                        file.WriteLine();
                    }
                    else
                    {
                        file.Write("无");
                        file.WriteLine();
                    }

                    file.Write("右端盖"); file.WriteLine();
                    if (isHaveRightEndCap)
                    {
                        Components c = (Components)components[components.Count - 1];
                        file.Write(c.startPoint.X); file.Write(","); file.Write(c.startPoint.Y); file.Write('|');
                        file.Write(c.point2.X); file.Write(","); file.Write(c.point2.Y); file.Write('|');
                        file.Write(c.point3.X); file.Write(","); file.Write(c.point3.Y); file.Write('|');
                        file.Write(c.point4.X); file.Write(","); file.Write(c.point4.Y);
                        file.WriteLine();
                        //端盖的材料
                        file.Write(c.newPath.Fill.ToString());
                        file.WriteLine();
                    }
                    else
                    {
                        file.Write("无");
                        file.WriteLine();
                    }

                    //起爆点相关数据
                    file.Write("起爆点 "); file.Write(explosions.Count); file.WriteLine();
                    if (explosions.Count != 0)
                    {
                        //位置  大小  颜色
                        for (int i = 0; i < explosions.Count; i++)
                        {
                            if (explosions[i] is Ellipse)
                            {
                                Ellipse el = (Ellipse)explosions[i];
                                file.Write(Canvas.GetLeft(stackpanel)); file.Write('|');
                                file.Write(Canvas.GetTop(stackpanel) + stackpanel.Height / 2); file.Write('|');
                                file.Write(el.Width); file.Write('|');
                                file.Write(el.Height); file.Write('|');
                                file.Write(((Hashtable)(explosionData[i + 1]))["type"]); file.Write('|');
                                file.Write(((Hashtable)(explosionData[i + 1]))["startPosition"]); file.Write('|');
                                file.Write(((Hashtable)(explosionData[i + 1]))["radius"]); file.Write('|');
                                file.Write(((Hashtable)(explosionData[i + 1]))["pointNums"]); file.Write('|');
                                file.Write(el.Fill.ToString());
                                file.WriteLine();
                            }
                        }
                    }
                    else
                    {
                        file.Write("无");
                        file.WriteLine();
                    }

                    //空心管相关数据
                    file.Write("空心管"); file.WriteLine();
                    if (isHaveCentralTube)
                    {
                        //位置  大小  颜色
                        for (int i = 0; i < centralCubePanel.Children.Count; i++)
                        {
                            if (i != 0)
                            {
                                file.Write("空心管"); file.WriteLine();
                            }
                            if (centralCubePanel.Children[i] is System.Windows.Shapes.Path)
                            {
                                Components c = (Components)centralCubes[i];
                                System.Windows.Shapes.Path rect = centralCubePanel.Children[i] as System.Windows.Shapes.Path;
                                file.Write(c.startPoint.X); file.Write(","); file.Write(c.startPoint.Y); file.Write('|');
                                file.Write(c.point2.X); file.Write(","); file.Write(c.point2.Y); file.Write('|');
                                file.Write(c.point3.X); file.Write(","); file.Write(c.point3.Y); file.Write('|');
                                file.Write(c.point4.X); file.Write(","); file.Write(c.point4.Y);
                                file.Write('|'); file.Write(c.cubeOffset); file.Write(","); file.Write(0);
                                if (c.isChangeOgive)
                                {
                                    file.Write('|'); file.Write(1); file.Write(","); file.Write(1); file.Write('|');
                                    file.Write(c.radius); file.Write(","); file.Write(0);
                                }
                                if (c.isChangeIOgive)
                                {
                                    file.Write('|'); file.Write(2); file.Write(","); file.Write(2); file.Write('|');
                                    file.Write(c.radius); file.Write(","); file.Write(1);
                                }

                                file.WriteLine();


                                GeometryGroup geometryGroup = (GeometryGroup)c.newPath.Data;

                                if (geometryGroup.Children.Count != 1)
                                {
                                    file.Write("空心层 "); file.Write(geometryGroup.Children.Count - 1); file.WriteLine();

                                    for (int j = 1; j < geometryGroup.Children.Count; j++)
                                    {
                                        //层号
                                        file.Write(c.layerNums[j - 1]); file.Write('|');
                                        //层的类型
                                        file.Write(c.layerType[j - 1]); file.Write('|');
                                        //层的材料
                                        file.Write(c.layerMaterial[j - 1]); file.Write('|');
                                        //层的size
                                        file.Write(((Hashtable)(c.layerSize[j]))["diameter"]); file.Write('|');
                                        file.Write(((Hashtable)(c.layerSize[j]))["longLength"]); file.Write('|');
                                        file.Write(((Hashtable)(c.layerSize[j]))["width"]); file.Write('|');
                                        file.Write(((Hashtable)(c.layerSize[j]))["height"]); file.Write('|');
                                        file.Write(((Hashtable)(c.layerSize[j]))["ObliqueAngle"]); file.Write('|');
                                        //层的左厚度
                                        file.Write(c.layerLeftThickness[j - 1]); file.Write('|');
                                        //层的右厚度
                                        file.Write(c.layerRightThickness[j - 1]);
                                        file.WriteLine();
                                    }
                                }
                                else
                                {
                                    file.Write("无"); file.WriteLine();
                                }
                            }
                        }
                    }
                    else
                    {
                        file.Write("无");
                        file.WriteLine();
                    }

                    saveFileName = saveFileDialog.FileName;
                }
            }
        }
        //打开文件
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.DefaultExt = ".zr";
                openFileDialog.Filter = "Text documents (.zr)|*.zr";
                if (openFileDialog.ShowDialog() == true)
                {
                    initWindow();

                    //读取保存的数据库材料
                    DataBaseMaterials = getMaterialFromFile(openFileDialog.FileName);

                    string[] temp = File.ReadAllLines(openFileDialog.FileName);
                    string lastPath = "";
                    int layerNumber = 0;
                    int cubeLayerNumber = 0;
                    int cubeLayerTemp = 0;
                    int layerTemp = 0;
                    int expNum = 0;
                    double radius = -1;
                    int isConvex = -1;
                    int CylinderIndex = 0;
                    changeTitle = 0;

                    foreach (string s in temp)
                    {
                        if (s != null && s != "")
                        {
                            if (s == "段")
                            {
                                lastPath = "段";
                                continue;
                            }
                            if (lastPath == "段" && s[0] != '#')
                            {
                                string[] compArr = s.Split('|');
                                double[] compNum = new double[20];
                                int i = 0;
                                foreach (string str in compArr)
                                {
                                    string[] sArray = str.Split(',');
                                    compNum[i] = int.Parse(sArray[0]);
                                    i++;
                                    compNum[i] = int.Parse(sArray[1]);
                                    i++;
                                }

                                Components cps = new Components(new Point(compNum[0], compNum[1]), new Point(compNum[2], compNum[3]), new Point(compNum[4], compNum[5]), new Point(compNum[6], compNum[7]));
                                components.Add(cps);
                                CylinderIndex = components.IndexOf(cps);

                                cps.newPath.Height += (compNum[1] > compNum[7] ? compNum[7] : compNum[1]) - 100;
                                cps.newPath.MouseRightButtonDown += viewbox_MouseRightButtonDown;
                                cps.newPath.MouseDown += Img1_MouseLeftButtonDown;
                                cps.newPath.MouseUp += Img1_MouseLeftButtonUp;
                                stackpanel.Children.Add(cps.newPath);
                                insertShape = cps.newPath;
                                radius = (double)compNum[8];
                                isConvex = (int)compNum[9];
                                cps.radius = radius;

                                //自动调整图形位置
                                autoResize();

                                allWidth = 0;
                                continue;
                            }
                            if (lastPath == "段" && s[0] == '#')
                            {
                                Components cps = (Components)components[components.Count - 1];
                                cps.newPath.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(s));
                                lastPath = "";
                                continue;
                            }
                            if (s[0] == '层')
                            {
                                string[] lArr = s.Split(' ');
                                layerNumber = Int32.Parse(lArr[1]);
                                curLayerNum = layerNumber;
                                lastPath = "层";
                                continue;
                            }

                            if (lastPath == "层")
                            {
                                string[] layerArr = s.Split('|');
                                ((Components)components[components.Count - 1]).layerNum += 1;
                                ((Components)components[components.Count - 1]).layerNums.Add(Int32.Parse(layerArr[0]));
                                ((Components)components[components.Count - 1]).layerType.Add(layerArr[1]);
                                ((Components)components[components.Count - 1]).layerMaterial.Add(layerArr[2]);
                                ((Components)components[components.Count - 1]).layerSize.Add(Int32.Parse(layerArr[0]), new Hashtable());
                                ((Hashtable)(((Components)components[components.Count - 1]).layerSize[Int32.Parse(layerArr[0])])).Add("diameter", layerArr[3]);
                                ((Hashtable)(((Components)components[components.Count - 1]).layerSize[Int32.Parse(layerArr[0])])).Add("longLength", layerArr[4]);
                                ((Hashtable)(((Components)components[components.Count - 1]).layerSize[Int32.Parse(layerArr[0])])).Add("width", layerArr[5]);
                                ((Hashtable)(((Components)components[components.Count - 1]).layerSize[Int32.Parse(layerArr[0])])).Add("height", layerArr[6]);
                                ((Hashtable)(((Components)components[components.Count - 1]).layerSize[Int32.Parse(layerArr[0])])).Add("ObliqueAngle", layerArr[7]);
                                ((Components)components[components.Count - 1]).layerLeftThickness.Add(layerArr[8]);
                                ((Components)components[components.Count - 1]).layerRightThickness.Add(layerArr[9]);
                                layerTemp++;
                                if (layerTemp % 2 == 1)
                                {
                                    continue;
                                }
                            }
                            if (lastPath == "层" && layerTemp % 2 == 0)
                            {
                                GeometryGroup geometryGroup = (GeometryGroup)((Components)components[components.Count - 1]).newPath.Data;
                                if (((Components)components[components.Count - 1]).layerNum <= 2)
                                {
                                    double leftWidth = Double.Parse(((Components)components[components.Count - 1]).layerLeftThickness[((Components)components[components.Count - 1]).layerNum - 2].ToString());
                                    double rightWidth = Double.Parse(((Components)components[components.Count - 1]).layerRightThickness[((Components)components[components.Count - 1]).layerNum - 2].ToString());
                                    double maxCh_Width = leftWidth > rightWidth ? leftWidth : rightWidth;
                                    PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
                                    PathFigure curPf = curPg.Figures.ElementAt(0);
                                    Point startP = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + maxCh_Width);
                                    curPf.StartPoint = startP;
                                    ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + maxCh_Width);
                                    ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + maxCh_Width);
                                    ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + maxCh_Width);
                                    if (curPf.Segments.Count > 3)
                                    {
                                        ((LineSegment)curPf.Segments[3]).Point = new Point(((LineSegment)curPf.Segments[3]).Point.X, ((LineSegment)curPf.Segments[3]).Point.Y + maxCh_Width);
                                    }

                                    //创建上下层路径
                                    PathGeometry topPg = new PathGeometry();
                                    PathGeometry bottomPg = new PathGeometry();

                                    PathFigure topPf = new PathFigure();
                                    PathFigure bottomPf = new PathFigure();

                                    //绘制上层路径
                                    topPf.StartPoint = curPf.StartPoint;
                                    LineSegment ls_top = new LineSegment();
                                    ls_top.Point = ((LineSegment)curPf.Segments[2]).Point;
                                    topPf.Segments.Add(ls_top);
                                    
                                    LineSegment ls2_top = new LineSegment();
                                    Point thirdPoint_top = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y - rightWidth);
                                    ls2_top.Point = thirdPoint_top;
                                    topPf.Segments.Add(ls2_top);

                                    LineSegment ls3_top = new LineSegment();
                                    Point forthPoint_top = new Point(curPf.StartPoint.X, curPf.StartPoint.Y - leftWidth);
                                    ls3_top.Point = forthPoint_top;
                                    topPf.Segments.Add(ls3_top);
                                    
                                    topPg.Figures.Add(topPf);
                                    topPf.IsClosed = true;

                                    Color color = new Color();
                                    if (DataBaseMaterials != null && DataBaseMaterials.Count != 0)
                                    {
                                        bool isfind = false;
                                        for (int j = 0; j < DataBaseMaterials.Count; j++)
                                        {
                                            Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["color"];
                                            Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["materialName"];
                                            string mColor = cdic["content"];
                                            string materialName = mdic["content"];
                                            if (materialName == ((Components)components[components.Count - 1]).layerMaterial[((Components)components[components.Count - 1]).layerNum - 2].ToString())
                                            {
                                                isfind = true;
                                                color = (Color)ColorConverter.ConvertFromString(mColor);
                                            }
                                        }
                                        if (isfind == false)
                                        {
                                            color = (Color)ColorConverter.ConvertFromString("#FFFFA500");
                                        }
                                    }
                                    else
                                    {
                                        color = (Color)ColorConverter.ConvertFromString("#FFFFA500");
                                    }
                                   
                                    ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, insertShape, topPf, 0, color);

                                    //绘制下层路径
                                    bottomPf.StartPoint = ((LineSegment)curPf.Segments[0]).Point;
                                    LineSegment ls_bottom = new LineSegment();
                                    Point secondPoint_bottom = new Point(bottomPf.StartPoint.X, bottomPf.StartPoint.Y + leftWidth);
                                    ls_bottom.Point = secondPoint_bottom;
                                    bottomPf.Segments.Add(ls_bottom);

                                    LineSegment ls2_bottom = new LineSegment();
                                    Point thirdPoint_bottom = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + rightWidth);
                                    ls2_bottom.Point = thirdPoint_bottom;
                                    bottomPf.Segments.Add(ls2_bottom);
                                    
                                    LineSegment ls3_bottom = new LineSegment();
                                    Point forthPoint_bottom = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y);
                                    ls3_bottom.Point = forthPoint_bottom;
                                    bottomPf.Segments.Add(ls3_bottom);

                                    if (curPf.Segments.Count > 3)
                                    {
                                        LineSegment ls4_bottom = new LineSegment();
                                        ls4_bottom.Point = bottomPf.StartPoint;
                                        bottomPf.Segments.Add(ls4_bottom);
                                    }

                                    bottomPg.Figures.Add(bottomPf);

                                    if (DataBaseMaterials != null && DataBaseMaterials.Count != 0)
                                    {
                                        bool isfind = false;
                                        for (int j = 0; j < DataBaseMaterials.Count; j++)
                                        {
                                            Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["color"];
                                            Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["materialName"];
                                            string mColor = cdic["content"];
                                            string materialName = mdic["content"];
                                            if (materialName == ((Components)components[components.Count - 1]).layerMaterial[((Components)components[components.Count - 1]).layerNum - 2].ToString())
                                            {
                                                isfind = true;
                                                color = (Color)ColorConverter.ConvertFromString(mColor);
                                            }
                                        }
                                        if (isfind == false)
                                        {
                                            color = (Color)ColorConverter.ConvertFromString("#FFFFA500");
                                        }
                                    }
                                    else
                                    {
                                        color = (Color)ColorConverter.ConvertFromString("#FFFFA500");
                                    }
                                    
                                    ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, insertShape, bottomPf, 1, color);
                                    //将上下层路径添加到组中
                                    geometryGroup.Children.Add(topPg);
                                    geometryGroup.Children.Add(bottomPg);

                                    ((Components)components[components.Count - 1]).newPath.Height += 2 * (leftWidth > rightWidth ? leftWidth : rightWidth);
                                    ((Components)components[components.Count - 1]).height = ((Components)components[components.Count - 1]).newPath.Height;


                                    //自动调整图形位置
                                    autoResize();
                                }
                                else
                                {
                                    double leftWidth = Double.Parse(((Components)components[components.Count - 1]).layerLeftThickness[geometryGroup.Children.Count - 1].ToString());
                                    double rightWidth = Double.Parse(((Components)components[components.Count - 1]).layerRightThickness[geometryGroup.Children.Count - 1].ToString());
                                    double maxCh_Width = leftWidth > rightWidth ? leftWidth : rightWidth;
                                    double minCh_Width = leftWidth < rightWidth ? leftWidth : rightWidth;
                                    int i;
                                    PathGeometry curPg_ = (PathGeometry)geometryGroup.Children[0];
                                    PathFigure curPf_ = curPg_.Figures.ElementAt(0);
                                    Point startP_ = new Point(curPf_.StartPoint.X, curPf_.StartPoint.Y + maxCh_Width);
                                    curPf_.StartPoint = startP_;
                                    ((LineSegment)curPf_.Segments[0]).Point = new Point(((LineSegment)curPf_.Segments[0]).Point.X, ((LineSegment)curPf_.Segments[0]).Point.Y + maxCh_Width);
                                    ((LineSegment)curPf_.Segments[1]).Point = new Point(((LineSegment)curPf_.Segments[1]).Point.X, ((LineSegment)curPf_.Segments[1]).Point.Y + maxCh_Width);
                                    ((LineSegment)curPf_.Segments[2]).Point = new Point(((LineSegment)curPf_.Segments[2]).Point.X, ((LineSegment)curPf_.Segments[2]).Point.Y + maxCh_Width);
                                    if (curPf_.Segments.Count > 3)
                                    {
                                        ((LineSegment)curPf_.Segments[3]).Point = new Point(((LineSegment)curPf_.Segments[3]).Point.X, ((LineSegment)curPf_.Segments[3]).Point.Y + maxCh_Width);
                                    }
                                    
                                    //层
                                    for (i = 2; i < geometryGroup.Children.Count; i += 2)
                                    {
                                        PathGeometry curPg = (PathGeometry)geometryGroup.Children[i - 1];
                                        PathFigure curPf = curPg.Figures.ElementAt(0);
                                        Point startP = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + maxCh_Width);
                                        curPf.StartPoint = startP;
                                        ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + maxCh_Width);
                                        ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + maxCh_Width);
                                        ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + maxCh_Width);

                                        PathGeometry curPg2 = (PathGeometry)geometryGroup.Children[i];
                                        PathFigure curPf2 = curPg2.Figures.ElementAt(0);
                                        Point startP2 = new Point(curPf2.StartPoint.X, curPf2.StartPoint.Y + maxCh_Width);
                                        curPf2.StartPoint = startP2;
                                        ((LineSegment)curPf2.Segments[0]).Point = new Point(((LineSegment)curPf2.Segments[0]).Point.X, ((LineSegment)curPf2.Segments[0]).Point.Y + maxCh_Width);
                                        ((LineSegment)curPf2.Segments[1]).Point = new Point(((LineSegment)curPf2.Segments[1]).Point.X, ((LineSegment)curPf2.Segments[1]).Point.Y + maxCh_Width);
                                        ((LineSegment)curPf2.Segments[2]).Point = new Point(((LineSegment)curPf2.Segments[2]).Point.X, ((LineSegment)curPf2.Segments[2]).Point.Y + maxCh_Width);
                                        if (curPf2.Segments.Count > 3)
                                        {
                                            ((LineSegment)curPf2.Segments[3]).Point = new Point(((LineSegment)curPf2.Segments[3]).Point.X, ((LineSegment)curPf2.Segments[3]).Point.Y + maxCh_Width);
                                        }
                                    }

                                    i -= 2;
                                    if (i == ((Components)components[components.Count - 1]).layerNum - 2)
                                    {
                                        PathGeometry curPg = (PathGeometry)geometryGroup.Children[i - 1];
                                        PathFigure curPf = curPg.Figures.ElementAt(0);
                                        //创建上下层路径
                                        PathGeometry topPg = new PathGeometry();
                                        PathFigure topPf = new PathFigure();

                                        //绘制上层路径
                                        topPf.StartPoint = ((LineSegment)curPf.Segments[2]).Point;
                                        LineSegment ls_top = new LineSegment();
                                        ls_top.Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y);
                                        topPf.Segments.Add(ls_top);

                                        LineSegment ls2_top = new LineSegment();
                                        Point thirdPoint_top = new Point(ls_top.Point.X, ls_top.Point.Y - rightWidth);
                                        ls2_top.Point = thirdPoint_top;
                                        topPf.Segments.Add(ls2_top);

                                        LineSegment ls3_top = new LineSegment();
                                        Point forthPoint_top = new Point(topPf.StartPoint.X, topPf.StartPoint.Y - leftWidth);
                                        ls3_top.Point = forthPoint_top;
                                        topPf.Segments.Add(ls3_top);

                                        topPg.Figures.Add(topPf);
                                        topPf.IsClosed = true;

                                        Color color = new Color();
                                        if (DataBaseMaterials != null && DataBaseMaterials.Count != 0)
                                        {
                                            bool isfind = false;
                                            for (int j = 0; j < DataBaseMaterials.Count; j++)
                                            {
                                                Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["color"];
                                                Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["materialName"];
                                                string mColor = cdic["content"];
                                                string materialName = mdic["content"];
                                                if (materialName == ((Components)components[components.Count - 1]).layerMaterial[((Components)components[components.Count - 1]).layerNum - 2].ToString())
                                                {
                                                    isfind = true;
                                                    color = (Color)ColorConverter.ConvertFromString(mColor);
                                                }
                                            }
                                            if (isfind == false)
                                            {
                                                color = (Color)ColorConverter.ConvertFromString("#FFFFA500");
                                            }
                                        }
                                        else
                                        {
                                            color = (Color)ColorConverter.ConvertFromString("#FFFFA500");
                                        }
                                        
                                        ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, insertShape, topPf, 0, color);

                                        //将上下层路径添加到组中
                                        geometryGroup.Children.Add(topPg);
                                    }
                                    if (i == ((Components)components[components.Count - 1]).layerNum - 2)
                                    {
                                        PathGeometry curPg2 = (PathGeometry)geometryGroup.Children[i];
                                        PathFigure curPf2 = curPg2.Figures.ElementAt(0);
                                        PathGeometry bottomPg = new PathGeometry();
                                        PathFigure bottomPf = new PathFigure();

                                        //绘制下层路径
                                        bottomPf.StartPoint = ((LineSegment)curPf2.Segments[0]).Point;
                                        LineSegment ls_bottom = new LineSegment();
                                        Point secondPoint_bottom = new Point(bottomPf.StartPoint.X, bottomPf.StartPoint.Y + leftWidth);
                                        ls_bottom.Point = secondPoint_bottom;
                                        bottomPf.Segments.Add(ls_bottom);

                                        LineSegment ls2_bottom = new LineSegment();
                                        Point thirdPoint_bottom = new Point(((LineSegment)curPf2.Segments[1]).Point.X, ((LineSegment)curPf2.Segments[1]).Point.Y + rightWidth);
                                        ls2_bottom.Point = thirdPoint_bottom;
                                        bottomPf.Segments.Add(ls2_bottom);

                                        LineSegment ls3_bottom = new LineSegment();
                                        Point forthPoint_bottom = new Point(((LineSegment)curPf2.Segments[1]).Point.X, ((LineSegment)curPf2.Segments[1]).Point.Y);
                                        ls3_bottom.Point = forthPoint_bottom;
                                        bottomPf.Segments.Add(ls3_bottom);

                                        if (curPf2.Segments.Count > 3)
                                        {
                                            LineSegment ls4_bottom = new LineSegment();
                                            ls4_bottom.Point = bottomPf.StartPoint;
                                            bottomPf.Segments.Add(ls4_bottom);
                                        }
                                        bottomPg.Figures.Add(bottomPf);

                                        Color color = new Color();
                                        if (DataBaseMaterials != null && DataBaseMaterials.Count != 0)
                                        {
                                            bool isfind = false;
                                            for (int j = 0; j < DataBaseMaterials.Count; j++)
                                            {
                                                Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["color"];
                                                Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)DataBaseMaterials[j])["materialName"];
                                                string mColor = cdic["content"];
                                                string materialName = mdic["content"];
                                                if (materialName == ((Components)components[components.Count - 1]).layerMaterial[((Components)components[components.Count - 1]).layerNum - 2].ToString())
                                                {
                                                    isfind = true;
                                                    color = (Color)ColorConverter.ConvertFromString(mColor);
                                                }
                                            }
                                            if (isfind == false)
                                            {
                                                color = (Color)ColorConverter.ConvertFromString("#FFFFA500");
                                            }
                                        }
                                        else
                                        {
                                            color = (Color)ColorConverter.ConvertFromString("#FFFFA500");
                                        }

                                        
                                        ColorProc.processWhenAddLayer(this.canvas, this.stackpanel, insertShape, bottomPf, 1, color);

                                        //将上下层路径添加到组中
                                        geometryGroup.Children.Add(bottomPg);

                                    }
                                    ((Components)components[components.Count - 1]).newPath.Height += 2 * (leftWidth > rightWidth ? leftWidth : rightWidth);
                                    ((Components)components[components.Count - 1]).height = ((Components)components[components.Count - 1]).newPath.Height;

                                    //
                                    autoResize();
                                }
                            }

                            if (lastPath == "层" && layerTemp == layerNumber)
                            {
                                if (radius != -1 && isConvex != -1)
                                {
                                    changeLineSegmentToArcSegment(radius, isConvex);
                                }
                                radius = -1;
                                isConvex = -1;

                                lastPath = "";
                                layerTemp = 0;

                                continue;
                            }

                            if (s == "左端盖")
                            {
                                lastPath = "左端盖";
                                continue;
                            }

                            if (lastPath == "左端盖" && s != "无" && s[0] != '#')
                            {
                                isHaveLeftEndCap = true;
                                MenuItem mi = (MenuItem)((MenuItem)aMenu.Items[1]).Items[0];
                                mi.IsEnabled = false;
                                string[] endCapArr = s.Split('|');
                                double[] endCapNum = new double[20];
                                int i = 0;
                                foreach (string str in endCapArr)
                                {
                                    string[] sArray = str.Split(',');
                                    endCapNum[i] = int.Parse(sArray[0]);
                                    i++;
                                    endCapNum[i] = int.Parse(sArray[1]);
                                    i++;
                                }

                                Components cps = new Components(new Point(endCapNum[0], endCapNum[1]), new Point(endCapNum[2], endCapNum[3]), new Point(endCapNum[4], endCapNum[5]), new Point(endCapNum[6], endCapNum[7]));
                                components.Insert(0, cps);
                                cps.newPath.MouseRightButtonDown += viewbox_MouseRightButtonDown;
                                cps.newPath.MouseDown += Img1_MouseLeftButtonDown;
                                cps.newPath.MouseUp += Img1_MouseLeftButtonUp;
                                stackpanel.Children.Insert(0, cps.newPath);

                                //自动调整图形位置
                                autoResize();

                                allWidth = 0;
                                continue;
                            }

                            if (lastPath == "左端盖" && s[0] == '#')
                            {
                                Components cps = (Components)components[0];
                                cps.newPath.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(s));
                                lastPath = "";
                                continue;
                            }

                            if (lastPath == "左端盖" && s == "无")
                            {
                                lastPath = "";
                                continue;
                            }

                            if (s == "右端盖")
                            {
                                lastPath = "右端盖";
                                continue;
                            }

                            if (lastPath == "右端盖" && s != "无" && s[0] != '#')
                            {
                                isHaveRightEndCap = true;
                                MenuItem mi = (MenuItem)((MenuItem)aMenu.Items[1]).Items[1];
                                mi.IsEnabled = false;
                                string[] endCapArr = s.Split('|');
                                double[] endCapNum = new double[20];
                                int i = 0;
                                foreach (string str in endCapArr)
                                {
                                    string[] sArray = str.Split(',');
                                    endCapNum[i] = int.Parse(sArray[0]);
                                    i++;
                                    endCapNum[i] = int.Parse(sArray[1]);
                                    i++;
                                }

                                Components cps = new Components(new Point(endCapNum[0], endCapNum[1]), new Point(endCapNum[2], endCapNum[3]), new Point(endCapNum[4], endCapNum[5]), new Point(endCapNum[6], endCapNum[7]));
                                components.Add(cps);
                                cps.newPath.MouseRightButtonDown += viewbox_MouseRightButtonDown;
                                cps.newPath.MouseDown += Img1_MouseLeftButtonDown;
                                cps.newPath.MouseUp += Img1_MouseLeftButtonUp;
                                stackpanel.Children.Add(cps.newPath);

                                //自动调整图形位置
                                autoResize();

                                allWidth = 0;
                                continue;
                            }

                            if (lastPath == "右端盖" && s[0] == '#')
                            {
                                Components cps = (Components)components[components.Count - 1];
                                cps.newPath.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(s));
                                lastPath = "";
                                continue;
                            }

                            if (lastPath == "右端盖" && s == "无")
                            {
                                lastPath = "";
                                continue;
                            }

                            if (s[0] == '起')
                            {
                                string[] explisionArr = s.Split(' ');
                                expNum = Int32.Parse(explisionArr[1]);
                                lastPath = "起爆点";
                                continue;
                            }
                            if (lastPath == "起爆点" && s != "无")
                            {
                                string[] ExplosionArr = s.Split('|');

                                double leftX = Double.Parse(ExplosionArr[0]);
                                double topY = Double.Parse(ExplosionArr[1]);

                                Ellipse el = new Ellipse();
                                el.Width = Int32.Parse(ExplosionArr[2]);
                                el.Height = Int32.Parse(ExplosionArr[3]);
                                el.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ExplosionArr[8]));
                                el.Stroke = Brushes.Black;
                                el.MouseDown += explosion_MouseRightButtonDown;
                                el.MouseDown += Img1_MouseLeftButtonDown;
                                el.MouseUp += Img1_MouseLeftButtonUp;

                                explosions.Add(el);
                                explosionData.Add(explosions.Count, new Hashtable());
                                ((Hashtable)(explosionData[explosions.Count])).Add("type", ExplosionArr[4]);
                                ((Hashtable)(explosionData[explosions.Count])).Add("startPosition", ExplosionArr[5]);
                                ((Hashtable)(explosionData[explosions.Count])).Add("radius", ExplosionArr[6]);
                                ((Hashtable)(explosionData[explosions.Count])).Add("pointNums", ExplosionArr[7]);

                                canvas.Children.Add(el);
                                double duangai = 0;
                                if (isHaveLeftEndCap)
                                {
                                    Components cps = (Components)components[0];
                                    duangai = cps.newPath.Width;
                                }

                                Canvas.SetLeft(el, leftX + Double.Parse(ExplosionArr[5]) + duangai);
                                Canvas.SetTop(el, topY - el.Height / 2 + Double.Parse(ExplosionArr[6]));

                                if (explosions.Count == expNum)
                                {
                                    lastPath = "";
                                    continue;
                                }
                            }

                            if (lastPath == "起爆点" && s == "无")
                            {
                                lastPath = "";
                                continue;
                            }

                            if (s == "空心管")
                            {
                                lastPath = "空心管";
                                continue;
                            }
                            if (lastPath == "空心管" && s != "无" && s[0] != '空')
                            {
                                string[] CentralTubeArr = s.Split('|');

                                double[] compNum = new double[20];
                                int i = 0;
                                foreach (string str in CentralTubeArr)
                                {
                                    string[] sArray = str.Split(',');
                                    compNum[i] = int.Parse(sArray[0]);
                                    i++;
                                    compNum[i] = int.Parse(sArray[1]);
                                    i++;
                                }

                                Components cps = new Components(new Point(compNum[0], compNum[1]), new Point(compNum[2], compNum[3]), new Point(compNum[4], compNum[5]), new Point(compNum[6], compNum[7]));
                                cps.layerNum = 0;
                                centralCubes.Add(cps);
                                cps.newPath.Height += (compNum[1] > compNum[7] ? compNum[7] : compNum[1]) - 100;
                                cps.newPath.MouseDown += CentralCube_MouseLeftButtonDown;
                                cps.newPath.MouseRightButtonDown += cube_MouseRightButtonDown;
                                cps.newPath.MouseUp += Img1_MouseLeftButtonUp;
                                centralCubePanel.Children.Add(cps.newPath);
                                changeTitle = 2;
                                curLayerNum = 0;
                                insertShape = cps.newPath;
                                if (compNum[10] == 1 || compNum[10] == 2)
                                {
                                    changeLineSegmentToArcSegment((double)compNum[12], (int)compNum[13]);
                                    cps.radius = (double)compNum[12];
                                }

                                //double leftX = Double.Parse(CentralTubeArr[4]);
                                //double topY = Double.Parse(CentralTubeArr[5]);

                                //double duangai = 0;
                                //if (isHaveLeftEndCap)
                                //{
                                //    Components cps1 = (Components)components[0];
                                //    duangai = cps1.newPath.Width;
                                //}
                                //Canvas.SetLeft(centralCubePanel, leftX + Double.Parse(CentralTubeArr[7]) + duangai);
                                cps.cubeOffset = compNum[8];
                                //Console.Write(Canvas.GetLeft(centralCubePanel));
                                //Canvas.SetTop(centralCubePanel, topY + (stackpanel.Height - cps.newPath.Height) / 2);
                                //cps.newPath.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(CentralTubeArr[6]));
                                //cps.newPath.Stroke = Brushes.Blue;

                                isHaveCentralTube = true;
                                autoResize();
                                
                                continue;
                            }
                            if (s.Contains("空心层"))
                            {
                                string[] lArr = s.Split(' ');
                                cubeLayerNumber = Int32.Parse(lArr[1]);
                                curLayerNum = cubeLayerNumber;
                                lastPath = "空心层";
                                cubeLayerTemp = 0;
                                continue;
                            }

                            if (lastPath == "空心层" && cubeLayerNumber != cubeLayerTemp)
                            {
                                string[] layerArr = s.Split('|');
                                ((Components)centralCubes[centralCubes.Count - 1]).layerNum += 1;
                                ((Components)centralCubes[centralCubes.Count - 1]).layerNums.Add(Int32.Parse(layerArr[0]));
                                ((Components)centralCubes[centralCubes.Count - 1]).layerType.Add(layerArr[1]);
                                ((Components)centralCubes[centralCubes.Count - 1]).layerMaterial.Add(layerArr[2]);
                                ((Components)centralCubes[centralCubes.Count - 1]).layerSize.Add(Int32.Parse(layerArr[0]), new Hashtable());
                                ((Hashtable)(((Components)centralCubes[centralCubes.Count - 1]).layerSize[Int32.Parse(layerArr[0])])).Add("diameter", layerArr[3]);
                                ((Hashtable)(((Components)centralCubes[centralCubes.Count - 1]).layerSize[Int32.Parse(layerArr[0])])).Add("longLength", layerArr[4]);
                                ((Hashtable)(((Components)centralCubes[centralCubes.Count - 1]).layerSize[Int32.Parse(layerArr[0])])).Add("width", layerArr[5]);
                                ((Hashtable)(((Components)centralCubes[centralCubes.Count - 1]).layerSize[Int32.Parse(layerArr[0])])).Add("height", layerArr[6]);
                                ((Hashtable)(((Components)centralCubes[centralCubes.Count - 1]).layerSize[Int32.Parse(layerArr[0])])).Add("ObliqueAngle", layerArr[7]);
                                ((Components)centralCubes[centralCubes.Count - 1]).layerLeftThickness.Add(layerArr[8]);
                                ((Components)centralCubes[centralCubes.Count - 1]).layerRightThickness.Add(layerArr[9]);
                                cubeLayerTemp++;
                            }
                            if (lastPath == "空心层" && cubeLayerNumber == cubeLayerTemp)
                            {
                                //添加层
                                //获取当前图形的路径
                                double leftWidth = Double.Parse(((Components)centralCubes[centralCubes.Count - 1]).layerLeftThickness[((Components)centralCubes[centralCubes.Count - 1]).layerNum - 2].ToString());
                                double rightWidth = Double.Parse(((Components)centralCubes[centralCubes.Count - 1]).layerRightThickness[((Components)centralCubes[centralCubes.Count - 1]).layerNum - 2].ToString());
                                double maxCh_Width = leftWidth > rightWidth ? leftWidth : rightWidth;
                                GeometryGroup geometryGroup = (GeometryGroup)((System.Windows.Shapes.Path)centralCubePanel.Children[centralCubes.Count - 1]).Data;
                                PathGeometry curPg = (PathGeometry)geometryGroup.Children[0];
                                PathFigure curPf = curPg.Figures.ElementAt(0);
                                Point startP = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + maxCh_Width);
                                curPf.StartPoint = startP;
                                ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + maxCh_Width);
                                if (curPf.Segments[1] is LineSegment)
                                {
                                    ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + maxCh_Width);
                                }
                                else if (curPf.Segments[1] is ArcSegment)
                                {
                                    ((ArcSegment)curPf.Segments[1]).Point = new Point(((ArcSegment)curPf.Segments[1]).Point.X, ((ArcSegment)curPf.Segments[1]).Point.Y + maxCh_Width);
                                }
                                else
                                {
                                    for (int i = 0; i < ((PolyLineSegment)curPf.Segments[1]).Points.Count; i++)
                                    {
                                        ((PolyLineSegment)curPf.Segments[1]).Points[i] = new Point(((PolyLineSegment)curPf.Segments[1]).Points[i].X, ((PolyLineSegment)curPf.Segments[1]).Points[i].Y + maxCh_Width);
                                    }
                                }
                                ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + maxCh_Width);
                                if (curPf.Segments[3] is LineSegment)
                                {
                                    ((LineSegment)curPf.Segments[3]).Point = new Point(((LineSegment)curPf.Segments[3]).Point.X, ((LineSegment)curPf.Segments[3]).Point.Y + maxCh_Width);
                                }
                                else if (curPf.Segments[3] is ArcSegment)
                                {
                                    ((ArcSegment)curPf.Segments[3]).Point = new Point(((ArcSegment)curPf.Segments[3]).Point.X, ((ArcSegment)curPf.Segments[3]).Point.Y + maxCh_Width);
                                }
                                else
                                {
                                    for (int i = 0; i < ((PolyLineSegment)curPf.Segments[3]).Points.Count; i++)
                                    {
                                        ((PolyLineSegment)curPf.Segments[3]).Points[i] = new Point(((PolyLineSegment)curPf.Segments[3]).Points[i].X, ((PolyLineSegment)curPf.Segments[3]).Points[i].Y + maxCh_Width);
                                    }
                                }

                                //创建上下层路径
                                PathGeometry topPg = new PathGeometry();
                                PathGeometry bottomPg = new PathGeometry();

                                PathFigure topPf = new PathFigure();
                                PathFigure bottomPf = new PathFigure();

                                //绘制上层路径
                                topPf.StartPoint = curPf.StartPoint;
                                if (curPf.Segments[3] is LineSegment)
                                {
                                    LineSegment ls_top = new LineSegment();
                                    ls_top.Point = ((LineSegment)curPf.Segments[2]).Point;
                                    topPf.Segments.Add(ls_top);
                                }
                                else if (curPf.Segments[3] is ArcSegment)
                                {
                                    ArcSegment ls_top = new ArcSegment();
                                    ls_top.Point = ((LineSegment)curPf.Segments[2]).Point;
                                    ls_top.Size = ((ArcSegment)curPf.Segments[3]).Size;
                                    if (((ArcSegment)curPf.Segments[3]).SweepDirection == SweepDirection.Clockwise)
                                    {
                                        ls_top.SweepDirection = SweepDirection.Counterclockwise;
                                    }
                                    else
                                    {
                                        ls_top.SweepDirection = SweepDirection.Clockwise;
                                    }
                                    topPf.Segments.Add(ls_top);
                                }
                                else
                                {
                                    PolyLineSegment pls_top = new PolyLineSegment();
                                    for (int i = ((PolyLineSegment)curPf.Segments[3]).Points.Count - 1; i >= 0; i--)
                                    {
                                        pls_top.Points.Add(((PolyLineSegment)curPf.Segments[3]).Points[i]);
                                    }
                                    topPf.Segments.Add(pls_top);
                                }

                                LineSegment ls2_top = new LineSegment();
                                Point thirdPoint_top = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y - rightWidth);
                                ls2_top.Point = thirdPoint_top;
                                topPf.Segments.Add(ls2_top);

                                if (curPf.Segments[3] is LineSegment)
                                {
                                    LineSegment ls3_top = new LineSegment();
                                    Point forthPoint_top = new Point(curPf.StartPoint.X, curPf.StartPoint.Y - leftWidth);
                                    ls3_top.Point = forthPoint_top;
                                    topPf.Segments.Add(ls3_top);
                                }
                                else if (curPf.Segments[3] is ArcSegment)
                                {
                                    ArcSegment ls3_top = new ArcSegment();
                                    Point forthPoint_top = new Point(curPf.StartPoint.X, curPf.StartPoint.Y - leftWidth);
                                    ls3_top.Point = forthPoint_top;
                                    ls3_top.Size = ((ArcSegment)curPf.Segments[3]).Size;
                                    //ls3_top.SweepDirection = SweepDirection.Counterclockwise;
                                    ls3_top.SweepDirection = ((ArcSegment)curPf.Segments[3]).SweepDirection;
                                    topPf.Segments.Add(ls3_top);
                                }
                                else
                                {
                                    PolyLineSegment pls2_top = new PolyLineSegment();
                                    //pls2_top.Points = ((PolyLineSegment)curPf.Segments[3]).Points;
                                    for (int i = 0; i < ((PolyLineSegment)curPf.Segments[3]).Points.Count; i++)
                                    {
                                        pls2_top.Points.Add(new Point(((PolyLineSegment)curPf.Segments[3]).Points[i].X, ((PolyLineSegment)curPf.Segments[3]).Points[i].Y - leftWidth));
                                    }
                                    topPf.Segments.Add(pls2_top);
                                }
                                topPg.Figures.Add(topPf);
                                topPf.IsClosed = true;

                                ColorProc.processWhenAddLayer_CC(this.canvas, this.centralCubePanel, (System.Windows.Shapes.Path)centralCubePanel.Children[centralCubePanel.Children.Count - 1], topPf, 0, (Color)ColorConverter.ConvertFromString("#FF787878"));

                                //绘制下层路径
                                bottomPf.StartPoint = ((LineSegment)curPf.Segments[0]).Point;
                                LineSegment ls_bottom = new LineSegment();
                                Point secondPoint_bottom = new Point(bottomPf.StartPoint.X, bottomPf.StartPoint.Y + leftWidth);
                                ls_bottom.Point = secondPoint_bottom;
                                bottomPf.Segments.Add(ls_bottom);

                                if (curPf.Segments[1] is LineSegment)
                                {
                                    LineSegment ls2_bottom = new LineSegment();
                                    Point thirdPoint_bottom = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + rightWidth);
                                    ls2_bottom.Point = thirdPoint_bottom;
                                    bottomPf.Segments.Add(ls2_bottom);
                                }
                                else if (curPf.Segments[1] is ArcSegment)
                                {
                                    ArcSegment ls2_bottom = new ArcSegment();
                                    Point thirdPoint_bottom = new Point(((ArcSegment)curPf.Segments[1]).Point.X, ((ArcSegment)curPf.Segments[1]).Point.Y + rightWidth);
                                    ls2_bottom.Point = thirdPoint_bottom;
                                    ls2_bottom.Size = ((ArcSegment)curPf.Segments[1]).Size;
                                    //ls2_bottom.SweepDirection = SweepDirection.Counterclockwise;
                                    ls2_bottom.SweepDirection = ((ArcSegment)curPf.Segments[1]).SweepDirection;
                                    bottomPf.Segments.Add(ls2_bottom);
                                }
                                else
                                {
                                    PolyLineSegment pls_bottom = new PolyLineSegment();
                                    for (int i = 0; i < ((PolyLineSegment)curPf.Segments[1]).Points.Count; i++)
                                    {
                                        pls_bottom.Points.Add(new Point(((PolyLineSegment)curPf.Segments[1]).Points[i].X, ((PolyLineSegment)curPf.Segments[1]).Points[i].Y + rightWidth));
                                    }
                                    bottomPf.Segments.Add(pls_bottom);
                                }

                                LineSegment ls3_bottom = new LineSegment();
                                if (curPf.Segments[1] is LineSegment)
                                {
                                    Point forthPoint_bottom = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y);
                                    ls3_bottom.Point = forthPoint_bottom;
                                }
                                else if (curPf.Segments[1] is ArcSegment)
                                {
                                    Point forthPoint_bottom = new Point(((ArcSegment)curPf.Segments[1]).Point.X, ((ArcSegment)curPf.Segments[1]).Point.Y);
                                    ls3_bottom.Point = forthPoint_bottom;
                                }
                                else
                                {
                                    Point forthPoint_bottom = new Point(((PolyLineSegment)curPf.Segments[1]).Points[((PolyLineSegment)curPf.Segments[1]).Points.Count - 1].X, ((PolyLineSegment)curPf.Segments[1]).Points[((PolyLineSegment)curPf.Segments[1]).Points.Count - 1].Y);
                                    ls3_bottom.Point = forthPoint_bottom;
                                }
                                bottomPf.Segments.Add(ls3_bottom);

                                if (curPf.Segments[1] is LineSegment)
                                {
                                    LineSegment ls4_bottom = new LineSegment();
                                    ls4_bottom.Point = bottomPf.StartPoint;
                                    bottomPf.Segments.Add(ls4_bottom);
                                }
                                else if (curPf.Segments[1] is ArcSegment)
                                {
                                    ArcSegment ls4_bottom = new ArcSegment();
                                    Point forthPoint_bottom = bottomPf.StartPoint;
                                    ls4_bottom.Point = forthPoint_bottom;
                                    ls4_bottom.Size = ((ArcSegment)curPf.Segments[1]).Size;
                                    //ls4_bottom.SweepDirection = SweepDirection.Clockwise;
                                    if (((ArcSegment)curPf.Segments[1]).SweepDirection == SweepDirection.Clockwise)
                                    {
                                        ls4_bottom.SweepDirection = SweepDirection.Counterclockwise;
                                    }
                                    else
                                    {
                                        ls4_bottom.SweepDirection = SweepDirection.Clockwise;
                                    }
                                    bottomPf.Segments.Add(ls4_bottom);
                                }
                                else
                                {
                                    PolyLineSegment pls2_bottom = new PolyLineSegment();

                                    for (int i = ((PolyLineSegment)curPf.Segments[1]).Points.Count - 1; i >= 0; i--)
                                    {
                                        pls2_bottom.Points.Add(((PolyLineSegment)curPf.Segments[1]).Points[i]);
                                    }
                                    bottomPf.Segments.Add(pls2_bottom);
                                }
                                bottomPg.Figures.Add(bottomPf);

                                ColorProc.processWhenAddLayer_CC(this.canvas, this.centralCubePanel, (System.Windows.Shapes.Path)centralCubePanel.Children[centralCubePanel.Children.Count - 1], bottomPf, 1, (Color)ColorConverter.ConvertFromString("#FF787878"));

                                //将上下层路径添加到组中
                                geometryGroup.Children.Add(topPg);
                                geometryGroup.Children.Add(bottomPg);

                                ((Components)centralCubes[centralCubes.Count - 1]).newPath.Height += 2 * (leftWidth > rightWidth ? leftWidth : rightWidth);
                                ((Components)centralCubes[centralCubes.Count - 1]).height = ((Components)components[components.Count - 1]).newPath.Height;

                                autoResize();
                            }
                        }
                        else
                        {
                            MessageBox.Show("空文件！", "警告");
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("文件参数有误！", "警告");
            }
        }

        //
        public static byte[] Serializer(object obj)
        {
            IFormatter formatter = new BinaryFormatter();//定义BinaryFormatter以序列化object对象  
            MemoryStream ms = new MemoryStream();//创建内存流对象  
            formatter.Serialize(ms, obj);//把object对象序列化到内存流  
            byte[] buffer = ms.ToArray();//把内存流对象写入字节数组  

            ms.Close();//关闭内存流对象  
            ms.Dispose();//释放资源  

            MemoryStream msNew = new MemoryStream();
            GZipStream gzipStream = new GZipStream(msNew, CompressionMode.Compress, true);//创建压缩对象  
            gzipStream.Write(buffer, 0, buffer.Length);//把压缩后的数据写入文件  

            gzipStream.Close();//关闭压缩流,这里要注意：一定要关闭，要不然解压缩的时候会出现小于4K的文件读取不到数据，大于4K的文件读取不完整              
            gzipStream.Dispose();//释放对象  

            msNew.Close();
            msNew.Dispose();

            return msNew.ToArray();
        }

        /// <summary>  

        /// 反序列化压缩的object  

        /// </summary>  

        /// <param name="_filePath"></param>  

        /// <returns></returns>  

        public static object Deserialize(byte[] bytes)
        {
            MemoryStream msNew = new MemoryStream(bytes);
            msNew.Position = 0;
            GZipStream gzipStream = new GZipStream(msNew, CompressionMode.Decompress);//创建解压对象  
            byte[] buffer = new byte[4096];//定义数据缓冲  
            int offset = 0;//定义读取位置  

            MemoryStream ms = new MemoryStream();//定义内存流  
            while ((offset = gzipStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                ms.Write(buffer, 0, offset);//解压后的数据写入内存流  
            }

            BinaryFormatter sfFormatter = new BinaryFormatter();//定义BinaryFormatter以反序列化object对象  
            ms.Position = 0;//设置内存流的位置  
            object obj;
            try
            {
                obj = (object)sfFormatter.Deserialize(ms);//反序列化  
            }
            catch
            {
                throw;
            }
            finally
            {
                ms.Close();//关闭内存流  
                ms.Dispose();//释放资源  
            }
            gzipStream.Close();//关闭解压缩流  
            gzipStream.Dispose();//释放资源  
            msNew.Close();
            msNew.Dispose();

            return obj;
        }

        private void userDataBase(object sender, RoutedEventArgs e)
        {
            //DataBaseWindow dbw = new DataBaseWindow();
            //dbw.Show();
           
            MaterialDefinitionWindow mdw = new MaterialDefinitionWindow();
            if (DataBaseMaterials != null && DataBaseMaterials.Count != 0)
            {
                mdw.receivedMaterialFromMainWindow(DataBaseMaterials);
            }
            mdw.PassValuesEvent += new MaterialDefinitionWindow.PassValuesHandler(ReceiveValues);
            mdw.Show();
            
        }

        private void ReceiveValues(List<Dictionary<string, Dictionary<string, string>>> list)
        {
            DataBaseMaterials = list;
        }

        private double[] calcTotalSize() {
            double[] size = new double[2] { 0, 0};
            double height = 0;
            double width = 0;
            foreach (Components component in components) {
                GeometryGroup gg =  component.geometryGroup;
                if (height < gg.Bounds.Height) {
                    height = gg.Bounds.Height;
                }
                width += gg.Bounds.Width;
            }
            size[0] = width;
            size[1] = height;
            return size;
        }

        private void showTotalSizeOnCanvas() {
            double[] size = calcTotalSize();
            string text = "当前总宽度: " + size[0].ToString("f2") + "\n当前总高度: " + size[1].ToString("f2");
            TotalSizeTextBox.Text = text;
        }

        public void showEditLayer(int indexOfLayer, PathFigure pg, System.Windows.Shapes.Path path)
        {
            layerEdit le = new layerEdit();
            insertShape = path;
            changeTitle = 0;
            le.list = DataBaseMaterials;
            le.currentLayerNum = indexOfLayer;
            int index = stackpanel.Children.IndexOf(path);
            currentComp = (Components)components[index];
            curLayerNum = currentComp.layerNum;
            le.changeTitle = changeTitle;
            le.setComponent(currentComp);
            le.ChangeLayerSizeEvent += new ChangeLayerSizeHandler(autoResize);
            le.Show();
        }

        public void showEditLayer_CC(int indexOfLayer, PathFigure pg, System.Windows.Shapes.Path path)
        {
            changeTitle = 2;
            insertShape = path;
            layerEdit _le = new layerEdit();
            _le.list = DataBaseMaterials;
            _le.currentLayerNum = indexOfLayer;
            _le.changeTitle = changeTitle;
            int _index = centralCubePanel.Children.IndexOf(path);
            currentComp = (Components)centralCubes[_index];
            curLayerNum = currentComp.layerNum;
            _le.setComponent(currentComp);
            _le.ChangeLayerSizeEvent += new ChangeLayerSizeHandler(autoResize);
            _le.Show();
        }

        public System.Windows.Shapes.Path getInsertShape() {
            return insertShape;
        }

        public void setInsertShape(System.Windows.Shapes.Path p) {
            insertShape = p;
        }

        public void setCurLayerNum(int i) {
            curLayerNum = i;
        }

        public int getCurLayerNum()
        {
            return curLayerNum;
        }

        // 轻松衔接左端盖
        public void makeLeftCoverConnectSkillfully() {
            // 如果有左端盖
            if (isHaveLeftEndCap) {
                // 删除左端盖
                ContextMenu c = canvas.ContextMenu;
                MenuItem m = c.Items[0] as MenuItem;
                MenuItem mi_delete = m.Items[1] as MenuItem;
                // 原左端盖com
                Components originalLeftCoverCom = (Components)components[0];
                // 原左端盖的shape
                System.Windows.Shapes.Path originalLeftCover = (System.Windows.Shapes.Path)stackpanel.Children[0];
                RoutedEventArgs e1 = new RoutedEventArgs(MenuItem.ClickEvent);
                mi_delete.RaiseEvent(e1);

                // 添加新的左端盖
                MenuItem mi_add = m.Items[0] as MenuItem;
                RoutedEventArgs e2 = new RoutedEventArgs(MenuItem.ClickEvent);
                mi_add.RaiseEvent(e2);
                // 修改左端盖
                Components currentLeftCoverCom = (Components)components[0];
                editWindow ew = new editWindow();
                ew.setComponent(currentLeftCoverCom);
                ew.Owner = this;
                ew.changeTitle = 1;
                //ew.leftD.Text = originalLeftCoverCom.pg.Bounds.BottomLeft.Y - originalLeftCoverCom.pg.Bounds.TopLeft.Y + "";
                ew.rightD.Text = currentLeftCoverCom.pg.Bounds.BottomRight.Y - currentLeftCoverCom.pg.Bounds.TopRight.Y + "";
                ew.radiusText.Text = originalLeftCoverCom.radius.ToString();
                ew.segmentWidth.Text = originalLeftCoverCom.width.ToString();
                ew.isLeftEndCap = originalLeftCover.Fill.ToString();
                ew.isRightEndCap = originalLeftCover.Fill.ToString();
                ew.canvas = canvas;
                if (originalLeftCoverCom.isChangeOgive)
                {
                    ew.shape = "Ogive";
                }
                else if (originalLeftCoverCom.isChangeIOgive)
                {
                    ew.shape = "IOgive";
                }
                else {
                    ew.shape = "Cylinder";
                }
                ew.sp = stackpanel;
                ew.ChangeTextEvent += new ChangeTextHandler(autoResize);
                ew.ChangeShapeEvent += new ChangeShapeHandler(changeLineSegmentToArcSegment);
                ew.ChangeShapeEvent2 += new ChangeShapeHandler(changeLineSegmentToArcSegment);
                ew.ChangeShapeEvent3 += new ChangeShapeHandler(changeArcSegmentToLineSegment);
                System.Windows.Shapes.Path originalInsertShape = getInsertShape();
                int originalCurLayerNum = getCurLayerNum();
                setInsertShape(currentLeftCoverCom.newPath);
                setCurLayerNum(currentLeftCoverCom.layerNum);
                ew.OKButton_Click(null, null);
                setInsertShape(originalInsertShape);
                setCurLayerNum(originalCurLayerNum);
            }
        }

        public void makeRightCoverConnectSkillfully() {
            // 如果有右端盖
            if (isHaveRightEndCap)
            {
                // 删除右端盖
                ContextMenu c = canvas.ContextMenu;
                MenuItem m = c.Items[1] as MenuItem;
                MenuItem mi_delete = m.Items[1] as MenuItem;
                // 原右端盖com
                Components originalRightCoverCom = (Components)components[components.Count - 1];
                // 原右端盖的shape
                System.Windows.Shapes.Path originalRightCover = (System.Windows.Shapes.Path)stackpanel.Children[stackpanel.Children.Count - 1];
                RoutedEventArgs e1 = new RoutedEventArgs(MenuItem.ClickEvent);
                mi_delete.RaiseEvent(e1);

                // 添加新的右端盖
                MenuItem mi_add = m.Items[0] as MenuItem;
                RoutedEventArgs e2 = new RoutedEventArgs(MenuItem.ClickEvent);
                mi_add.RaiseEvent(e2);
                // 修改右端盖
                Components currentRightCoverCom = (Components)components[components.Count - 1];
                editWindow ew = new editWindow();
                ew.changeTitle = 1;
                ew.setComponent(currentRightCoverCom);
                ew.Owner = this;
                ew.leftD.Text = currentRightCoverCom.pg.Bounds.BottomLeft.Y - currentRightCoverCom.pg.Bounds.TopLeft.Y + "";
                //ew.rightD.Text = originalRightCoverCom.pg.Bounds.BottomRight.Y - originalRightCoverCom.pg.Bounds.TopRight.Y + "";
                ew.radiusText.Text = originalRightCoverCom.radius.ToString();
                ew.segmentWidth.Text = originalRightCoverCom.width.ToString();
                ew.isLeftEndCap = originalRightCover.Fill.ToString();
                ew.isRightEndCap = originalRightCover.Fill.ToString();

                ew.canvas = canvas;
                if (originalRightCoverCom.isChangeOgive)
                {
                    ew.shape = "Ogive";
                }
                else if (originalRightCoverCom.isChangeIOgive)
                {
                    ew.shape = "IOgive";
                }
                else
                {
                    ew.shape = "Cylinder";
                }
                ew.sp = stackpanel;
                ew.ChangeTextEvent += new ChangeTextHandler(autoResize);
                ew.ChangeShapeEvent += new ChangeShapeHandler(changeLineSegmentToArcSegment);
                ew.ChangeShapeEvent2 += new ChangeShapeHandler(changeLineSegmentToArcSegment);
                ew.ChangeShapeEvent3 += new ChangeShapeHandler(changeArcSegmentToLineSegment);
                System.Windows.Shapes.Path originalInsertShape = getInsertShape();
                int originalCurLayerNum = getCurLayerNum();
                setInsertShape(currentRightCoverCom.newPath);
                setCurLayerNum(currentRightCoverCom.layerNum);
                ew.OKButton_Click(null, null);
                setInsertShape(originalInsertShape);
                setCurLayerNum(originalCurLayerNum);
            }
        }
    }
}


