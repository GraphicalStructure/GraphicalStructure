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
        public static void processWhenAddLayer(Canvas canvas, StackPanel curSp,Path curPath, PathFigure pf)
        {
            //获取当前添加层的坐标
            ArrayList arr = new ArrayList(getPathFigureCoordinate(canvas, curSp, curPath, pf));
            //添加cover 
            Path newPath = new Path();
            // Create a blue and a black Brush
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromRgb(255, 255, 255);
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
                newpf.Segments.Add(ls);
            }
            else
            {
                PolyLineSegment ls = new PolyLineSegment();
                for (int i = 0; i < ((PolyLineSegment)pf.Segments[0]).Points.Count - 1; i++)
                {
                    ls.Points.Add(((PolyLineSegment)pf.Segments[0]).Points[i]);
                }
                newpf.Segments.Add(ls);
            }
            LineSegment ls2 = new LineSegment();
            ls2.Point = new Point(((Point)arr[2]).X, ((Point)arr[2]).Y);
            newpf.Segments.Add(ls2);

            if (pf.Segments[0] is LineSegment)
            {
                LineSegment ls3 = new LineSegment();
                ls3.Point = new Point(((Point)arr[3]).X, ((Point)arr[3]).Y);
                newpf.Segments.Add(ls3);
            }
            else if (pf.Segments[0] is ArcSegment)
            {
                ArcSegment ls3 = new ArcSegment();
                ls3.Point = new Point(((Point)arr[3]).X, ((Point)arr[3]).Y);
                newpf.Segments.Add(ls3);
            }
            else
            {
                PolyLineSegment ls3 = new PolyLineSegment();
                for (int i = 0; i < ((PolyLineSegment)pf.Segments[0]).Points.Count - 1; i++)
                {
                    ls3.Points.Add(((PolyLineSegment)pf.Segments[0]).Points[i]);
                }
                newpf.Segments.Add(ls3);
            }

            newpf.IsClosed = true;
            pg.Figures.Add(newpf);

            geometryGroup.Children.Add(pg);
            newPath.Data = geometryGroup;
            canvas.Children.Add(newPath);
        }

        //获取PF对应的坐标
        public static ArrayList getPathFigureCoordinate(Canvas canvas, StackPanel curSp, Path curPath, PathFigure pf)
        {
            double getSpLeftDisToCan = Canvas.GetLeft(curSp);
            double getSpTopDisToCan = Canvas.GetTop(curSp);

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
