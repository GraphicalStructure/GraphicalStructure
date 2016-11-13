using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace GraphicalStructure
{
    interface IMainWindow
    {
        void showEditLayer(int indexOfLayer, PathFigure pg, Path path);

        void autoResize();

        void changeLineSegmentToArcSegment(double radius, int isConvex);

        void changeLineSegmentToPolySegmentForLayer(double radius, int isConvex);

        void changeArcSegmentToLineSegment(double a, int b);

        Path getInsertShape();

        void setInsertShape(Path p);

        void setCurLayerNum(int i);

        int getCurLayerNum();

        void makeLeftCoverConnectSkillfully();

        void makeRightCoverConnectSkillfully();

        /**
         * 返回可能的两个圆心, p1 p2无所谓顺序
         */
        Point[] calcuCentralPoints(Point p1, Point p2, double r);

        /*
         * 根据圆心，半径，x区域，点的个数，得到一系列点,p1
         * p1终点，p2为起始点
         */
        PointCollection findPolyPointsByCircle(Point p0, double r, Point p1, Point p2, int num, int isConvex);

        /*
         * 根据上层圆弧上的点对称求下层的点
         * y_mid 为中间点y坐标
         */
        PointCollection getSymmetricPoint(PointCollection pointsUp, double y_mid);

        /**
         * reverse PointCollection
         */
        PointCollection reversePointCollection(PointCollection pointCollection);
    }
}
