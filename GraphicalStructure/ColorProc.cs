using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Collections;
using System.Windows;

namespace GraphicalStructure
{
    class ColorProc
    {
        // 保存 层与cover的映射关系
        public static KeyValuePair<PathFigure, Shape> PfToCoverMap;
        public static KeyValuePair<Shape, PathFigure> CoverToPfMap;

        // 添加层时，在canvas上对应位置（绝对坐标）添加cover，并作相应处理
        public static void processWhenAddLayer(Canvas canvas, StackPanel curSp,Path curPath, PathFigure pf, int isTop)
        {
            //获取当前添加层的坐标
            ArrayList arr = new ArrayList(getPathFigureCoordinate(canvas, curSp, curPath, pf, isTop));
            //添加cover 
            if (isTop == 0)
            {
                Path newPath = new Path();
                // Create a blue and a black Brush
                Random random = new Random();
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromRgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                SolidColorBrush blackBrush = new SolidColorBrush();
                blackBrush.Color = Colors.Blue;

                newPath.Stroke = blackBrush;
                newPath.StrokeThickness = 1;
                newPath.Fill = brush;

                GeometryGroup geometryGroup = new GeometryGroup();
                geometryGroup.FillRule = FillRule.Nonzero;

                PathGeometry pg = new PathGeometry();
                PathFigure newpf = new PathFigure();
                newpf.StartPoint = new Point(((Point)arr[0]).X, ((Point)arr[0]).Y);

                if (pf.Segments[0] is LineSegment)
                {
                    LineSegment ls = new LineSegment();
                    ls.Point = new Point(((Point)arr[1]).X, ((Point)arr[1]).Y);
                    newpf.Segments.Add(ls);
                }
                else if (pf.Segments[0] is ArcSegment)
                {
                    ArcSegment ls = new ArcSegment();
                    ls.Point = new Point(((Point)arr[1]).X, ((Point)arr[1]).Y);
                    ls.Size = new Size(((ArcSegment)pf.Segments[0]).Size.Width, ((ArcSegment)pf.Segments[0]).Size.Height);
                    ls.SweepDirection = ((ArcSegment)pf.Segments[0]).SweepDirection;
                    newpf.Segments.Add(ls);
                }
                else
                {
                    PolyLineSegment ls = new PolyLineSegment();
                    for (int i = 0; i < ((PolyLineSegment)pf.Segments[0]).Points.Count - 1; i++)
                    {
                        double delta_x, delta_y;
                        delta_x = (((Point)arr[0]).X - pf.StartPoint.X);
                        delta_y = (((Point)arr[0]).Y - pf.StartPoint.Y);
                        ls.Points.Add(new Point(((PolyLineSegment)pf.Segments[0]).Points[i].X + delta_x, ((PolyLineSegment)pf.Segments[0]).Points[i].Y + delta_y));
                    }
                    newpf.Segments.Add(ls);
                }
                LineSegment ls2 = new LineSegment();
                ls2.Point = new Point(((Point)arr[2]).X, ((Point)arr[2]).Y);
                newpf.Segments.Add(ls2);

                if (pf.Segments[2] is LineSegment)
                {
                    LineSegment ls3 = new LineSegment();
                    ls3.Point = new Point(((Point)arr[3]).X, ((Point)arr[3]).Y);
                    newpf.Segments.Add(ls3);
                }
                else if (pf.Segments[2] is ArcSegment)
                {
                    ArcSegment ls3 = new ArcSegment();
                    ls3.Point = new Point(((Point)arr[3]).X, ((Point)arr[3]).Y);
                    ls3.Size = new Size(((ArcSegment)pf.Segments[2]).Size.Width, ((ArcSegment)pf.Segments[2]).Size.Height);
                    ls3.SweepDirection = ((ArcSegment)pf.Segments[2]).SweepDirection;
                    newpf.Segments.Add(ls3);
                }
                else
                {
                    PolyLineSegment ls3 = new PolyLineSegment();
                    for (int i = 0; i < ((PolyLineSegment)pf.Segments[2]).Points.Count - 1; i++)
                    {
                        double delta_x, delta_y;
                        delta_x = (((Point)arr[0]).X - pf.StartPoint.X);
                        delta_y = (((Point)arr[0]).Y - pf.StartPoint.Y);
                        ls3.Points.Add(new Point(((PolyLineSegment)pf.Segments[2]).Points[i].X + delta_x, ((PolyLineSegment)pf.Segments[2]).Points[i].Y + delta_y));
                    }
                    newpf.Segments.Add(ls3);
                }

                newpf.IsClosed = true;
                pg.Figures.Add(newpf);

                geometryGroup.Children.Add(pg);
                newPath.Data = geometryGroup;
                canvas.Children.Add(newPath);
            }
            else {
                Path newPath = new Path();
                // Create a blue and a black Brush
                Random random = new Random();
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromRgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                SolidColorBrush blackBrush = new SolidColorBrush();
                blackBrush.Color = Colors.Blue;

                newPath.Stroke = blackBrush;
                newPath.StrokeThickness = 1;
                newPath.Fill = brush;

                GeometryGroup geometryGroup = new GeometryGroup();
                geometryGroup.FillRule = FillRule.Nonzero;

                PathGeometry pg = new PathGeometry();
                PathFigure newpf = new PathFigure();
                newpf.StartPoint = new Point(((Point)arr[0]).X, ((Point)arr[0]).Y);

                LineSegment ls1 = new LineSegment();
                ls1.Point = new Point(((Point)arr[1]).X, ((Point)arr[1]).Y);
                newpf.Segments.Add(ls1);

                if (pf.Segments[1] is LineSegment)
                {
                    LineSegment ls2 = new LineSegment();
                    ls2.Point = new Point(((Point)arr[2]).X, ((Point)arr[2]).Y);
                    newpf.Segments.Add(ls2);
                }
                else if (pf.Segments[1] is ArcSegment)
                {
                    ArcSegment ls2 = new ArcSegment();
                    ls2.Point = new Point(((Point)arr[2]).X, ((Point)arr[2]).Y);
                    ls2.Size = new Size(((ArcSegment)pf.Segments[1]).Size.Width, ((ArcSegment)pf.Segments[1]).Size.Height);
                    ls2.SweepDirection = ((ArcSegment)pf.Segments[1]).SweepDirection;
                    newpf.Segments.Add(ls2);
                }
                else
                {
                    PolyLineSegment ls2 = new PolyLineSegment();
                    for (int i = 0; i < ((PolyLineSegment)pf.Segments[1]).Points.Count - 1; i++)
                    {
                        double delta_x, delta_y;
                        delta_x = (((Point)arr[0]).X - pf.StartPoint.X);
                        delta_y = (((Point)arr[0]).Y - pf.StartPoint.Y);
                        ls2.Points.Add(new Point(((PolyLineSegment)pf.Segments[1]).Points[i].X + delta_x, ((PolyLineSegment)pf.Segments[1]).Points[i].Y + delta_y));
                    }
                    newpf.Segments.Add(ls2);
                }
                LineSegment ls3 = new LineSegment();
                ls3.Point = new Point(((Point)arr[3]).X, ((Point)arr[3]).Y);
                newpf.Segments.Add(ls3);

                if (pf.Segments.Count < 4 || pf.Segments[3] is LineSegment )
                {
                    LineSegment ls4 = new LineSegment();
                    ls4.Point = new Point(((Point)arr[0]).X, ((Point)arr[0]).Y);
                    newpf.Segments.Add(ls4);
                }
                else if (pf.Segments[3] is ArcSegment)
                {
                    ArcSegment ls4 = new ArcSegment();
                    ls4.Point = new Point(((Point)arr[0]).X, ((Point)arr[0]).Y);
                    ls4.Size = new Size(((ArcSegment)pf.Segments[3]).Size.Width, ((ArcSegment)pf.Segments[3]).Size.Height);
                    ls4.SweepDirection = ((ArcSegment)pf.Segments[3]).SweepDirection;
                    newpf.Segments.Add(ls4);
                }
                else
                {
                    PolyLineSegment ls4 = new PolyLineSegment();
                    for (int i = 0; i < ((PolyLineSegment)pf.Segments[3]).Points.Count - 1; i++)
                    {
                        double delta_x, delta_y;
                        delta_x = (((Point)arr[0]).X - pf.StartPoint.X);
                        delta_y = (((Point)arr[0]).Y - pf.StartPoint.Y);
                        ls4.Points.Add(new Point(((PolyLineSegment)pf.Segments[3]).Points[i].X + delta_x, ((PolyLineSegment)pf.Segments[3]).Points[i].Y + delta_y));
                    }
                    newpf.Segments.Add(ls4);
                }

                newpf.IsClosed = true;
                pg.Figures.Add(newpf);

                geometryGroup.Children.Add(pg);
                newPath.Data = geometryGroup;
                canvas.Children.Add(newPath);
            }
           
        }

