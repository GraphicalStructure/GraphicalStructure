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
    /// DataBaseWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DataBaseWindow : Window
    {
        private UseAccessDB adb;

        public DataBaseWindow()
        {
            InitializeComponent();

            adb = new UseAccessDB();
            adb.OpenDb();

            ArrayList al = adb.getTableName();
            if (al.Count != 0)
            {
                dataTable.ItemsSource = al;
                dataTable.SelectedItem = al[0];
            }
        }

        private void ReadTable(object sender, RoutedEventArgs e)
        {
            listView.ItemsSource = adb.queryAllTable("select * from material");
        }

        private void queryFromTable(object sender, RoutedEventArgs e)
        {
            if (queryMaterial.Text == "")
            {
                return;
            }

            string material = queryMaterial.Text;

            string sql = "select * from material where m_name='" + material + "\'";
            Console.WriteLine(sql);
            ArrayList arr = adb.queryByCondition(sql);
            if (arr.Count != 0)
            {
                currentID.Text = arr[0].ToString();
                currentMaterial.Text = arr[1].ToString();
                currentDensity.Text = arr[2].ToString();
                currentColor.Text = arr[3].ToString();
            }
            else
            {
                currentID.Text = "";
                currentMaterial.Text = "";
                currentDensity.Text = "";
                currentColor.Text = "";
                MessageBox.Show("没有查询到！","警告");
            }
        }

        private void insertTableData(object sender, RoutedEventArgs e)
        {
            if (addMaterial.Text == "")
            {
                addMaterial.Text = " ";
            }

            if (addDensity.Text == "")
            {
                addDensity.Text = " ";
            }

            if (addColor.Text == "")
            {
                addColor.Text = " ";
            }
            string sql = "insert into material(m_name,m_density,m_color) values('" + addMaterial.Text + "','" + addDensity.Text + "','" + addColor.Text + "')";
            bool result = adb.insertTableData(sql);
            if(!result)
            {
                MessageBox.Show("插入失败！","警告");
            }
        }

        private void deleteDataFromTable(object sender, RoutedEventArgs e)
        {
            string sql = "delete from material where ID =" + deleteID.Text;
            bool result = adb.deleteFromTableData(sql,Int32.Parse(deleteID.Text));
            if (!result)
            {
                MessageBox.Show("删除失败！", "警告");
            }
        }

        private void updateTableData(object sender, RoutedEventArgs e)
        {
            string sql = string.Format("update material set m_name='{0}',m_density='{1}',m_color='{2}' where ID=",currentMaterial.Text,currentDensity.Text,currentColor.Text) + currentID.Text;
            bool result = adb.updateTableData(sql);
            if (!result)
            {
                MessageBox.Show("更新失败！", "警告");
            }
        }


    }
}
