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

namespace GraphicalStructure
{
    /// <summary>
    /// explosionEdit.xaml 的交互逻辑
    /// </summary>
    ///

    public delegate void ChangePositionHandler();

    public partial class explosionEdit : Window
    {
        public ArrayList explosions;
        public Hashtable explosionData;

        public string lastRadioButton;

        public string lastStartPosition;
        public string lastRadius;
        public string lastPointNums;

        public event ChangePositionHandler ChangePositionEvent;

        private bool flag = false;

        public explosionEdit()
        {
            InitializeComponent();

            explosions = new ArrayList();
            explosionData = new Hashtable();
        }

        public void showComponentInfo()
        {
            if (explosions != null && explosions.Count != 0 && explosionData != null && explosionData.Count != 0)
            {
                explosionNum.ItemsSource = explosionData.Keys;
                explosionNum.SelectedIndex = 0;

                int key = Int32.Parse(explosionNum.SelectedItem.ToString());

                if (((Hashtable)explosionData[key])["type"].ToString() == "点")
                {
                    spot.IsChecked = true;
                }
                else if (((Hashtable)explosionData[key])["type"].ToString() == "环")
                {
                    ring.IsChecked = true;
                }
                else if (((Hashtable)explosionData[key])["type"].ToString() == "多点")
                {
                    multiSpot.IsChecked = true;
                }

                startPosition.Text = ((Hashtable)explosionData[key])["startPosition"].ToString();
                radius.Text = ((Hashtable)explosionData[key])["radius"].ToString();
                pointNum.Text = ((Hashtable)explosionData[key])["pointNums"].ToString();
            }
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            radioButtonChanged();

            var radioButton = sender as RadioButton;
            lastRadioButton = radioButton.Content.ToString();

        }

        private void radioButtonChanged()
        {
            if(spot.IsChecked == true)
            {
                startPosition.IsEnabled = true;
                radius.IsEnabled = false;
                pointNum.IsEnabled = false;
            }
            else if(ring.IsChecked == true)
            {
                startPosition.IsEnabled = true;
                radius.IsEnabled = true;
                pointNum.IsEnabled = false;
            }
            else if(multiSpot.IsEnabled == true)
            {
                startPosition.IsEnabled = true;
                radius.IsEnabled = true;
                pointNum.IsEnabled = true;
            }
        }

        private void explosionNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(flag == true)
            {
                //获取当前选择的起爆点编号
                int explosionNumber = Int32.Parse(explosionNum.SelectedItem.ToString());

                if (((Hashtable)explosionData[explosionNumber])["type"].ToString() == "点")
                {
                    spot.IsChecked = true;
                }
                else if (((Hashtable)explosionData[explosionNumber])["type"].ToString() == "环")
                {
                    ring.IsChecked = true;
                }
                else if (((Hashtable)explosionData[explosionNumber])["type"].ToString() == "多点")
                {
                    multiSpot.IsChecked = true;
                }

                radioButtonChanged();

                int key = Int32.Parse(explosionNum.SelectedItem.ToString());
                startPosition.Text = ((Hashtable)explosionData[key])["startPosition"].ToString();
                radius.Text = ((Hashtable)explosionData[key])["radius"].ToString();
                pointNum.Text = ((Hashtable)explosionData[key])["pointNums"].ToString();
            }
            flag = true;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            saveRadioButtonData(lastRadioButton);
            savePositionData();

            if (ChangePositionEvent != null)
            {
                ChangePositionEvent();
            }

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveRadioButtonData(string content)
        {
            int explosionNumber = Int32.Parse(explosionNum.SelectedItem.ToString());

            if (((Hashtable)explosionData[explosionNumber])["type"].ToString() != content)
            {
                ((Hashtable)explosionData[explosionNumber])["type"] = content;
            }
        }

        private void savePositionData()
        {
            lastStartPosition = startPosition.Text;
            lastRadius = radius.Text;
            lastPointNums = pointNum.Text;

            int explosionNumber = Int32.Parse(explosionNum.SelectedItem.ToString());

            if (((Hashtable)explosionData[explosionNumber])["startPosition"].ToString() != lastStartPosition)
            {
                ((Hashtable)explosionData[explosionNumber])["startPosition"] = lastStartPosition;
            }

            if (((Hashtable)explosionData[explosionNumber])["radius"].ToString() != lastRadius)
            {
                ((Hashtable)explosionData[explosionNumber])["radius"] = lastRadius;
            }

            if (((Hashtable)explosionData[explosionNumber])["pointNums"].ToString() != lastPointNums)
            {
                ((Hashtable)explosionData[explosionNumber])["pointNums"] = lastPointNums;
            }
        }
    }
}
