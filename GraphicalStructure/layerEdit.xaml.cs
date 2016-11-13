using System;
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
using System.Windows.Shapes;
using System.Collections;
using System.Data;

namespace GraphicalStructure
{
    /// <summary>
    /// layerEdit.xaml 的交互逻辑
    /// </summary>
    /// 

    public delegate void ChangeLayerSizeHandler();

    public partial class layerEdit : Window
    {
        public Components currentCom;
        private double leftLength;
        private double rightLength;

        public int currentLayerNum = 1;

        public double left_X;
        public double pathWidth;

        public event ChangeLayerSizeHandler ChangeLayerSizeEvent;

        private static UseAccessDB accessDb;

        public DataSet ds;
        public DataTable dt;

        public bool flag;

        public string lastRadioButton;
        public string lastMaterial;
        public string lastDiameter;
        public string lastLongLength;
        public string lastWidth;
        public string lastHeight;
        public string lastObliqueAngle;
        private static IMainWindow mainWindow;

        public List<Dictionary<string, Dictionary<string, string>>> list;

        private layerEdit(IMainWindow imw)
        {
            mainWindow = imw;
            InitializeComponent();
            flag = false;
            list = new List<Dictionary<string, Dictionary<string, string>>>();
        }

        public layerEdit()
        {
            InitializeComponent();

            flag = false;

            list = new List<Dictionary<string, Dictionary<string, string>>>();
        }

        public void setComponent(Components c)
        {
            this.currentCom = c;

            showComponentInfo();
        }

        public void showComponentInfo()
        {
            ArrayList material = new ArrayList();
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Dictionary<string, string> dict = ((Dictionary<string, Dictionary<string, string>>)list[i])["matName"];
                    Dictionary<string, string> dic = ((Dictionary<string, Dictionary<string, string>>)list[i])["materialName"];
                    if (dict["content"] == "JOHNSON_COOK" || dict["content"] == "PLASTIC_KINEMATIC")
                    {
                        string materialName = dic["content"];
                        material.Add(materialName);
                    }
                }
                materialBox.ItemsSource = material;
            }

