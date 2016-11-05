using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GraphicalStructure
{
    /// <summary>
    /// EditMaterialAttrWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditMaterialAttrWindow : Window
    {

        public delegate void PassValuesHandler(object sender, Validity data);
        public event PassValuesHandler PassValuesEvent;

        private string materialName;
        private Label[] labels;
        private TextBox[] textBoxes;

        private string type;

        private int matParamNum;

        private int soeParamNum;


        private void setLabelsAndTextBoxes() {
            labels[0] = label_1;
            labels[1] = label_2;
            labels[2] = label_3;
            labels[3] = label_4;
            labels[4] = label_5;
            labels[5] = label_6;
            labels[6] = label_7;
            labels[7] = label_8;
            labels[8] = label_9;
            labels[9] = label_10;
            labels[10] = label_11;
            labels[11] = label_12;
            labels[12] = label_13;
            labels[13] = label_14;
            labels[14] = label_15;
            labels[15] = label_16;
            labels[16] = label_17;
            labels[17] = label_18;
            labels[18] = label_19;
            labels[19] = label_20;
            labels[20] = label_21;
            labels[21] = label_22;
            labels[22] = label_23;
            labels[23] = label_24;
            labels[24] = label_25;
            labels[25] = label_26;
            labels[26] = label_27;
            labels[27] = label_28;
            labels[28] = label_29;
            textBoxes[0] = textBox_1;
            textBoxes[1] = textBox_2;
            textBoxes[2] = textBox_3;
            textBoxes[3] = textBox_4;
            textBoxes[4] = textBox_5;
            textBoxes[5] = textBox_6;
            textBoxes[6] = textBox_7;
            textBoxes[7] = textBox_8;
            textBoxes[8] = textBox_9;
            textBoxes[9] = textBox_10;
            textBoxes[10] = textBox_11;
            textBoxes[11] = textBox_12;
            textBoxes[12] = textBox_13;
            textBoxes[13] = textBox_14;
            textBoxes[14] = textBox_15;
            textBoxes[15] = textBox_16;
            textBoxes[16] = textBox_17;
            textBoxes[17] = textBox_18;
            textBoxes[18] = textBox_19;
            textBoxes[19] = textBox_20;
            textBoxes[20] = textBox_21;
            textBoxes[21] = textBox_22;
            textBoxes[22] = textBox_23;
            textBoxes[23] = textBox_24;
            textBoxes[24] = textBox_25;
            textBoxes[25] = textBox_26;
            textBoxes[26] = textBox_27;
            textBoxes[27] = textBox_28;
            textBoxes[28] = textBox_29;
        }

        // 初始化界面文本框
        // i 表示load窗口点击第几条item
        public void setMaterialName(string mn, string refer){
            type = "materialName";
            setLabelsAndTextBoxes();
            materialName = mn; 
            textBox_1.Text = mn;
            label_1.Content = "材料名";
            textBox_1.Visibility = Visibility.Visible;
            label_1.Visibility = Visibility.Visible;
            label_refer.Visibility = Visibility.Visible;
            label_refer.Content = "文献引用";
            textBox_refer.Visibility = Visibility.Visible;
            textBox_refer.Text = refer;
            for (int i = 1; i < 29; i++) {
                textBoxes[i].Visibility = Visibility.Hidden;
                labels[i].Visibility = Visibility.Hidden;
            }
        }

        // 初始化界面文本框
        public void setMat(Dictionary<string, string> dict) {
            type = "mat";
            setLabelsAndTextBoxes();
            label_refer.Visibility = Visibility.Hidden;
            textBox_refer.Visibility = Visibility.Hidden;
            int i = 0;
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                labels[i].Content = kvp.Key;
                textBoxes[i].Text = kvp.Value;
                labels[i].Visibility = Visibility.Visible;
                textBoxes[i].Visibility = Visibility.Visible;
                i++;
            }
            matParamNum = i ;
            for (; i < labels.Length; i++) {
                labels[i].Content = "v";
                textBoxes[i].Text = "0";
                labels[i].Visibility = Visibility.Hidden;
                textBoxes[i].Visibility = Visibility.Hidden;
            }
        }

        // 初始化界面文本框
        public void setSoe(Dictionary<string, string> dict) {
            type = "soe";
            setLabelsAndTextBoxes();
            label_refer.Visibility = Visibility.Hidden;
            textBox_refer.Visibility = Visibility.Hidden;
            int i = 0;
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                labels[i].Content = kvp.Key;
                textBoxes[i].Text = kvp.Value;
                labels[i].Visibility = Visibility.Visible;
                textBoxes[i].Visibility = Visibility.Visible;
                i++; 
            }
            soeParamNum = i;
            for (; i < labels.Length; i++)
            {
                labels[i].Content = "v";
                textBoxes[i].Text = "0";
                labels[i].Visibility = Visibility.Hidden;
                textBoxes[i].Visibility = Visibility.Hidden;
            }
        }

        public EditMaterialAttrWindow()
        {
            labels = new Label[29];
            textBoxes = new TextBox[29];
            InitializeComponent();
        }

        private void confirmClick(object sender, RoutedEventArgs e) {
            Validity val = new Validity();
            val.isConfirm = true;
            if (type == "materialName")
            {
                val.data = new Dictionary<string, Dictionary<string, string>>();
                val.data.Add("materialName", new Dictionary<string, string>() { { "content", textBox_1 .Text} });
                val.data.Add("refer", new Dictionary<string, string>() { { "content", textBox_refer.Text } });
            }
            else if (type == "mat")
            {
                val.data = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> matData = new Dictionary<string, string>();
                for (int i = 0; i < matParamNum; i++) {
                    matData.Add(labels[i].Content.ToString(), textBoxes[i].Text);
                }
                val.data.Add("mat", matData );
            }
            else if (type == "soe")
            {
                val.data = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> soeData = new Dictionary<string, string>();
                for (int i = 0; i < soeParamNum; i++)
                {
                    soeData.Add(labels[i].Content.ToString(), textBoxes[i].Text);
                }
                val.data.Add("soe", soeData);
            }
            val.type = type;
            PassValuesEvent(this, val);
            Close();
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            Validity val = new Validity();
            val.isConfirm = false;
            PassValuesEvent(this, val);
            Close();
        }
    }
}
