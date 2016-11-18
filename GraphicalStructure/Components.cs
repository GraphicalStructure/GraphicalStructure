using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections;
using System.Windows.Markup;

namespace GraphicalStructure
{
    public class Components : UIElement
    {
        // Component's width & height
        public double width;
        public double height;

        public bool isLeftCover;
        public bool isRightCover;

        // The first point of component   anti-clockwise 逆时针
        public Point startPoint;
        public Point point2;
        public Point point3;
        public Point point4;

        //路径几何体
        public PathGeometry pg;
        public PathFigure pf;
        public LineSegment ls;
        public LineSegment ls2;
        public LineSegment ls3;
        public LineSegment ls4;
        public Path newPath;

        public int layerNum = 0;

        //多条路径组成复合几何体
        public GeometryGroup geometryGroup;

        //层的一些属性
        public ArrayList layerNums;        //层号
        public ArrayList layerType;        //层类型
        public ArrayList layerMaterial;    //层材料
        public Hashtable layerSize;
        public ArrayList layerLeftThickness;
        public ArrayList layerRightThickness;

        public bool isChangeOgive = false;
        public bool isChangeIOgive = false;
        public double radius;

        //空心管的偏移量
        public double cubeOffset = 0;

        //public Components deepCopy(Components c)
        //{
        //    this.width = c.width;
        //    this.height = c.height;

        //    this.startPoint = c.startPoint;
        //    this.point2 = c.point2;
        //    this.point3 = c.point3;
        //    this.point4 = c.point4;

        //    string xaml = XamlWriter.Save(c.newPath);

        //    if (xaml != null)
        //    {
        //        using (System.IO.MemoryStream stream = new System.IO.MemoryStream(xaml.Length))
        //        {
        //            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(stream))
        //            {
        //                sw.Write(xaml);
        //                sw.Flush();
        //                stream.Seek(0, System.IO.SeekOrigin.Begin);
        //                UIElement clonePath = XamlReader.Load(stream) as UIElement;
        //                this.newPath = (Path)clonePath;
        //            }
        //        }
        //    }

        //    return this;
        //}

        public void setLs2(Point point){
            this.ls2.Point = new Point(point.X, point.Y);
        }

        public Components()
        {
            width = 0;
            height = 0;

            startPoint = new Point(0,0);
            point2 = new Point(0,0);
            point3= new Point(0,0);
            point4= new Point(0,0);

            // Create a blue and a black Brush
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Colors.Black;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Blue;

            // Create a Path with black brush and blue fill
            newPath = new Path();
            newPath.Stroke = blackBrush;
            newPath.StrokeThickness = 1;
            newPath.Fill = blueBrush;

            geometryGroup = new GeometryGroup();
            geometryGroup.FillRule = FillRule.Nonzero;

            pg = new PathGeometry();

            pf = new PathFigure();
            pf.StartPoint = startPoint;

            //初始化直线
            ls = new LineSegment();
            ls.Point = point2;
            pf.Segments.Add(ls);

            ls2 = new LineSegment();
            ls2.Point = point3;
            pf.Segments.Add(ls2);

            ls3 = new LineSegment();
            ls3.Point = point4;
            pf.Segments.Add(ls3);

            ls4 = new LineSegment();
            ls4.Point = startPoint;
            pf.Segments.Add(ls4);

            pf.IsClosed = true;
            pg.Figures.Add(pf);

            geometryGroup.Children.Add(pg);

            // Set Path.Data
            newPath.Data = geometryGroup;
            newPath.Width = width;
            newPath.Height = height + 100;

            //设置层的属性
            layerNums = new ArrayList();
            layerType = new ArrayList();
            layerMaterial = new ArrayList();
            layerSize = new Hashtable();
            layerLeftThickness = new ArrayList();
            layerRightThickness = new ArrayList();
        }

        public Components(Point startP, Point p2, Point p3, Point p4)
        {
            //属性赋值
            this.width = p4.X - startP.X;
            double h1 = Math.Abs(startP.Y - p2.Y);
            double h2 = Math.Abs(p4.Y - p3.Y);
            double maxHeight = h1 > h2 ? h1 : h2;
            if ((p2.Y > p3.Y ? p2.Y : p3.Y) > maxHeight)
            {
                this.height = (p2.Y > p3.Y ? p2.Y : p3.Y);
            }
            else
            {
                this.height = maxHeight;
            }

            this.startPoint = startP;
            this.point2 = p2;
            this.point3 = p3;
            this.point4 = p4;

            // Create a blue and a black Brush
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Colors.Black;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Blue;

            // Create a Path with black brush and blue fill
            newPath = new Path();
            newPath.Stroke = blackBrush;
            newPath.StrokeThickness = 1;
            newPath.Fill = blueBrush;

            geometryGroup = new GeometryGroup();
            geometryGroup.FillRule = FillRule.EvenOdd;

            pg = new PathGeometry();

            pf = new PathFigure();
            pf.StartPoint = startP;
            
            //初始化直线
            ls = new LineSegment();
            ls.Point = p2;
            pf.Segments.Add(ls);

            ls2 = new LineSegment();
            ls2.Point = p3;
            pf.Segments.Add(ls2);

            ls3 = new LineSegment();
            ls3.Point = p4;
            pf.Segments.Add(ls3);
            
            ls4 = new LineSegment();
            ls4.Point = startPoint;
            pf.Segments.Add(ls4);

            pf.IsClosed = true;
            pg.Figures.Add(pf);

            geometryGroup.Children.Add(pg);

            // Set Path.Data
            newPath.Data = geometryGroup;
            newPath.Width = width;
            newPath.Height = height + 100;

            //设置层的属性
            layerNums = new ArrayList();
            layerType = new ArrayList();
            layerMaterial = new ArrayList();
            layerSize = new Hashtable();
            layerLeftThickness = new ArrayList();
            layerRightThickness = new ArrayList();
        }
    }
}