            //显示当前尺寸大小
            if (this.currentCom.layerNum != 0)
            {
                //显示当前层号
                layerNums.ItemsSource = this.currentCom.layerNums;
                layerNums.SelectedIndex = currentLayerNum - 1;

                int layerN = Int32.Parse(layerNums.SelectedItem.ToString());

                /*
                //显示当前材料
                accessDb = new UseAccessDB();
                accessDb.getConnection();
                ds = accessDb.SelectToDataSet("select * from material", "material");
                dt = ds.Tables[0];
                ArrayList col = new ArrayList();
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine(row[dt.Columns[1]]);
                    col.Add(row[dt.Columns[1]]);

                    if (currentCom.layerMaterial[layerN - 1].ToString() == row[dt.Columns[1]].ToString ())
                    {
                        materialBox.SelectedItem = row[dt.Columns[1]];
                    }
                }
                materialBox.ItemsSource = col;
                */

                if (list != null)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)list[j])["color"];
                        Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)list[j])["materialName"];
                        string mColor = cdic["content"];
                        string materialName = mdic["content"];
                        if (materialName == currentCom.layerMaterial[layerN - 1].ToString())
                        {
                            materialBox.SelectedItem = materialName;
                        }
                    }
                }

                if (currentCom.layerType[layerN - 1].ToString() == "球")
                {
                    ball.IsChecked = true;
                }
                else if (currentCom.layerType[layerN - 1].ToString() == "柱")
                {
                    column.IsChecked = true;
                }
                else if (currentCom.layerType[layerN - 1].ToString() == "杆")
                {
                    pole.IsChecked = true;
                }
                else if (currentCom.layerType[layerN - 1].ToString() == "长方体")
                {
                    cube.IsChecked = true;
                }
                else if (currentCom.layerType[layerN - 1].ToString() == "结构")
                {
                    structure.IsChecked = true;
                }
                radioButtonChanged();

                string key = layerNums.SelectedItem.ToString();
                diameter.Text = ((Hashtable)this.currentCom.layerSize[layerNums.SelectedItem])["diameter"].ToString();
                longLength.Text = ((Hashtable)this.currentCom.layerSize[layerNums.SelectedItem])["longLength"].ToString();
                width.Text = ((Hashtable)this.currentCom.layerSize[layerNums.SelectedItem])["width"].ToString();
                height.Text = ((Hashtable)this.currentCom.layerSize[layerNums.SelectedItem])["height"].ToString();
                ObliqueAngle.Text = ((Hashtable)this.currentCom.layerSize[layerNums.SelectedItem])["ObliqueAngle"].ToString();

                leftSize.Text = this.currentCom.layerLeftThickness[layerN-1].ToString();
                rightSize.Text = this.currentCom.layerRightThickness[layerN-1].ToString();

                lastDiameter = diameter.Text;
                lastLongLength = longLength.Text;
                lastWidth = width.Text;
                lastHeight = height.Text;
                lastObliqueAngle = ObliqueAngle.Text;
            }
            else
            {
                layerNums.SelectedItem = "0";

                diameter.Text = "0";
                longLength.Text = "0";
                width.Text = "0";
                height.Text = "0";
                ObliqueAngle.Text = "0";
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            leftSizeChange();
            rightSizeChange();

            diameterChange();

            saveRadioButtonData(lastRadioButton);
            saveMaterialData(lastMaterial);
            saveLayerSizeData();

            if (ChangeLayerSizeEvent != null)
            {
                ChangeLayerSizeEvent();
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void diameterChange()
        {
            if(lastDiameter != diameter.Text)
            {
                leftSize.Text = diameter.Text;
                rightSize.Text = diameter.Text;

                leftSizeChange();
                rightSizeChange();
            }
        }

        public void leftSizeChange()
        {
            try
            {
                //判断当前层号是奇偶数？  要改一对。。。
                int layerNumberOne = Int32.Parse(layerNums.SelectedItem.ToString());
                int layerNumberTwo;
                double originLeftSize;
                if (layerNumberOne % 2 == 0)
                {
                    layerNumberTwo = layerNumberOne - 1;

                    GeometryGroup geometryGroup = (GeometryGroup)currentCom.newPath.Data;
                    PathGeometry t_curPg = (PathGeometry)geometryGroup.Children[layerNumberTwo];
                    PathFigure t_curPf = t_curPg.Figures.ElementAt(0);
                    PathGeometry b_curPg = (PathGeometry)geometryGroup.Children[layerNumberOne];
                    PathFigure b_curPf = b_curPg.Figures.ElementAt(0);

                    originLeftSize = t_curPf.StartPoint.Y - ((LineSegment)t_curPf.Segments[2]).Point.Y;
                    double changeValue;
                    if (originLeftSize - Double.Parse(leftSize.Text) > 0)
                    {
                        //厚度变小
                        changeValue = originLeftSize - Double.Parse(leftSize.Text);

                        ((LineSegment)t_curPf.Segments[2]).Point = new Point(((LineSegment)t_curPf.Segments[2]).Point.X, ((LineSegment)t_curPf.Segments[2]).Point.Y + changeValue);
                        ((LineSegment)b_curPf.Segments[0]).Point = new Point(((LineSegment)b_curPf.Segments[0]).Point.X, ((LineSegment)b_curPf.Segments[0]).Point.Y - changeValue);

                        //判断是否有在修改本层之上的层
                        if (geometryGroup.Children.Count - 1 > layerNumberOne)
                        {
                            //有
                            for (int i = layerNumberOne + 1; i < geometryGroup.Children.Count; i += 2)
                            {
                                PathGeometry tcurPg = (PathGeometry)geometryGroup.Children[i];
                                PathFigure tcurPf = tcurPg.Figures.ElementAt(0);
                                tcurPf.StartPoint = new Point(tcurPf.StartPoint.X, tcurPf.StartPoint.Y + changeValue);
                                ((LineSegment)tcurPf.Segments[2]).Point = new Point(((LineSegment)tcurPf.Segments[2]).Point.X, ((LineSegment)tcurPf.Segments[2]).Point.Y + changeValue);

                                PathGeometry bcurPg = (PathGeometry)geometryGroup.Children[i + 1];
                                PathFigure bcurPf = bcurPg.Figures.ElementAt(0);
                                bcurPf.StartPoint = new Point(bcurPf.StartPoint.X, bcurPf.StartPoint.Y - changeValue);
                                ((LineSegment)bcurPf.Segments[0]).Point = new Point(((LineSegment)bcurPf.Segments[0]).Point.X, ((LineSegment)bcurPf.Segments[0]).Point.Y - changeValue);
                            }
                        }
                    }
                    else
                    {
                        //厚度变大
                        changeValue = Double.Parse(leftSize.Text) - originLeftSize;

                        //currentCom.newPath.Height += changeValue * 2;
                        //currentCom.height = currentCom.newPath.Height;

                        ((LineSegment)t_curPf.Segments[2]).Point = new Point(((LineSegment)t_curPf.Segments[2]).Point.X, ((LineSegment)t_curPf.Segments[2]).Point.Y - changeValue);
                        ((LineSegment)b_curPf.Segments[0]).Point = new Point(((LineSegment)b_curPf.Segments[0]).Point.X, ((LineSegment)b_curPf.Segments[0]).Point.Y + changeValue);

                        PathGeometry tempcurPg = (PathGeometry)geometryGroup.Children[geometryGroup.Children.Count - 2];
                        PathFigure tempcurPf = tempcurPg.Figures.ElementAt(0);

                        //整体的高度调节？？？？
                        //如果变换的高度与之前最高层Y距离顶部的距离相同，则整体的高度会变小
                        //if (Math.Abs(((LineSegment)tempcurPf.Segments[2]).Point.Y) == changeValue)
                        //{
                        //    currentCom.newPath.Height -= changeValue * 2;
                        //    currentCom.height = currentCom.newPath.Height;
                        //}

                        //判断是否有在修改本层之上的层
                        if (geometryGroup.Children.Count - 1 > layerNumberOne)
                        {
                            //有
                            for (int i = layerNumberOne + 1; i < geometryGroup.Children.Count; i += 2)
                            {
                                PathGeometry tcurPg = (PathGeometry)geometryGroup.Children[i];
                                PathFigure tcurPf = tcurPg.Figures.ElementAt(0);
                                tcurPf.StartPoint = new Point(tcurPf.StartPoint.X, tcurPf.StartPoint.Y - changeValue);
                                ((LineSegment)tcurPf.Segments[2]).Point = new Point(((LineSegment)tcurPf.Segments[2]).Point.X, ((LineSegment)tcurPf.Segments[2]).Point.Y - changeValue);

                                PathGeometry bcurPg = (PathGeometry)geometryGroup.Children[i + 1];
                                PathFigure bcurPf = bcurPg.Figures.ElementAt(0);
                                bcurPf.StartPoint = new Point(bcurPf.StartPoint.X, bcurPf.StartPoint.Y + changeValue);
                                ((LineSegment)bcurPf.Segments[0]).Point = new Point(((LineSegment)bcurPf.Segments[0]).Point.X, ((LineSegment)bcurPf.Segments[0]).Point.Y + changeValue);
                            }
                        }

                        //如果最高层的Y不为负就不用向下移动
                        if (((LineSegment)tempcurPf.Segments[2]).Point.Y < 0)
                        {
                            double temp = Math.Abs(((LineSegment)tempcurPf.Segments[2]).Point.Y);
                            for (int i = 0; i < geometryGroup.Children.Count; i++)
                            {
                                PathGeometry curPg = (PathGeometry)geometryGroup.Children[i];
                                PathFigure curPf = curPg.Figures.ElementAt(0);
                                curPf.StartPoint = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + temp);
                                ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + temp);
                                ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + temp);
                                ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + temp);
                            }
                            currentCom.newPath.Height += temp * 2;
                            currentCom.height = currentCom.newPath.Height;

                        }
                    }
                    if (changeValue != 0)
                    {
                        ColorProc.processWhenChangeLayerHeight(b_curPf, 0, 1);
                        ColorProc.processWhenChangeLayerHeight(t_curPf, 0, 0);
                    }

                }
                else
                {
                    layerNumberTwo = layerNumberOne + 1;

                    GeometryGroup geometryGroup = (GeometryGroup)currentCom.newPath.Data;
                    PathGeometry t_curPg = (PathGeometry)geometryGroup.Children[layerNumberOne];
                    PathFigure t_curPf = t_curPg.Figures.ElementAt(0);
                    PathGeometry b_curPg = (PathGeometry)geometryGroup.Children[layerNumberTwo];
                    PathFigure b_curPf = b_curPg.Figures.ElementAt(0);

                    originLeftSize = t_curPf.StartPoint.Y - ((LineSegment)t_curPf.Segments[2]).Point.Y;
                    double changeValue;
                    if (originLeftSize - Double.Parse(leftSize.Text) > 0)
                    {
                        //厚度变小
                        changeValue = originLeftSize - Double.Parse(leftSize.Text);

                        ((LineSegment)t_curPf.Segments[2]).Point = new Point(((LineSegment)t_curPf.Segments[2]).Point.X, ((LineSegment)t_curPf.Segments[2]).Point.Y + changeValue);
                        ((LineSegment)b_curPf.Segments[0]).Point = new Point(((LineSegment)b_curPf.Segments[0]).Point.X, ((LineSegment)b_curPf.Segments[0]).Point.Y - changeValue);

                        //判断是否有在修改本层之上的层
                        if (geometryGroup.Children.Count - 1 > layerNumberTwo)
                        {
                            //有
                            for (int i = layerNumberTwo + 1; i < geometryGroup.Children.Count; i += 2)
                            {
                                PathGeometry tcurPg = (PathGeometry)geometryGroup.Children[i];
                                PathFigure tcurPf = tcurPg.Figures.ElementAt(0);
                                tcurPf.StartPoint = new Point(tcurPf.StartPoint.X, tcurPf.StartPoint.Y + changeValue);
                                ((LineSegment)tcurPf.Segments[2]).Point = new Point(((LineSegment)tcurPf.Segments[2]).Point.X, ((LineSegment)tcurPf.Segments[2]).Point.Y + changeValue);

                                PathGeometry bcurPg = (PathGeometry)geometryGroup.Children[i + 1];
                                PathFigure bcurPf = bcurPg.Figures.ElementAt(0);
                                bcurPf.StartPoint = new Point(bcurPf.StartPoint.X, bcurPf.StartPoint.Y - changeValue);
                                ((LineSegment)bcurPf.Segments[0]).Point = new Point(((LineSegment)bcurPf.Segments[0]).Point.X, ((LineSegment)bcurPf.Segments[0]).Point.Y - changeValue);
                            }
                        }
                    }
                    else
                    {
                        //厚度变大
                        changeValue = Double.Parse(leftSize.Text) - originLeftSize;

                        //currentCom.newPath.Height += changeValue * 2;
                        //currentCom.height = currentCom.newPath.Height;

                        ((LineSegment)t_curPf.Segments[2]).Point = new Point(((LineSegment)t_curPf.Segments[2]).Point.X, ((LineSegment)t_curPf.Segments[2]).Point.Y - changeValue);
                        ((LineSegment)b_curPf.Segments[0]).Point = new Point(((LineSegment)b_curPf.Segments[0]).Point.X, ((LineSegment)b_curPf.Segments[0]).Point.Y + changeValue);

                        PathGeometry tempcurPg = (PathGeometry)geometryGroup.Children[geometryGroup.Children.Count - 2];
                        PathFigure tempcurPf = tempcurPg.Figures.ElementAt(0);

                        //如果变换的高度与之前最高层Y距离顶部的距离相同，则整体的高度会变小
                        //if (Math.Abs(((LineSegment)tempcurPf.Segments[2]).Point.Y) == changeValue)
                        //{
                        //    currentCom.newPath.Height -= changeValue * 2;
                        //    currentCom.height = currentCom.newPath.Height;
                        //}


                        //判断是否有在修改本层之上的层
                        if (geometryGroup.Children.Count - 1 > layerNumberTwo)
                        {
                            //有
                            for (int i = layerNumberTwo + 1; i < geometryGroup.Children.Count; i += 2)
                            {
                                PathGeometry tcurPg = (PathGeometry)geometryGroup.Children[i];
                                PathFigure tcurPf = tcurPg.Figures.ElementAt(0);
                                tcurPf.StartPoint = new Point(tcurPf.StartPoint.X, tcurPf.StartPoint.Y - changeValue);
                                ((LineSegment)tcurPf.Segments[2]).Point = new Point(((LineSegment)tcurPf.Segments[2]).Point.X, ((LineSegment)tcurPf.Segments[2]).Point.Y - changeValue);

                                PathGeometry bcurPg = (PathGeometry)geometryGroup.Children[i + 1];
                                PathFigure bcurPf = bcurPg.Figures.ElementAt(0);
                                bcurPf.StartPoint = new Point(bcurPf.StartPoint.X, bcurPf.StartPoint.Y + changeValue);
                                ((LineSegment)bcurPf.Segments[0]).Point = new Point(((LineSegment)bcurPf.Segments[0]).Point.X, ((LineSegment)bcurPf.Segments[0]).Point.Y + changeValue);
                            }
                        }


                        //如果最高层的Y不为负就不用向下移动
                        if (((LineSegment)tempcurPf.Segments[2]).Point.Y < 0)
                        {
                            double temp = Math.Abs(((LineSegment)tempcurPf.Segments[2]).Point.Y);
                            for (int i = 0; i < geometryGroup.Children.Count; i++)
                            {
                                PathGeometry curPg = (PathGeometry)geometryGroup.Children[i];
                                PathFigure curPf = curPg.Figures.ElementAt(0);
                                curPf.StartPoint = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + temp);
                                ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + temp);
                                ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + temp);
                                ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + temp);
                            }

                            currentCom.newPath.Height += temp * 2;
                            currentCom.height = currentCom.newPath.Height;
                        }
                    }
                    if (changeValue != 0)
                    {
                        ColorProc.processWhenChangeLayerHeight(b_curPf, 0, 1);
                        ColorProc.processWhenChangeLayerHeight(t_curPf, 0, 0);
                    }
                }

                //层左厚度变化，保存到数据中
                if (originLeftSize - Double.Parse(leftSize.Text) != 0)
                {
                    this.currentCom.layerLeftThickness[layerNumberOne - 1] = Double.Parse(leftSize.Text);
                    this.currentCom.layerLeftThickness[layerNumberTwo - 1] = Double.Parse(leftSize.Text);
                }
            }
            catch
            {
                layerEdit _ = new layerEdit((MainWindow)Application.Current.MainWindow);
                mainWindow.changeArcSegmentToLineSegment(1, 2);
                layerEdit _le = new layerEdit();
                _le.layerNums.ItemsSource = currentCom.layerNums;
                _le.layerNums.SelectedIndex = currentLayerNum - 1;
                _le.currentLayerNum = currentLayerNum;
                _le.setComponent(currentCom);
                _le.ChangeLayerSizeEvent += new ChangeLayerSizeHandler(mainWindow.autoResize);
                _le.leftSize.Text = leftSize.Text;
                _le.rightSize.Text = rightSize.Text;
                _le.OKButton_Click(null, null);
                int isConvex = 0;
                if (currentCom.isChangeIOgive) isConvex = 1;
                mainWindow.changeLineSegmentToPolySegmentForLayer(currentCom.radius, isConvex);
            }
            layerEdit le = new layerEdit((MainWindow)Application.Current.MainWindow);
            mainWindow.makeLeftCoverConnectSkillfully();
        }

        public void rightSizeChange()
        {
            try
            {
                //判断当前层号是奇偶数？  要改一对。。。
                int layerNumberOne = Int32.Parse(layerNums.SelectedItem.ToString());
                int layerNumberTwo;
                double originRightSize;
                if (layerNumberOne % 2 == 0)
                {
                    layerNumberTwo = layerNumberOne - 1;

                    GeometryGroup geometryGroup = (GeometryGroup)currentCom.newPath.Data;
                    PathGeometry t_curPg = (PathGeometry)geometryGroup.Children[layerNumberTwo];
                    PathFigure t_curPf = t_curPg.Figures.ElementAt(0);
                    PathGeometry b_curPg = (PathGeometry)geometryGroup.Children[layerNumberOne];
                    PathFigure b_curPf = b_curPg.Figures.ElementAt(0);

                    originRightSize = ((LineSegment)t_curPf.Segments[0]).Point.Y - ((LineSegment)t_curPf.Segments[1]).Point.Y;
                    double changeValue;
                    if (originRightSize - Double.Parse(rightSize.Text) > 0)
                    {
                        //厚度变小
                        changeValue = originRightSize - Double.Parse(rightSize.Text);

                        ((LineSegment)t_curPf.Segments[1]).Point = new Point(((LineSegment)t_curPf.Segments[1]).Point.X, ((LineSegment)t_curPf.Segments[1]).Point.Y + changeValue);
                        ((LineSegment)b_curPf.Segments[1]).Point = new Point(((LineSegment)b_curPf.Segments[1]).Point.X, ((LineSegment)b_curPf.Segments[1]).Point.Y - changeValue);

                        //判断是否有在修改本层之上的层
                        if (geometryGroup.Children.Count - 1 > layerNumberOne)
                        {
                            //有
                            for (int i = layerNumberOne + 1; i < geometryGroup.Children.Count; i += 2)
                            {
                                PathGeometry tcurPg = (PathGeometry)geometryGroup.Children[i];
                                PathFigure tcurPf = tcurPg.Figures.ElementAt(0);
                                ((LineSegment)tcurPf.Segments[0]).Point = new Point(((LineSegment)tcurPf.Segments[0]).Point.X, ((LineSegment)tcurPf.Segments[0]).Point.Y + changeValue);
                                ((LineSegment)tcurPf.Segments[1]).Point = new Point(((LineSegment)tcurPf.Segments[1]).Point.X, ((LineSegment)tcurPf.Segments[1]).Point.Y + changeValue);

                                PathGeometry bcurPg = (PathGeometry)geometryGroup.Children[i + 1];
                                PathFigure bcurPf = bcurPg.Figures.ElementAt(0);
                                ((LineSegment)bcurPf.Segments[2]).Point = new Point(((LineSegment)bcurPf.Segments[2]).Point.X, ((LineSegment)bcurPf.Segments[2]).Point.Y - changeValue);
                                ((LineSegment)bcurPf.Segments[1]).Point = new Point(((LineSegment)bcurPf.Segments[1]).Point.X, ((LineSegment)bcurPf.Segments[1]).Point.Y - changeValue);
                            }
                        }
                    }
                    else
                    {
                        //厚度变大
                        changeValue = Double.Parse(rightSize.Text) - originRightSize;

                        //currentCom.newPath.Height += changeValue * 2;
                        //currentCom.height = currentCom.newPath.Height;

                        ((LineSegment)t_curPf.Segments[1]).Point = new Point(((LineSegment)t_curPf.Segments[1]).Point.X, ((LineSegment)t_curPf.Segments[1]).Point.Y - changeValue);
                        ((LineSegment)b_curPf.Segments[1]).Point = new Point(((LineSegment)b_curPf.Segments[1]).Point.X, ((LineSegment)b_curPf.Segments[1]).Point.Y + changeValue);

                        //整体的高度调节？？？？
                        //如果变换的高度与之前最高层Y距离顶部的距离相同，则整体的高度会变小
                        //if (Math.Abs(((LineSegment)tempcurPf.Segments[2]).Point.Y) == changeValue)
                        //{
                        //    currentCom.newPath.Height -= changeValue * 2;
                        //    currentCom.height = currentCom.newPath.Height;
                        //}

                        //判断是否有在修改本层之上的层
                        if (geometryGroup.Children.Count - 1 > layerNumberOne)
                        {
                            //有
                            for (int i = layerNumberOne + 1; i < geometryGroup.Children.Count; i += 2)
                            {
                                PathGeometry tcurPg = (PathGeometry)geometryGroup.Children[i];
                                PathFigure tcurPf = tcurPg.Figures.ElementAt(0);
                                ((LineSegment)tcurPf.Segments[0]).Point = new Point(((LineSegment)tcurPf.Segments[0]).Point.X, ((LineSegment)tcurPf.Segments[0]).Point.Y - changeValue);
                                ((LineSegment)tcurPf.Segments[1]).Point = new Point(((LineSegment)tcurPf.Segments[1]).Point.X, ((LineSegment)tcurPf.Segments[1]).Point.Y - changeValue);

                                PathGeometry bcurPg = (PathGeometry)geometryGroup.Children[i + 1];
                                PathFigure bcurPf = bcurPg.Figures.ElementAt(0);
                                ((LineSegment)bcurPf.Segments[2]).Point = new Point(((LineSegment)bcurPf.Segments[2]).Point.X, ((LineSegment)bcurPf.Segments[2]).Point.Y + changeValue);
                                ((LineSegment)bcurPf.Segments[1]).Point = new Point(((LineSegment)bcurPf.Segments[1]).Point.X, ((LineSegment)bcurPf.Segments[1]).Point.Y + changeValue);
                            }
                        }

                        //如果最高层的Y不为负就不用向下移动
                        PathGeometry tempcurPg = (PathGeometry)geometryGroup.Children[geometryGroup.Children.Count - 2];
                        PathFigure tempcurPf = tempcurPg.Figures.ElementAt(0);

                        if (((LineSegment)tempcurPf.Segments[1]).Point.Y < 0)
                        {
                            double temp = Math.Abs(((LineSegment)tempcurPf.Segments[1]).Point.Y);
                            for (int i = 0; i < geometryGroup.Children.Count; i++)
                            {
                                PathGeometry curPg = (PathGeometry)geometryGroup.Children[i];
                                PathFigure curPf = curPg.Figures.ElementAt(0);
                                curPf.StartPoint = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + temp);
                                ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + temp);
                                ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + temp);
                                ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + temp);
                            }
                            currentCom.newPath.Height += temp * 2;
                            currentCom.height = currentCom.newPath.Height;

                        }
                    }
                    if (changeValue != 0)
                    {
                        ColorProc.processWhenChangeLayerHeight(b_curPf, 1, 1);
                        ColorProc.processWhenChangeLayerHeight(t_curPf, 1, 0);
                    }
                }
                else
                {
                    layerNumberTwo = layerNumberOne + 1;

                    GeometryGroup geometryGroup = (GeometryGroup)currentCom.newPath.Data;
                    PathGeometry t_curPg = (PathGeometry)geometryGroup.Children[layerNumberOne];
                    PathFigure t_curPf = t_curPg.Figures.ElementAt(0);
                    PathGeometry b_curPg = (PathGeometry)geometryGroup.Children[layerNumberTwo];
                    PathFigure b_curPf = b_curPg.Figures.ElementAt(0);

                    originRightSize = ((LineSegment)t_curPf.Segments[0]).Point.Y - ((LineSegment)t_curPf.Segments[1]).Point.Y;
                    double changeValue;
                    if (originRightSize - Double.Parse(rightSize.Text) > 0)
                    {
                        //厚度变小
                        changeValue = originRightSize - Double.Parse(rightSize.Text);

                        ((LineSegment)t_curPf.Segments[1]).Point = new Point(((LineSegment)t_curPf.Segments[1]).Point.X, ((LineSegment)t_curPf.Segments[1]).Point.Y + changeValue);
                        ((LineSegment)b_curPf.Segments[1]).Point = new Point(((LineSegment)b_curPf.Segments[1]).Point.X, ((LineSegment)b_curPf.Segments[1]).Point.Y - changeValue);

                        //判断是否有在修改本层之上的层
                        if (geometryGroup.Children.Count - 1 > layerNumberTwo)
                        {
                            //有
                            for (int i = layerNumberTwo + 1; i < geometryGroup.Children.Count; i += 2)
                            {
                                PathGeometry tcurPg = (PathGeometry)geometryGroup.Children[i];
                                PathFigure tcurPf = tcurPg.Figures.ElementAt(0);
                                ((LineSegment)tcurPf.Segments[0]).Point = new Point(((LineSegment)tcurPf.Segments[0]).Point.X, ((LineSegment)tcurPf.Segments[0]).Point.Y + changeValue);
                                ((LineSegment)tcurPf.Segments[1]).Point = new Point(((LineSegment)tcurPf.Segments[1]).Point.X, ((LineSegment)tcurPf.Segments[1]).Point.Y + changeValue);

                                PathGeometry bcurPg = (PathGeometry)geometryGroup.Children[i + 1];
                                PathFigure bcurPf = bcurPg.Figures.ElementAt(0);
                                ((LineSegment)bcurPf.Segments[2]).Point = new Point(((LineSegment)bcurPf.Segments[2]).Point.X, ((LineSegment)bcurPf.Segments[2]).Point.Y - changeValue);
                                ((LineSegment)bcurPf.Segments[1]).Point = new Point(((LineSegment)bcurPf.Segments[1]).Point.X, ((LineSegment)bcurPf.Segments[1]).Point.Y - changeValue);
                            }
                        }
                    }
                    else
                    {
                        //厚度变大
                        changeValue = Double.Parse(rightSize.Text) - originRightSize;

                        //currentCom.newPath.Height += changeValue * 2;
                        //currentCom.height = currentCom.newPath.Height;

                        ((LineSegment)t_curPf.Segments[1]).Point = new Point(((LineSegment)t_curPf.Segments[1]).Point.X, ((LineSegment)t_curPf.Segments[1]).Point.Y - changeValue);
                        ((LineSegment)b_curPf.Segments[1]).Point = new Point(((LineSegment)b_curPf.Segments[1]).Point.X, ((LineSegment)b_curPf.Segments[1]).Point.Y + changeValue);

                        PathGeometry tempcurPg = (PathGeometry)geometryGroup.Children[geometryGroup.Children.Count - 2];
                        PathFigure tempcurPf = tempcurPg.Figures.ElementAt(0);

                        //如果变换的高度与之前最高层Y距离顶部的距离相同，则整体的高度会变小
                        //if (Math.Abs(((LineSegment)tempcurPf.Segments[2]).Point.Y) == changeValue)
                        //{
                        //    currentCom.newPath.Height -= changeValue * 2;
                        //    currentCom.height = currentCom.newPath.Height;
                        //}


                        //判断是否有在修改本层之上的层
                        if (geometryGroup.Children.Count - 1 > layerNumberTwo)
                        {
                            //有
                            for (int i = layerNumberTwo + 1; i < geometryGroup.Children.Count; i += 2)
                            {
                                PathGeometry tcurPg = (PathGeometry)geometryGroup.Children[i];
                                PathFigure tcurPf = tcurPg.Figures.ElementAt(0);
                                ((LineSegment)tcurPf.Segments[0]).Point = new Point(((LineSegment)tcurPf.Segments[0]).Point.X, ((LineSegment)tcurPf.Segments[0]).Point.Y - changeValue);
                                ((LineSegment)tcurPf.Segments[1]).Point = new Point(((LineSegment)tcurPf.Segments[1]).Point.X, ((LineSegment)tcurPf.Segments[1]).Point.Y - changeValue);

                                PathGeometry bcurPg = (PathGeometry)geometryGroup.Children[i + 1];
                                PathFigure bcurPf = bcurPg.Figures.ElementAt(0);
                                ((LineSegment)bcurPf.Segments[2]).Point = new Point(((LineSegment)bcurPf.Segments[2]).Point.X, ((LineSegment)bcurPf.Segments[2]).Point.Y + changeValue);
                                ((LineSegment)bcurPf.Segments[1]).Point = new Point(((LineSegment)bcurPf.Segments[1]).Point.X, ((LineSegment)bcurPf.Segments[1]).Point.Y + changeValue);
                            }
                        }


                        //如果最高层的Y不为负就不用向下移动
                        if (((LineSegment)tempcurPf.Segments[1]).Point.Y < 0)
                        {
                            double temp = Math.Abs(((LineSegment)tempcurPf.Segments[1]).Point.Y);
                            for (int i = 0; i < geometryGroup.Children.Count; i++)
                            {
                                PathGeometry curPg = (PathGeometry)geometryGroup.Children[i];
                                PathFigure curPf = curPg.Figures.ElementAt(0);
                                curPf.StartPoint = new Point(curPf.StartPoint.X, curPf.StartPoint.Y + temp);
                                ((LineSegment)curPf.Segments[0]).Point = new Point(((LineSegment)curPf.Segments[0]).Point.X, ((LineSegment)curPf.Segments[0]).Point.Y + temp);
                                ((LineSegment)curPf.Segments[1]).Point = new Point(((LineSegment)curPf.Segments[1]).Point.X, ((LineSegment)curPf.Segments[1]).Point.Y + temp);
                                ((LineSegment)curPf.Segments[2]).Point = new Point(((LineSegment)curPf.Segments[2]).Point.X, ((LineSegment)curPf.Segments[2]).Point.Y + temp);
                            }

                            currentCom.newPath.Height += temp * 2;
                            currentCom.height = currentCom.newPath.Height;
                        }
                    }

                    if (changeValue != 0)
                    {
                        ColorProc.processWhenChangeLayerHeight(b_curPf, 1, 1);
                        ColorProc.processWhenChangeLayerHeight(t_curPf, 1, 0);
                    }
                }


                //层右厚度变化，保存到数据中
                if (originRightSize - Double.Parse(rightSize.Text) != 0)
                {
                    this.currentCom.layerRightThickness[layerNumberOne - 1] = Double.Parse(rightSize.Text);
                    this.currentCom.layerRightThickness[layerNumberTwo - 1] = Double.Parse(rightSize.Text);
                }
            }
            catch {
                layerEdit _ = new layerEdit((MainWindow)Application.Current.MainWindow);
                mainWindow.changeArcSegmentToLineSegment(1, 2);
                layerEdit _le = new layerEdit();
                _le.setComponent(currentCom);
                _le.currentLayerNum = currentLayerNum;
                _le.layerNums.ItemsSource = currentCom.layerNums;
                _le.layerNums.SelectedIndex = currentLayerNum - 1;
                _le.ChangeLayerSizeEvent += new ChangeLayerSizeHandler(mainWindow.autoResize);
                _le.leftSize.Text = leftSize.Text;
                _le.rightSize.Text = rightSize.Text;
                _le.OKButton_Click(null, null);
                int isConvex = 0;
                if (currentCom.isChangeIOgive) isConvex = 1;
                mainWindow.changeLineSegmentToPolySegmentForLayer(currentCom.radius, isConvex);
            }
            layerEdit le = new layerEdit((MainWindow)Application.Current.MainWindow);
            mainWindow.makeRightCoverConnectSkillfully();
        }

        private void materialBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbb = (ComboBox)sender;
            lastMaterial = cbb.SelectedItem.ToString();
        }

        private void layerNums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flag == true)
            {
                //获取当前选择的层号
                int layerNumber = Int32.Parse(layerNums.SelectedItem.ToString());

                /*
                //显示当前材料
                accessDb = new UseAccessDB();
                accessDb.getConnection();
                ds = accessDb.SelectToDataSet("select * from material", "material");
                dt = ds.Tables[0];
                ArrayList col = new ArrayList();
                foreach (DataRow row in dt.Rows)
                {
                    if (row[dt.Columns[1]].ToString() == currentCom.layerMaterial[layerNumber-1].ToString())
                    {
                        Console.WriteLine((row[dt.Columns[1]]));
                        materialBox.SelectedItem = row[dt.Columns[1]];
                    }
                }*/

                if (list != null)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)list[j])["color"];
                        Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)list[j])["materialName"];
                        string mColor = cdic["content"];
                        string materialName = mdic["content"];
                        if (materialName == currentCom.layerMaterial[layerNumber - 1].ToString())
                        {
                            materialBox.SelectedItem = materialName;
                        }
                    }
                }

                if (currentCom.layerType[layerNumber - 1].ToString() == "球")
                {
                    ball.IsChecked = true;
                }
                else if (currentCom.layerType[layerNumber - 1].ToString() == "柱")
                {
                    column.IsChecked = true;
                }
                else if (currentCom.layerType[layerNumber - 1].ToString() == "杆")
                {
                    pole.IsChecked = true;
                }
                else if (currentCom.layerType[layerNumber - 1].ToString() == "长方体")
                {
                    cube.IsChecked = true;
                }
                radioButtonChanged();

                string key = layerNums.SelectedItem.ToString();
                diameter.Text = ((Hashtable)this.currentCom.layerSize[layerNums.SelectedItem])["diameter"].ToString();
                longLength.Text = ((Hashtable)this.currentCom.layerSize[layerNums.SelectedItem])["longLength"].ToString();
                width.Text = ((Hashtable)this.currentCom.layerSize[layerNums.SelectedItem])["width"].ToString();
                height.Text = ((Hashtable)this.currentCom.layerSize[layerNums.SelectedItem])["height"].ToString();

                int layerN = Int32.Parse(layerNums.SelectedItem.ToString());
                leftSize.Text = this.currentCom.layerLeftThickness[layerN - 1].ToString();
                rightSize.Text = this.currentCom.layerRightThickness[layerN - 1].ToString();
            }

            flag = true;
        }

        //radiobutton change
        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            radioButtonChanged();

            var radioButton = sender as RadioButton;
            lastRadioButton = radioButton.Content.ToString();
        }

        private void radioButton2_Checked(object sender, RoutedEventArgs e)
        {
            radioButton2Changed();

            //var radioButton = sender as RadioButton;
            //lastRadioButton = radioButton.Content.ToString();
        }

        private void radioButton2Changed()
        {
            if (pole != null && pole.IsChecked == true)
            {
                if (sectorBar.IsChecked == true)
                {
                    copyNumber.Visibility = Visibility.Visible;
                    copyNumber_.Visibility = Visibility.Visible;
                    thickness.Visibility = Visibility.Visible;
                    thickness_.Visibility = Visibility.Visible;
                    thickness__.Visibility = Visibility.Visible;
                }
                else
                {
                    copyNumber.Visibility = Visibility.Hidden;
                    copyNumber_.Visibility = Visibility.Hidden;
                    thickness.Visibility = Visibility.Hidden;
                    thickness_.Visibility = Visibility.Hidden;
                    thickness__.Visibility = Visibility.Hidden;
                }

                if (roundBar.IsChecked == true)
                {
                    diameter.IsEnabled = true;
                    longLength.IsEnabled = true;
                    width.IsEnabled = false;
                    height.IsEnabled = false;
                }

                if (squareBar.IsChecked == true)
                {
                    diameter.IsEnabled = false;
                    longLength.IsEnabled = true;
                    width.IsEnabled = true;
                    height.IsEnabled = true;
                }

                if (triangleBar.IsChecked == true || pentagonBar.IsChecked == true)
                {
                    long1.Visibility = Visibility.Visible;
                    long1_.Visibility = Visibility.Visible;
                    long1__.Visibility = Visibility.Visible;
                    long2.Visibility = Visibility.Visible;
                    long2_.Visibility = Visibility.Visible;
                    long2__.Visibility = Visibility.Visible;


                    if (triangleBar.IsChecked == true)
                    {
                        long3.Visibility = Visibility.Hidden;
                        long3_.Visibility = Visibility.Hidden;
                        long3__.Visibility = Visibility.Hidden;
                    }
                    if (pentagonBar.IsChecked == true)
                    {
                        long3.Visibility = Visibility.Visible;
                        long3_.Visibility = Visibility.Visible;
                        long3__.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    long1.Visibility = Visibility.Hidden;
                    long1_.Visibility = Visibility.Hidden;
                    long1__.Visibility = Visibility.Hidden;
                    long2.Visibility = Visibility.Hidden;
                    long2_.Visibility = Visibility.Hidden;
                    long2__.Visibility = Visibility.Hidden;
                    long3.Visibility = Visibility.Hidden;
                    long3_.Visibility = Visibility.Hidden;
                    long3__.Visibility = Visibility.Hidden;
                    long4.Visibility = Visibility.Hidden;
                    long4_.Visibility = Visibility.Hidden;
                    long4__.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                copyNumber.Visibility = Visibility.Hidden;
                copyNumber_.Visibility = Visibility.Hidden;
                thickness.Visibility = Visibility.Hidden;
                thickness_.Visibility = Visibility.Hidden;
                thickness__.Visibility = Visibility.Hidden;
                long1.Visibility = Visibility.Hidden;
                long1_.Visibility = Visibility.Hidden;
                long1__.Visibility = Visibility.Hidden;
                long2.Visibility = Visibility.Hidden;
                long2_.Visibility = Visibility.Hidden;
                long2__.Visibility = Visibility.Hidden;
                long3.Visibility = Visibility.Hidden;
                long3_.Visibility = Visibility.Hidden;
                long3__.Visibility = Visibility.Hidden;
                long4.Visibility = Visibility.Hidden;
                long4_.Visibility = Visibility.Hidden;
                long4__.Visibility = Visibility.Hidden;
            }
        }

        private void radioButtonChanged()
        {
            if (ball.IsChecked == true)
            {
                diameter.IsEnabled = true;
                longLength.IsEnabled = false;
                width.IsEnabled = false;
                height.IsEnabled = false;
                leftSize.IsEnabled = false;
                rightSize.IsEnabled = false;
                ObliqueAngle.Visibility = Visibility.Hidden;
                ObliqueAngleValue.Visibility = Visibility.Hidden;
                ObliqueAngleName.Visibility = Visibility.Hidden;
                roundBar.Visibility = Visibility.Hidden;
                squareBar.Visibility = Visibility.Hidden;
                sectorBar.Visibility = Visibility.Hidden;
                triangleBar.Visibility = Visibility.Hidden;
                pentagonBar.Visibility = Visibility.Hidden;
                copyNumber.Visibility = Visibility.Hidden;
                copyNumber_.Visibility = Visibility.Hidden;
                thickness.Visibility = Visibility.Hidden;
                thickness_.Visibility = Visibility.Hidden;
                thickness__.Visibility = Visibility.Hidden;
                long1.Visibility = Visibility.Hidden;
                long1_.Visibility = Visibility.Hidden;
                long1__.Visibility = Visibility.Hidden;
                long2.Visibility = Visibility.Hidden;
                long2_.Visibility = Visibility.Hidden;
                long2__.Visibility = Visibility.Hidden;
                long3.Visibility = Visibility.Hidden;
                long3_.Visibility = Visibility.Hidden;
                long3__.Visibility = Visibility.Hidden;
                long4.Visibility = Visibility.Hidden;
                long4_.Visibility = Visibility.Hidden;
                long4__.Visibility = Visibility.Hidden;
            }
            else if (column.IsChecked == true)
            {
                diameter.IsEnabled = true;
                longLength.IsEnabled = true;
                width.IsEnabled = false;
                height.IsEnabled = false;
                leftSize.IsEnabled = false;
                rightSize.IsEnabled = false;
                ObliqueAngle.Visibility = Visibility.Hidden;
                ObliqueAngleValue.Visibility = Visibility.Hidden;
                ObliqueAngleName.Visibility = Visibility.Hidden;
                roundBar.Visibility = Visibility.Hidden;
                squareBar.Visibility = Visibility.Hidden;
                sectorBar.Visibility = Visibility.Hidden;
                triangleBar.Visibility = Visibility.Hidden;
                pentagonBar.Visibility = Visibility.Hidden;
                copyNumber.Visibility = Visibility.Hidden;
                copyNumber_.Visibility = Visibility.Hidden;
                thickness.Visibility = Visibility.Hidden;
                thickness_.Visibility = Visibility.Hidden;
                thickness__.Visibility = Visibility.Hidden;
                long1.Visibility = Visibility.Hidden;
                long1_.Visibility = Visibility.Hidden;
                long1__.Visibility = Visibility.Hidden;
                long2.Visibility = Visibility.Hidden;
                long2_.Visibility = Visibility.Hidden;
                long2__.Visibility = Visibility.Hidden;
                long3.Visibility = Visibility.Hidden;
                long3_.Visibility = Visibility.Hidden;
                long3__.Visibility = Visibility.Hidden;
                long4.Visibility = Visibility.Hidden;
                long4_.Visibility = Visibility.Hidden;
                long4__.Visibility = Visibility.Hidden;
            }
            else if (pole.IsChecked == true)
            {
                diameter.IsEnabled = true;
                longLength.IsEnabled = true;
                width.IsEnabled = false;
                height.IsEnabled = false;
                leftSize.IsEnabled = false;
                rightSize.IsEnabled = false;
                ObliqueAngle.Visibility = Visibility.Visible;
                ObliqueAngleValue.Visibility = Visibility.Visible;
                ObliqueAngleName.Visibility = Visibility.Visible;
                roundBar.Visibility = Visibility.Visible;
                squareBar.Visibility = Visibility.Visible;
                sectorBar.Visibility = Visibility.Visible;
                triangleBar.Visibility = Visibility.Visible;
                pentagonBar.Visibility = Visibility.Visible;
                radioButton2Changed();
            }
            else if (cube.IsChecked == true)
            {
                diameter.IsEnabled = false;
                longLength.IsEnabled = true;
                width.IsEnabled = true;
                height.IsEnabled = true;
                leftSize.IsEnabled = false;
                rightSize.IsEnabled = false;
                ObliqueAngle.Visibility = Visibility.Hidden;
                ObliqueAngleValue.Visibility = Visibility.Hidden;
                ObliqueAngleName.Visibility = Visibility.Hidden;
                roundBar.Visibility = Visibility.Hidden;
                squareBar.Visibility = Visibility.Hidden;
                sectorBar.Visibility = Visibility.Hidden;
                triangleBar.Visibility = Visibility.Hidden;
                pentagonBar.Visibility = Visibility.Hidden;
                copyNumber.Visibility = Visibility.Hidden;
                copyNumber_.Visibility = Visibility.Hidden;
                thickness.Visibility = Visibility.Hidden;
                thickness_.Visibility = Visibility.Hidden;
                thickness__.Visibility = Visibility.Hidden;
                long1.Visibility = Visibility.Hidden;
                long1_.Visibility = Visibility.Hidden;
                long1__.Visibility = Visibility.Hidden;
                long2.Visibility = Visibility.Hidden;
                long2_.Visibility = Visibility.Hidden;
                long2__.Visibility = Visibility.Hidden;
                long3.Visibility = Visibility.Hidden;
                long3_.Visibility = Visibility.Hidden;
                long3__.Visibility = Visibility.Hidden;
                long4.Visibility = Visibility.Hidden;
                long4_.Visibility = Visibility.Hidden;
                long4__.Visibility = Visibility.Hidden;
            }
            else if(structure.IsChecked == true)
            {
                diameter.IsEnabled = false;
                longLength.IsEnabled = false;
                width.IsEnabled = false;
                height.IsEnabled = false;
                leftSize.IsEnabled = true;
                rightSize.IsEnabled = true;
                ObliqueAngle.Visibility = Visibility.Hidden;
                ObliqueAngleValue.Visibility = Visibility.Hidden;
                ObliqueAngleName.Visibility = Visibility.Hidden;
                roundBar.Visibility = Visibility.Hidden;
                squareBar.Visibility = Visibility.Hidden;
                sectorBar.Visibility = Visibility.Hidden;
                triangleBar.Visibility = Visibility.Hidden;
                pentagonBar.Visibility = Visibility.Hidden;
                copyNumber.Visibility = Visibility.Hidden;
                copyNumber_.Visibility = Visibility.Hidden;
                thickness.Visibility = Visibility.Hidden;
                thickness_.Visibility = Visibility.Hidden;
                thickness__.Visibility = Visibility.Hidden;
                long1.Visibility = Visibility.Hidden;
                long1_.Visibility = Visibility.Hidden;
                long1__.Visibility = Visibility.Hidden;
                long2.Visibility = Visibility.Hidden;
                long2_.Visibility = Visibility.Hidden;
                long2__.Visibility = Visibility.Hidden;
                long3.Visibility = Visibility.Hidden;
                long3_.Visibility = Visibility.Hidden;
                long3__.Visibility = Visibility.Hidden;
                long4.Visibility = Visibility.Hidden;
                long4_.Visibility = Visibility.Hidden;
                long4__.Visibility = Visibility.Hidden;
            }
        }

        private void saveRadioButtonData(string content)
        {
            int layerNumber = Int32.Parse(layerNums.SelectedItem.ToString());

            if (currentCom.layerType[layerNumber - 1].ToString() != content)
            {
                currentCom.layerType[layerNumber - 1] = content;
                if (layerNumber % 2 != 0)
                {
                    currentCom.layerType[layerNumber] = content;
                }
                else
                {
                    currentCom.layerType[layerNumber - 1 - 1] = content;
                }
            }
        }

        private void saveMaterialData(string content)
        {
            int layerNumber = Int32.Parse(layerNums.SelectedItem.ToString());

            if (content != null && currentCom.layerMaterial[layerNumber - 1].ToString() != content)
            {
                currentCom.layerMaterial[layerNumber - 1] = content;

                if (layerNumber % 2 != 0)
                {
                    currentCom.layerMaterial[layerNumber] = content;
                }
                else
                {
                    currentCom.layerMaterial[layerNumber - 1 - 1] = content;
                }
                if (materialBox.SelectedItem != null)
                {
                    string comValue = materialBox.SelectedItem.ToString();

                    for (int i = 0; i < list.Count; i++)
                    {
                        Dictionary<string, string> cdic = ((Dictionary<string, Dictionary<string, string>>)list[i])["color"];
                        Dictionary<string, string> mdic = ((Dictionary<string, Dictionary<string, string>>)list[i])["materialName"];
                        string mColor = cdic["content"];
                        string materialName = mdic["content"];
                        if (materialName == comValue)
                        {
                            Color color = (Color)ColorConverter.ConvertFromString(mColor);

                            if (layerNumber % 2 == 0)
                                ColorProc.processWhenChangeLayerColor(currentCom.newPath, layerNumber - 1, color);
                            else
                                ColorProc.processWhenChangeLayerColor(currentCom.newPath, layerNumber, color);
                        }
                    }
                }
                /*
                //遍历数据表，得到对应的颜色值
                foreach (DataRow row in dt.Rows)
                {
                    if (row[dt.Columns[1]].ToString() == comValue)
                    {
                        Console.WriteLine((row[dt.Columns[3]]));
                        Color color = (Color)ColorConverter.ConvertFromString(row[dt.Columns[3]].ToString());
                        //
                        if (layerNumber % 2 == 0)
                            ColorProc.processWhenChangeLayerColor(currentCom.newPath, layerNumber - 1, color);
                        else
                            ColorProc.processWhenChangeLayerColor(currentCom.newPath, layerNumber, color);

                    }
                }
                 * */
            }
        }

        private void saveLayerSizeData()
        {
            lastDiameter = diameter.Text;
            lastLongLength = longLength.Text;
            lastWidth = width.Text;
            lastHeight = height.Text;
            lastObliqueAngle = ObliqueAngle.Text;

            int layerNumber = Int32.Parse(layerNums.SelectedItem.ToString());

            if (((Hashtable)this.currentCom.layerSize[layerNumber])["diameter"].ToString() != lastDiameter)
            {
                ((Hashtable)this.currentCom.layerSize[layerNumber])["diameter"] = lastDiameter;

                if (layerNumber % 2 != 0)
                {
                    ((Hashtable)this.currentCom.layerSize[layerNumber + 1])["diameter"] = lastDiameter;
                }
                else
                {
                    ((Hashtable)this.currentCom.layerSize[layerNumber - 1])["diameter"] = lastDiameter;
                }
            }

            if (((Hashtable)this.currentCom.layerSize[layerNumber])["longLength"].ToString() != lastLongLength)
            {
                ((Hashtable)this.currentCom.layerSize[layerNumber])["longLength"] = lastLongLength;

                if (layerNumber % 2 != 0)
                {
                    ((Hashtable)this.currentCom.layerSize[layerNumber + 1])["longLength"] = lastLongLength;
                }
                else
                {
                    ((Hashtable)this.currentCom.layerSize[layerNumber - 1])["longLength"] = lastLongLength;
                }
            }

            if (((Hashtable)this.currentCom.layerSize[layerNumber])["width"].ToString() != lastWidth)
            {
                ((Hashtable)this.currentCom.layerSize[layerNumber])["width"] = lastWidth;

                if (layerNumber % 2 != 0)
                {
                    ((Hashtable)this.currentCom.layerSize[layerNumber + 1])["width"] = lastWidth;
                }
                else
                {
                    ((Hashtable)this.currentCom.layerSize[layerNumber - 1])["width"] = lastWidth;
                }
            }

            if (((Hashtable)this.currentCom.layerSize[layerNumber])["height"].ToString() != lastHeight)
            {
                ((Hashtable)this.currentCom.layerSize[layerNumber])["height"] = lastHeight;

                if (layerNumber % 2 != 0)
                {
                    ((Hashtable)this.currentCom.layerSize[layerNumber + 1])["height"] = lastHeight;
                }
                else
                {
                    ((Hashtable)this.currentCom.layerSize[layerNumber - 1])["height"] = lastHeight;
                }
            }

            if (((Hashtable)this.currentCom.layerSize[layerNumber])["ObliqueAngle"].ToString() != lastObliqueAngle)
            {
                ((Hashtable)this.currentCom.layerSize[layerNumber])["ObliqueAngle"] = lastObliqueAngle;

                if (layerNumber % 2 != 0)
                {
                    ((Hashtable)this.currentCom.layerSize[layerNumber + 1])["ObliqueAngle"] = lastObliqueAngle;
                }
                else
                {
                    ((Hashtable)this.currentCom.layerSize[layerNumber - 1])["ObliqueAngle"] = lastObliqueAngle;
                }
            }
        }
        
    }
}
