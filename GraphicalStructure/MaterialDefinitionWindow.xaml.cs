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
using System.IO;

namespace GraphicalStructure
{
    /// <summary>
    /// MaterialDefinitionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MaterialDefinitionWindow : Window
    {

        private int totalMaterialsNum;  // 从1开始, Tag中保存的比显示序号小1

        private List<Dictionary<string, Dictionary<string, string>>> totalMaterials;

        private UseAccessDB adb;

        // 从库中查得的数据，未经修改，修改之后放入updatedMaterials， ListBoxItem指某行listBox
        private List<Dictionary<string, Dictionary<string, string>>> DataBaseMaterials;

        public MaterialDefinitionWindow()
        {
            totalMaterials = new List<Dictionary<string, Dictionary<string, string>>>();
            totalMaterialsNum = 0;
            InitializeComponent();

            adb = new UseAccessDB();
            adb.OpenDb();
        }

        private void receivedNewMaterialWindowData(object sender, Validity val) {
            // 处理来自新建材料窗口的数据
            Dictionary<string, Dictionary<string, string>> newMaterial= val.data;
            ListBoxItem listBoxItem = new ListBoxItem();
            string materialName = newMaterial["materialName"]["content"];
            totalMaterials.Add(newMaterial);
            listBoxItem.Tag = totalMaterialsNum;
            totalMaterialsNum += 1;
            listBoxItem.Content = totalMaterialsNum + "\t" + materialName;
            // Tag存储当前行材料在totalMaterials的位置
            listBox.Items.Add(listBoxItem);
        }

        private void receivedUpdateMaterial(object sender, Validity val) {
            // 处理点击修改按钮之后收到数据的逻辑
            int index = Int32.Parse(val.data["index"]["content"]); // 修改了第index条数据，从0开始
            string materialName = totalMaterials[index]["materialName"]["content"];
            Dictionary < string, Dictionary < string, string>>  update = new Dictionary<string, Dictionary<string, string>>();
            foreach (KeyValuePair<string, Dictionary<string, string>> kvp in val.data) {
                if (kvp.Key != "index") {
                    update.Add(kvp.Key, new Dictionary<string, string>(kvp.Value));
                }
            }
            if (materialName != update["materialName"]["content"])
            {
                foreach (ListBoxItem lbi in listBox.Items)
                {
                    int lbiTag = Convert.ToInt32(lbi.Tag);
                    if (lbiTag == index)
                    {
                        int _tag = Convert.ToInt32(lbi.Tag);
                        lbi.Content = _tag+1 +  "\t" + update["materialName"]["content"];
                    }
                }
            }
            totalMaterials[index] = update;
        }

        private void receivedLoadMaterialWindowData(object sender, Validity val) {
            // 处理来自load材料窗口的数据
            if (val.type == "r") {
                // 接收从load窗口选择的数据
                Dictionary<string, Dictionary<string, string>> choosedMaterial = val.data;
                ListBoxItem listBoxItem = new ListBoxItem();
                string materialName = choosedMaterial["materialName"]["content"];
                totalMaterials.Add(choosedMaterial);
                listBoxItem.Tag = totalMaterialsNum;
                listBoxItem.Content = (totalMaterialsNum + 1) + "\t" + materialName;
                totalMaterialsNum += 1;
                listBox.Items.Add(listBoxItem);
            }
        }

        private void findAllMaterialFromDb() {
            DataBaseMaterials = new List<Dictionary<string, Dictionary<string, string>>>();
            // 下边要被实际的库查询结果替代
            DataBaseMaterials.Add(new Dictionary<string, Dictionary<string, string>>() {
                { "mat", new Dictionary<string, string>() { { "RO", "0" }, { "PC", "0" }, { "MU", "0" }, { "TEROD", "0" }, { "CEROD", "0" }, { "YM", "0" }, { "PR", "0" } } },
                { "materialName",new Dictionary<string, string>() { {"content", "xy" } } },
                { "refer",new Dictionary<string, string>() { {"content", "我是引用" } } },
                { "soe",new Dictionary<string, string>() { { "C0", "0" }, { "C1", "0" }, { "C2", "0" }, { "C3", "0" }, { "C4", "0" }, { "C5", "0" }, { "C6", "0" }, { "E0", "0" }, { "V0", "0" } } },
                { "soeName",new Dictionary<string, string>() { {"content", "LINEAR_POLYNOMIAL" } } }
                , { "matName",new Dictionary<string, string>() { { "content", "NULL" } }}
            });
            DataBaseMaterials.Add(new Dictionary<string, Dictionary<string, string>>() {
                { "mat", new Dictionary<string, string>() { { "RO", "0" }, { "PC", "0" }, { "MU", "0" }, { "TEROD", "0" }, { "CEROD", "0" }, { "YM", "0" }, { "PR", "0" } } },
                { "materialName",new Dictionary<string, string>() { {"content", "bingbingbing" } } },
                { "refer",new Dictionary<string, string>() { {"content", "我是引用" } } },
                { "soe",new Dictionary<string, string>() { { "C0", "0" }, { "C1", "0" }, { "C2", "0" }, { "C3", "0" }, { "C4", "0" }, { "C5", "0" }, { "C6", "0" }, { "E0", "0" }, { "V0", "0" } } },
                { "soeName",new Dictionary<string, string>() { {"content", "LINEAR_POLYNOMIAL" } } }
                , { "matName",new Dictionary<string, string>() { { "content", "NULL" } }}
            });
            DataBaseMaterials.Add(new Dictionary<string, Dictionary<string, string>>() {
                { "mat", new Dictionary<string, string>() { { "RO", "0" }, { "G", "0" }, { "E", "0" }, { "PR", "0" }, { "DTF", "0" }, { "VP", "0" }, { "RATEOP", "0" }, { "A", "0" } , { "B", "0" } , { "N", "0" },
            {"C", "0"},{"M", "0"},{"TM", "0"},{"TR", ""},{"EPSO", ""},{"CP", ""},{"PC", ""},{"SPALL", ""},{"IT", ""},{"D1", ""},{"D2", ""},{"D3", ""},{"D4", ""},{"D5", ""},{"EROD", ""},{"EFMIN", ""},{"NUMINT", ""},{"C2/P", ""} } },
                { "materialName",new Dictionary<string, string>() { {"content", "feifeifei" } } },
                { "refer",new Dictionary<string, string>() { {"content", "我是引用" } } },
                { "soe",new Dictionary<string, string>() { { "C", "0" }, { "S1", "0" }, { "S2", "0" }, { "S3", "0" }, { "GAMAO", "0" }, { "A", "0" }, { "E0", "0" }, { "V0", "0" } } },
                { "soeName",new Dictionary<string, string>() { {"content", "GRUNEISEN" } } }
                , { "matName",new Dictionary<string, string>() { { "content", "JOHNSON_ COOK" } }}
            });
            DataBaseMaterials.Add(new Dictionary<string, Dictionary<string, string>>() {
                { "mat", new Dictionary<string, string>() { { "RO", "0" }, { "D", "0" }, { "PCJ", "0" }, { "BETA", "0" }, { "K", "0" }, { "G", "0" }, { "SIGY", "0" } } },
                { "materialName",new Dictionary<string, string>() { {"content", "jesolem" } } },
                { "refer",new Dictionary<string, string>() { {"content", "我是引用" } } },
                { "soe",new Dictionary<string, string>() { { "A", "0" }, { "B", "0" }, { "R1", "0" }, { "R2", "0" }, { "OMEG", "0" }, { "E0", "0" }, { "V0", "0" } } },
                { "soeName",new Dictionary<string, string>() { {"content", "JWL" } } }
                , { "matName",new Dictionary<string, string>() { { "content", "HIGH_EXPLOSIVE_BURN" } }}
            });
        }

        private void newMaterial(object sender, RoutedEventArgs e) {
            NewMaterialWindow nmw = new NewMaterialWindow();
            nmw.setPreUpdateMaterialData(0, null, -1);
            nmw.PassValuesEvent += new NewMaterialWindow.PassValuesHandler(receivedNewMaterialWindowData);
            nmw.Show();
        }

        // 删除一条,更新容器编号，tag值
        private void delMaterial(object sender, RoutedEventArgs e)
        {
            if (listBox.Items.Count == 0)
            {
                MessageBox.Show("操作有误");
                return;
            }
            int selectedIndex = listBox.SelectedIndex;
            if (selectedIndex == -1) {
                MessageBox.Show("请选择一个材料");
                return;
            }
            ListBoxItem lbi = listBox.SelectedItem as ListBoxItem;
            if (lbi != null) {
                int deleteIndex = Convert.ToInt32(lbi.Tag);
                // 更新totalMaterials及更新tag值
                totalMaterials.RemoveAt(deleteIndex);
                foreach (ListBoxItem _lbi in listBox.Items) {
                    int _index = Convert.ToInt32(_lbi.Tag);
                    if (_index > deleteIndex) {
                        _lbi.Tag = _index - 1;
                        _lbi.Content = _index + "\t" + totalMaterials[_index - 1]["materialName"]["content"];
                    }
                }
                listBox.Items.Remove(lbi);
                totalMaterialsNum -= 1;

            }
        }

        //更新该条的参数
        private void  updateMaterial(object sender, RoutedEventArgs e)
        {
            // 修改该条材料和新建材料使用同一个window
            NewMaterialWindow nmw = new NewMaterialWindow();
            ListBoxItem listBoxItem = (ListBoxItem)listBox.SelectedItem;
            if (listBoxItem == null) {
                return;
            }
            int index = Convert.ToInt32(listBoxItem.Tag);
            Dictionary<string, Dictionary<string, string>> currentChoosed = new Dictionary<string, Dictionary<string, string>>();
            foreach (KeyValuePair<string, Dictionary<string, string>> kvp in totalMaterials[index]) {
                Dictionary<string, string> _dic = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> _kvp in kvp.Value) {
                    _dic.Add(_kvp.Key, _kvp.Value);
                }
                currentChoosed.Add(kvp.Key, _dic);
            }
            nmw.setPreUpdateMaterialData(1, currentChoosed, index);
            nmw.PassValuesEvent += new NewMaterialWindow.PassValuesHandler(receivedUpdateMaterial);
            nmw.Show();
        }

