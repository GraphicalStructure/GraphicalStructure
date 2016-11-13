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
using Xceed.Wpf.Toolkit;

namespace GraphicalStructure
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class NewMaterialWindow : Window
    {

        public delegate void PassValuesHandler(object sender, Validity data);
        public event PassValuesHandler PassValuesEvent;

        private string materialName;

        private int updateWhich;

        private int isUpdate;

        private Dictionary<string, Dictionary<string, string>> materialForUpdate;

        private Dictionary<string, Dictionary<string, string>> gatherDataFromEditWindow ; 

        private Dictionary<string, string> matSoeMap;

        private Dictionary<string, Dictionary<string, string>> matNameToMatDetail;

        private Dictionary<string, Dictionary<string, string>> soeNameToSoeDetail;

        public NewMaterialWindow()
        {
            //init
            //读mat_table,将4条mat读出来，逐条添加到MatComBox
            matNameToMatDetail = new Dictionary<string, Dictionary<string, string>>();
            soeNameToSoeDetail = new Dictionary<string, Dictionary<string, string>>();
            matSoeMap = new Dictionary<string, string>();
            matSoeMap.Add("NULL", "LINEAR_POLYNOMIAL");
            matSoeMap.Add("HIGH_EXPLOSIVE_BURN", "JWL");
            matSoeMap.Add("JOHNSON_COOK", "GRUNEISEN");
            matSoeMap.Add("PLASTIC_KINEMATIC", "-");
            matNameToMatDetail.Add("NULL", new Dictionary<string, string>() { { "RO", "0" }, { "PC", "0" }, { "MU", "0" }, { "TEROD", "0" }, { "CEROD", "0" }, { "YM", "0" }, { "PR", "0" } });
            matNameToMatDetail.Add("HIGH_EXPLOSIVE_BURN", new Dictionary<string, string>() { { "RO", "0" }, { "D", "0" }, { "PCJ", "0" }, { "BETA", "0" }, { "K", "0" }, { "G", "0" }, { "SIGY", "0" } });
            matNameToMatDetail.Add("JOHNSON_COOK", new Dictionary<string, string>() { { "RO", "0" }, { "G", "0" }, { "E", "0" }, { "PR", "0" }, { "DTF", "0" }, { "VP", "0" }, { "RATEOP", "0" }, { "A", "0" } , { "B", "0" } , { "N", "0" },
            {"C", "0"},{"M", "0"},{"TM", "0"},{"TR", "0"},{"EPSO", "0"},{"CP", "0"},{"PC", "0"},{"SPALL", "0"},{"IT", "0"},{"D1", "0"},{"D2", "0"},{"D3", "0"},{"D4", "0"},{"D5", "0"},{"EROD", "0"},{"EFMIN", "0"},{"NUMINT", "0"},{"C2P", "0"} });
            matNameToMatDetail.Add("PLASTIC_KINEMATIC", new Dictionary<string, string>() { { "RO", "0" }, { "E", "0" }, { "PR", "0" }, { "SIGY", "0" }, { "ETAN", "0" }, { "BETA", "0" }, { "SRC", "0" }, { "SRP", "0" }, { "FS", "0" }, { "VP", "0" } });
            soeNameToSoeDetail.Add("LINEAR_POLYNOMIAL", new Dictionary<string, string>() { { "C0", "0" }, { "C1", "0" }, { "C2", "0" }, { "C3", "0" }, { "C4", "0" }, { "C5", "0" }, { "C6", "0" }, { "E0", "0" }, { "V0", "0" } });
            soeNameToSoeDetail.Add("JWL", new Dictionary<string, string>() { { "A", "0" }, { "B", "0" }, { "R1", "0" }, { "R2", "0" }, { "OMEG", "0" }, { "E0", "0" }, { "V0", "0" } });
            soeNameToSoeDetail.Add("GRUNEISEN", new Dictionary<string, string>() { { "C", "0" }, { "S1", "0" }, { "S2", "0" }, { "S3", "0" }, { "GAMAO", "0" }, { "A", "0" }, { "E0", "0" }, { "V0", "0" } });
            soeNameToSoeDetail.Add("-", new Dictionary<string, string>());

            // 初始化时：gatherMat，gatherSoe初始化为matNameToMatDetail， soeNameToSoeDetail

            InitializeComponent();
        }

        // 处理修改的mat参数数据
        private void receivedEditMatWindowData(object sender, Validity val) {
            if (val.isConfirm) {
                gatherDataFromEditWindow["mat"] = val.data["mat"];
            }
        }

        // 处理修改的Soe参数数据，新修改的数据加入gatherDataFromEditWindow
        private void receivedEditSoeWindowData(object sender, Validity val) {
            if (val.isConfirm)
            {
                gatherDataFromEditWindow["soe"] = val.data["soe"];
            }
        }

        // 选择mat的时候自动更新soe的选择
        private void updateTextBoxSoe(object sender, RoutedEventArgs e) {
            ComboBoxItem item = MatComboBox.SelectedItem as ComboBoxItem;
            string currentMatName = item.Content.ToString();
            textBox_soe.Text = matSoeMap[currentMatName];
            gatherDataFromEditWindow = new Dictionary<string, Dictionary<string, string>>();
            gatherDataFromEditWindow.Add("mat", matNameToMatDetail[currentMatName]);
            gatherDataFromEditWindow.Add("soe", soeNameToSoeDetail[matSoeMap[currentMatName]]);
        }

        // @deprecated
        public void setMaterialName(string mn){
            materialName = mn;
            textBox_materialName.Text = mn;
        }

        public void setPreUpdateMaterialData(int _isUpdate, Dictionary<string, Dictionary<string, string>> dict, int index) {
            isUpdate = _isUpdate;
            if (isUpdate == 1)
            {   // 是更新材料参数，会从父窗口带着数据
                updateWhich = index;
                materialForUpdate = dict;
                textBox_materialName.Text = materialForUpdate["materialName"]["content"];
                string matName = materialForUpdate["matName"]["content"];
                foreach (ComboBoxItem cbi in MatComboBox.Items)
                {
                    if (cbi.Content.ToString() == matName)
                    {
                        MatComboBox.SelectedItem = cbi;
                    }
                }
                textBox_Density.Text = materialForUpdate["density"]["content"];
                textBox_Color.Text = materialForUpdate["color"]["content"];
                textBox_refer.Text = materialForUpdate["refer"]["content"];

                gatherDataFromEditWindow = new Dictionary<string, Dictionary<string, string>>();
                gatherDataFromEditWindow.Add("mat", new Dictionary<string, string>(materialForUpdate["mat"]));
                gatherDataFromEditWindow.Add("soe", new Dictionary<string, string>(materialForUpdate["soe"]));
            }
            else {
            }
        }


        private void editSoeClick(object sender, RoutedEventArgs e) {
            // 编辑soe参数
            EditMaterialAttrWindow edaw = new EditMaterialAttrWindow();
            Dictionary<string, string> soeData = gatherDataFromEditWindow["soe"];
            edaw.PassValuesEvent += new EditMaterialAttrWindow.PassValuesHandler(receivedEditSoeWindowData);
            edaw.setSoe(soeData);
            edaw.Topmost = true;
            edaw.Show();
        }

        private void editMatClick(object sender, RoutedEventArgs e) {
            // 编辑mat参数
            EditMaterialAttrWindow edaw = new EditMaterialAttrWindow();
            Dictionary<string, string> matData = gatherDataFromEditWindow["mat"];
            edaw.PassValuesEvent += new EditMaterialAttrWindow.PassValuesHandler(receivedEditMatWindowData);
            edaw.setMat(matData);
            edaw.Topmost = true;
            edaw.Show();
        }


        private void confirmButtonClick(object sender, RoutedEventArgs e) {
            ComboBoxItem item = MatComboBox.SelectedItem as ComboBoxItem;
            if (item == null || textBox_materialName.Text == "") {
                // 未填写名称和没有选择mat
                return;
            }
            Validity val = new Validity();
            if (isUpdate == 0)
            {
                // 新增
                Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
                data.Add("materialName", new Dictionary<string, string>() { { "content", textBox_materialName.Text } });
                string currentMatName = item.Content.ToString();
                string currentSoeName = textBox_soe.Text;
                data.Add("matName", new Dictionary<string, string>() { { "content", currentMatName } });
                data.Add("mat", gatherDataFromEditWindow["mat"]);
                data.Add("soeName", new Dictionary<string, string>() { { "content", currentSoeName } });
                data.Add("soe", gatherDataFromEditWindow["soe"]);
                string density = textBox_Density.Text;
                if (density == null) {
                    density = "";
                }
                data.Add("density", new Dictionary<string, string>() { { "content", density } });
                string color = textBox_Color.Text;
                if (color == null)
                {
                    color = "";
                }
                data.Add("color", new Dictionary<string, string>() { { "content", color } });
                data.Add("refer", new Dictionary<string, string>() { { "content", textBox_refer.Text } });
                data.Add("density", new Dictionary<string, string>() { { "content", "1314" } });
                data.Add("color", new Dictionary<string, string>() { { "content", "#FFFFA500" } });
                val.isConfirm = true;
                val.type = "all";
                val.data = data;
            }
            else if (isUpdate == 1){
                // 修改材料
                materialForUpdate["materialName"]["content"] = textBox_materialName.Text;
                materialForUpdate["refer"]["content"] = textBox_refer.Text;
                materialForUpdate["matName"]["content"] = ((ComboBoxItem)MatComboBox.SelectedItem).Content.ToString();
                materialForUpdate["soeName"]["content"] = textBox_soe.Text;
                materialForUpdate["density"]["content"] = textBox_Density.Text;
                materialForUpdate["color"]["content"] = textBox_Color.Text;

                materialForUpdate["mat"] = new Dictionary<string, string>(gatherDataFromEditWindow["mat"]);
                materialForUpdate["soe"] = new Dictionary<string, string>(gatherDataFromEditWindow["soe"]);
                val.isConfirm = true;
                val.type = "all";
                val.data = materialForUpdate;
                val.data.Add("index", new Dictionary<string, string>() { { "content", "" + updateWhich } });
            }
            
            PassValuesEvent(this, val);
            Close();
        }

        private void cancelButtonClick(object sender, RoutedEventArgs e) {
            // 取消新建材料
            Close();
        }

        private string changeDictToString(Dictionary<string, string> dict) {
            string result = "";
            foreach (KeyValuePair<string, string> kvp in dict) {
                result +=kvp.Key + "=" + kvp.Value;
            }
            return result;
        }

        private void colorCanvas_SelectedColorChanged(object sender, EventArgs e)
        {
            textBox_Color.Text = colorCanvas.SelectedColor.ToString();
        }

        private void showColorCanvas(object sender, RoutedEventArgs e) {
            colorCanvas.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        private void hiddenColorCanvas(object sender, RoutedEventArgs e) {
            colorCanvas.Visibility = Visibility.Hidden;
            e.Handled = true;
        }
    }
}