        //获取PF对应的坐标
        private static ArrayList getPathFigureCoordinate(Canvas canvas, StackPanel curSp, Path curPath, PathFigure pf, int isTop)
        {
            double getSpLeftDisToCan = Canvas.GetLeft(curSp);
            double getSpTopDisToCan = Canvas.GetTop(curSp);
            double y1, y2, y3, y4;
            if (isTop == 0)
            {
                //上层
                y1 = pf.StartPoint.Y;
                if (pf.Segments[0] is LineSegment)
                {
                    y2 = ((LineSegment)pf.Segments[0]).Point.Y;
                }
                else if (pf.Segments[0] is ArcSegment)
                {
                    y2 = ((ArcSegment)pf.Segments[0]).Point.Y;
                }
                else
                {
                    y2 = (((PolyLineSegment)pf.Segments[0]).Points[((PolyLineSegment)pf.Segments[0]).Points.Count - 1]).Y;
                }

                y3 = ((LineSegment)pf.Segments[1]).Point.Y;

                if (pf.Segments[2] is LineSegment)
                {
                    y4 = ((LineSegment)pf.Segments[2]).Point.Y;
                }
                else if (pf.Segments[2] is ArcSegment)
                {
                    y4 = ((ArcSegment)pf.Segments[2]).Point.Y;
                }
                else
                {
                    y4 = (((PolyLineSegment)pf.Segments[2]).Points[((PolyLineSegment)pf.Segments[2]).Points.Count - 1]).Y;
                }

                if (Math.Abs(y4 - y1) <= Math.Abs(y2 - y3)) {
                    getSpTopDisToCan -= Math.Abs(y2 - y3);
                }
                else {
                    getSpTopDisToCan -= Math.Abs(y4 - y1);
                }

                double getPathLeftDisToSp = 0;
                //double getPathTopDisToSp = (curSp.Height - (curPath.Height - 200))/2;
                int index = curSp.Children.IndexOf(curPath);
                if (index > 0)
                {
                    for (int i = 0; i < index; i++)
                    {
                        Path path = curSp.Children[i] as Path;
                        getPathLeftDisToSp += path.Width;
                    }
                }

                double left = getSpLeftDisToCan + getPathLeftDisToSp;
                double top = getSpTopDisToCan;// +getPathTopDisToSp;

                Point p1 = new Point(pf.StartPoint.X + left, pf.StartPoint.Y + top);
                Point p2;
                if (pf.Segments[0] is LineSegment)
                {
                    p2 = new Point(((LineSegment)pf.Segments[0]).Point.X + left, ((LineSegment)pf.Segments[0]).Point.Y + top);
                }
                else if (pf.Segments[0] is ArcSegment)
                {
                    p2 = new Point(((ArcSegment)pf.Segments[0]).Point.X + left, ((ArcSegment)pf.Segments[0]).Point.Y + top);
                }
                else
                {
                    p2 = new Point((((PolyLineSegment)pf.Segments[0]).Points[((PolyLineSegment)pf.Segments[0]).Points.Count - 1]).X + left, (((PolyLineSegment)pf.Segments[0]).Points[((PolyLineSegment)pf.Segments[0]).Points.Count - 1]).Y + top);
                }
                Point p3 = new Point(((LineSegment)pf.Segments[1]).Point.X + left, ((LineSegment)pf.Segments[1]).Point.Y + top);
                Point p4;
                if (pf.Segments[0] is LineSegment)
                {
                    p4 = new Point(((LineSegment)pf.Segments[2]).Point.X + left, ((LineSegment)pf.Segments[2]).Point.Y + top);
                }
                else if (pf.Segments[0] is ArcSegment)
                {
                    p4 = new Point(((ArcSegment)pf.Segments[2]).Point.X + left, ((ArcSegment)pf.Segments[2]).Point.Y + top);
                }
                else
                {
                    p4 = new Point((((PolyLineSegment)pf.Segments[2]).Points[((PolyLineSegment)pf.Segments[2]).Points.Count - 1]).X + left, (((PolyLineSegment)pf.Segments[2]).Points[((PolyLineSegment)pf.Segments[2]).Points.Count - 1]).Y + top);
                }
                ArrayList arrList = new ArrayList();
                arrList.Add(p1);
                arrList.Add(p2);
                arrList.Add(p3);
                arrList.Add(p4);

                return arrList;
            }
            else {
                //下层
                y1 = pf.StartPoint.Y;
                y2 = ((LineSegment)pf.Segments[0]).Point.Y;
                if (pf.Segments[1] is LineSegment)
                {
                    y3 = ((LineSegment)pf.Segments[1]).Point.Y;
                }
                else if (pf.Segments[1] is ArcSegment)
                {
                    y3 = ((ArcSegment)pf.Segments[1]).Point.Y;
                }
                else
                {
                    y3 = (((PolyLineSegment)pf.Segments[1]).Points[((PolyLineSegment)pf.Segments[1]).Points.Count - 1]).Y;
                }

                y4 = ((LineSegment)pf.Segments[2]).Point.Y;
               
                if (Math.Abs(y2 - y1) <= Math.Abs(y3 - y4))
                {
                    getSpTopDisToCan -= Math.Abs(y3 - y4);
                }
                else
                {
                    getSpTopDisToCan -= Math.Abs(y2 - y1);
                }
                double getPathLeftDisToSp = 0;
                //double getPathTopDisToSp = (curSp.Height - (curPath.Height - 200))/2;
                int index = curSp.Children.IndexOf(curPath);
                if (index > 0)
                {
                    for (int i = 0; i < index; i++)
                    {
                        Path path = curSp.Children[i] as Path;
                        getPathLeftDisToSp += path.Width;
                    }
                }

                double left = getSpLeftDisToCan + getPathLeftDisToSp;
                double top = getSpTopDisToCan;// +getPathTopDisToSp;

                Point p1 = new Point(pf.StartPoint.X + left, pf.StartPoint.Y + top);
                Point p2 = new Point(((LineSegment)pf.Segments[0]).Point.X + left, ((LineSegment)pf.Segments[0]).Point.Y + top);
                Point p3;
                if (pf.Segments[1] is LineSegment)
                {
                    p3 = new Point(((LineSegment)pf.Segments[1]).Point.X + left, ((LineSegment)pf.Segments[1]).Point.Y + top);
                }
                else if (pf.Segments[1] is ArcSegment)
                {
                    p3 = new Point(((ArcSegment)pf.Segments[1]).Point.X + left, ((ArcSegment)pf.Segments[1]).Point.Y + top);
                }
                else
                {
                    p3 = new Point((((PolyLineSegment)pf.Segments[1]).Points[((PolyLineSegment)pf.Segments[1]).Points.Count - 1]).X + left, (((PolyLineSegment)pf.Segments[1]).Points[((PolyLineSegment)pf.Segments[1]).Points.Count - 1]).Y + top);
                }
                Point p4 = new Point(((LineSegment)pf.Segments[2]).Point.X + left, ((LineSegment)pf.Segments[2]).Point.Y + top);
                
                ArrayList arrList = new ArrayList();
                arrList.Add(p1);
                arrList.Add(p2);
                arrList.Add(p3);
                arrList.Add(p4);

                return arrList;
            }
        }

        // 删除层时，删除cover， 并作相应处理
        public static void processWhenDelLayer()
        {

        }

        // 移动层时，同时移动cover， 并作相应处理
        public static void processWhenMoveLayer()
        {

        }

        // 真正的层点击更改颜色后，改变cover的颜色，并作相应处理
        public static void processWhenChangeLayerColor()
        {

        }

        // cover 的双击事件回调,传递到真正的层上，手动触发真正的层的双击事件
        public static void doubleClickOnCover()
        {

        }
    }
}