        private void findMaterial(object sender, RoutedEventArgs e)
        {
            MaterialLoadWindow mlw = new MaterialLoadWindow();
            findAllMaterialFromDb();// 查找的数据放入DataBaseMaterials
            mlw.setCURDType("r", DataBaseMaterials);
            mlw.PassValuesEvent += new MaterialLoadWindow.PassValuesHandler(receivedLoadMaterialWindowData);
            mlw.Show();
        }

        private void saveMaterial(object sender, RoutedEventArgs e)
        {
            saveDataToFile(totalMaterials);
        }

        // 复制一条
        private void copyMaterial(object sender, RoutedEventArgs e) {
            if (listBox.Items.Count == 0)
            {
                MessageBox.Show("操作有误");
                return;
            }
            int selectedIndex = listBox.SelectedIndex;
            if (selectedIndex == -1)
            {
                MessageBox.Show("请选择一个材料");
                return;
            }
            ListBoxItem lbi = listBox.SelectedItem as ListBoxItem;
            if (lbi !=  null) {
                int copyIndex = Convert.ToInt32(lbi.Tag);
                Dictionary<string, Dictionary<string, string>> pasteMaterial  = new Dictionary<string, Dictionary<string, string>>();
                foreach (KeyValuePair<string, Dictionary<string, string>> kvp in totalMaterials[copyIndex])
                {
                    Dictionary<string, string> _dic = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> _kvp in kvp.Value)
                    {
                        _dic.Add(_kvp.Key, _kvp.Value);
                    }
                    pasteMaterial.Add(kvp.Key, _dic);
                }
                // 显示部分
                ListBoxItem pasteListBoxItem = new ListBoxItem();
                pasteListBoxItem.Tag = totalMaterialsNum;
                totalMaterialsNum += 1;
                pasteListBoxItem.Content = (totalMaterialsNum) + "\t" + pasteMaterial["materialName"]["content"];
                listBox.Items.Add(pasteListBoxItem);
                totalMaterials.Add(pasteMaterial);
            }
        }

        private void saveDataToFile(List<Dictionary<string, Dictionary<string, string>>> data)
        {
            //新建文件
            string filePath = System.Environment.CurrentDirectory + "\\data.txt";
            FileStream myFs = new FileStream(filePath, FileMode.Create);
            myFs.Close();

            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(string.Concat("*KEYWORD", Environment.NewLine));

            //解析list数据
            for (int i = 0; i < data.Count; i++)
            {
                Dictionary<string, Dictionary<string, string>> dic = data[i];
                string materialName = dic["materialName"]["content"];
                writer.Write(string.Concat("$" + materialName, Environment.NewLine));
                string materialMatName = dic["matName"]["content"];
                writer.Write(string.Concat("*MAT_" + materialMatName, Environment.NewLine));

                string parameterNameCat = "$";
                string parameterValueCat = "";
                Dictionary<string, string> matData = dic["mat"];
                for (int j = 0; j < matData.Keys.Count; j++)
                {
                    string key = matData.Keys.ElementAt(j);
                    string value = matData[key];
                    if (j % 8 == 0)
                    {
                        key = key.PadLeft(9, ' ');
                    }
                    else
                    {
                        key = key.PadLeft(10, ' ');
                    }
                    value = value.PadLeft(10, ' ');
                    parameterNameCat += key;
                    parameterValueCat += value;

                    if (j % 8 == 7)
                    {
                        writer.Write(string.Concat(parameterNameCat, Environment.NewLine));
                        writer.Write(string.Concat(parameterValueCat, Environment.NewLine));
                        parameterNameCat = "$";
                        parameterValueCat = "";
                    }
                    if (j == matData.Keys.Count - 1 && j % 8 != 7)
                    {
                        writer.Write(string.Concat(parameterNameCat, Environment.NewLine));
                        writer.Write(string.Concat(parameterValueCat, Environment.NewLine));
                    }
                }

                string materialEOSName = dic["soeName"]["content"];
                writer.Write(string.Concat("*EOS_" + materialEOSName, Environment.NewLine));

                string parameterNameCat2 = "$";
                string parameterValueCat2 = "";
                Dictionary<string, string> eosData = dic["soe"];
                for (int j = 0; j < eosData.Keys.Count; j++)
                {
                    string key = eosData.Keys.ElementAt(j);
                    string value = eosData[key];
                    if (j % 8 == 0)
                    {
                        key = key.PadLeft(9, ' ');
                    }
                    else
                    {
                        key = key.PadLeft(10, ' ');
                    }
                    value = value.PadLeft(10, ' ');
                    parameterNameCat2 += key;
                    parameterValueCat2 += value;

                    if (j % 8 == 7)
                    {
                        writer.Write(string.Concat(parameterNameCat2, Environment.NewLine));
                        writer.Write(string.Concat(parameterValueCat2, Environment.NewLine));
                        parameterNameCat2 = "$";
                        parameterValueCat2 = "";
                    }
                    if (j == eosData.Keys.Count - 1 && j % 8 != 7)
                    {
                        writer.Write(string.Concat(parameterNameCat2, Environment.NewLine));
                        writer.Write(string.Concat(parameterValueCat2, Environment.NewLine));
                    }
                }
                if (i != data.Count - 1)
                {
                    writer.Write(string.Concat("$", Environment.NewLine));
                    writer.Write(string.Concat("$", Environment.NewLine));
                }
            }
            writer.Write(string.Concat("*END", Environment.NewLine));
            writer.Close();
        }
    }
}
