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
using System.Collections;

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

        public delegate void PassValuesHandler(List<Dictionary<string, Dictionary<string, string>>> DataBaseMaterials);

        public event PassValuesHandler PassValuesEvent;

        private bool openDataBase;

        public MaterialDefinitionWindow()
        {
            totalMaterials = new List<Dictionary<string, Dictionary<string, string>>>();
            totalMaterialsNum = 0;
            InitializeComponent();

            adb = new UseAccessDB();
            openDataBase = adb.OpenDb();
        }

        public void receivedMaterialFromMainWindow(List<Dictionary<string, Dictionary<string, string>>> dict)
        {
            totalMaterials = dict;
            for (int i = 0; i < totalMaterials.Count; i++)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                Dictionary<string, Dictionary<string, string>> choosedMaterial = totalMaterials[i];
                string materialName = choosedMaterial["materialName"]["content"];
                listBoxItem.Tag = totalMaterialsNum;
                listBoxItem.Content = (totalMaterialsNum + 1) + "\t" + materialName;
                totalMaterialsNum += 1;
                listBox.Items.Add(listBoxItem);
            }
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

            ArrayList result = adb.queryALLMaterialFromTable("select * from Material");

            for (int i = 0; i < result.Count; i++)
            {
                Dictionary<string, Dictionary<string, string>> record = new Dictionary<string, Dictionary<string, string>>();

                ArrayList materialType = adb.queryALLMaterialFromTable("select * from Material_Type where material_type_name ='" + ((ArrayList)result[i])[2].ToString() + "'");
                string matName = ((ArrayList)materialType[0])[2].ToString();
                string eosName = ((ArrayList)materialType[0])[3].ToString();
                ArrayList matDataResult = adb.queryALLMaterialFromTable("select * from Mat_" + matName + " where MID ='" + ((ArrayList)result[i])[3] + "'");
                ArrayList eosDataResult = adb.queryALLMaterialFromTable("select * from Eos_" + eosName + " where EOSID ='" + ((ArrayList)result[i])[4] + "'");
                
                //组装数据
                //mat
                Dictionary<string, Dictionary<string, string>> mat = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> matData = new Dictionary<string,string>();
                record.Add("mat", matData);
                int count = ((ArrayList)matDataResult[0]).Count;
                int num = 1;
                //得到字段名
                List<string> matFieldName = adb.GetTableFieldNameList("Mat_" + matName);
                while (num < count)
                {
                    matData.Add(matFieldName[num], ((ArrayList)matDataResult[0])[num].ToString());
                    num++;
                }
                //eos
                Dictionary<string, Dictionary<string, string>> eos = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> eosData = new Dictionary<string, string>();
                record.Add("soe", eosData);
                count = ((ArrayList)eosDataResult[0]).Count;
                List<string> eosFieldName = adb.GetTableFieldNameList("Eos_" + eosName);
                num = 1;
                while (num < count)
                {
                    eosData.Add(eosFieldName[num], ((ArrayList)eosDataResult[0])[num].ToString());
                    num++;
                }

                //materialName
                Dictionary<string, Dictionary<string, string>> materialName = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> mnData = new Dictionary<string, string>();
                mnData.Add("content", ((ArrayList)result[i])[1].ToString());
                record.Add("materialName", mnData);

                //refer
                Dictionary<string, Dictionary<string, string>> refer = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> referData = new Dictionary<string, string>();
                referData.Add("content", ((ArrayList)result[i])[5].ToString());
                record.Add("refer", referData);

                //matName
                Dictionary<string, Dictionary<string, string>> mat_name = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> m_nData = new Dictionary<string, string>();
                m_nData.Add("content", matName.ToString());
                record.Add("matName", m_nData);

                //eosName
                Dictionary<string, Dictionary<string, string>> eos_name = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> e_nData = new Dictionary<string, string>();
                e_nData.Add("content", eosName.ToString());
                record.Add("soeName", e_nData);

                //density
                Dictionary<string, Dictionary<string, string>> density = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> denData = new Dictionary<string, string>();
                denData.Add("content", ((ArrayList)result[i])[6].ToString());
                record.Add("density", denData);

                //color
                Dictionary<string, Dictionary<string, string>> color = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> colorData = new Dictionary<string, string>();
                colorData.Add("content", ((ArrayList)result[i])[7].ToString());
                record.Add("color", colorData);

                DataBaseMaterials.Add(record);
            }
        }

        private void newMaterial(object sender, RoutedEventArgs e) {
            if (!openDataBase)
            {
                MessageBox.Show("打开数据库失败！","警告");
                return;
            }
            NewMaterialWindow nmw = new NewMaterialWindow();
            nmw.setPreUpdateMaterialData(0, null, -1);
            nmw.PassValuesEvent += new NewMaterialWindow.PassValuesHandler(receivedNewMaterialWindowData);
            nmw.Show();
            
        }

        // 删除一条,更新容器编号，tag值
        private void delMaterial(object sender, RoutedEventArgs e)
        {
            if (!openDataBase)
            {
                MessageBox.Show("打开数据库失败！", "警告");
                return;
            }
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
            if (!openDataBase)
            {
                MessageBox.Show("打开数据库失败！", "警告");
                return;
            }
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
            if (!openDataBase)
            {
                MessageBox.Show("打开数据库失败！", "警告");
                return;
            }
            MaterialLoadWindow mlw = new MaterialLoadWindow();
            findAllMaterialFromDb();// 查找的数据放入DataBaseMaterials
            mlw.setCURDType("r", DataBaseMaterials);
            mlw.PassValuesEvent += new MaterialLoadWindow.PassValuesHandler(receivedLoadMaterialWindowData);
            mlw.Show();
        }

        private void saveMaterial(object sender, RoutedEventArgs e)
        {
            if (!openDataBase)
            {
                MessageBox.Show("打开数据库失败！", "警告");
                return;
            }
            saveDataToFile(totalMaterials);

            addMaterialToDataBase(totalMaterials);
        }

        // 复制一条
        private void copyMaterial(object sender, RoutedEventArgs e) {
            if (!openDataBase)
            {
                MessageBox.Show("打开数据库失败！", "警告");
                return;
            }
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

            PassValuesEvent(this.totalMaterials);

            this.Close();
        }

        private void addMaterialToDataBase(List<Dictionary<string, Dictionary<string, string>>> data)
        {
            //只添加新创建的材料，已有的材料就不添加了
            ArrayList result = adb.queryALLMaterialFromTable("select * from Material");
            
            for (int i = 0; i < data.Count; i++)
            {
                bool isHaveMaterial = false;
                string materialName = ((Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)data[i])["materialName"])["content"];
                
                for (int j = 0; j < result.Count; j++)
                {
                    if (materialName == ((ArrayList)result[j])[1].ToString())
                    {
                        isHaveMaterial = true;
                        break;
                    }
                }

                if (isHaveMaterial == false)
                {
                    string matName = ((Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)data[i])["matName"])["content"];
                    string eosName = ((Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)data[i])["soeName"])["content"];
                    Dictionary<string, string> matData = (Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)data[i])["mat"];
                    Dictionary<string, string> eosData = (Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)data[i])["soe"];
                    string refer = ((Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)data[i])["refer"])["content"];
                    string density = ((Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)data[i])["density"])["content"];
                    string color = ((Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)data[i])["color"])["content"];

                    //新增一条材料   需要更新三个表
                    bool success;
                    string matTableName = "Mat_" + matName;
                    string eosTableName = "Eos_" + eosName;

                    //先获取mat表中最后一条数据的id
                    ArrayList mat_ = adb.queryALLMaterialFromTable("select * from " + matTableName);
                    int mid = 0;
                    if(mat_.Count != 0)
                       mid = Int32.Parse(((ArrayList)mat_[mat_.Count - 1])[1].ToString());
                    //先获取eos表中最后一条数据的id
                    ArrayList eos_ = adb.queryALLMaterialFromTable("select * from " + eosTableName);
                    int eosid = 0;
                    if(eos_.Count != 0)
                       eosid = Int32.Parse(((ArrayList)eos_[eos_.Count - 1])[1].ToString());
                    //组装sql语句
                    //插入mat表
                    string matSql1 = "insert into " + matTableName + "(" + "MID,";
                    string matSql2 = " values(" + (mid+1).ToString() + ",";
                    for (int k = 0; k < matData.Count; k++)
                    {
                        matSql1 += matData.Keys.ElementAt(k);
                        if (k != matData.Count - 1)
                            matSql1 += ",";
                        else
                            matSql1 += ")";

                        matSql2 += matData[matData.Keys.ElementAt(k)];
                        if (k != matData.Count - 1)
                            matSql2 += ",";
                        else
                            matSql2 += ")";
                    }
                    string matsql = matSql1 + matSql2;
                    success = adb.insertTableData(matsql);
                    if (success)
                        Console.WriteLine("插入mat表成功");
                    else
                        Console.WriteLine("插入mat表失败");

                    //插入eos表
                    string eosSql1 = "insert into " + eosTableName + "(" + "EOSID,";
                    string eosSql2 = " values(" + (eosid + 1).ToString() + ",";
                    for (int l = 0; l < eosData.Count; l++)
                    {
                        eosSql1 += eosData.Keys.ElementAt(l);
                        if (l != eosData.Count - 1)
                            eosSql1 += ",";
                        else
                            eosSql1 += ")";

                        eosSql2 += eosData[eosData.Keys.ElementAt(l)];
                        if (l != eosData.Count - 1)
                            eosSql2 += ",";
                        else
                            eosSql2 += ")";
                    }
                    string eosSql = eosSql1 + eosSql2;
                    success = adb.insertTableData(eosSql);
                    if (success)
                        Console.WriteLine("插入eos表成功");
                    else
                        Console.WriteLine("插入eos表失败");

                    //插入material表
                    //得到material表字段名
                    List<string> matFieldName = adb.GetTableFieldNameList("Material");
                    string materialSql1 = "insert into Material(";
                    string materialSql2 = " values(";
                    for (int m = 1; m < matFieldName.Count; m++)
                    {
                        materialSql1 += matFieldName[m];
                        if (m != matFieldName.Count - 1)
                            materialSql1 += ",";
                        else
                            materialSql1 += ")";
                    }
                    ArrayList materialType = adb.queryALLMaterialFromTable("select * from Material_Type where mat_name ='" + matName + "'");
                    string material_type_name = ((ArrayList)materialType[0])[1].ToString();
                    materialSql2 += "'"+ materialName + "','" + material_type_name + "','" + (mid + 1).ToString() + "','" + (eosid + 1).ToString() + "','" + refer + "','" +density + "','"+color+"')";
                    string materialSql = materialSql1 + materialSql2;
                    success = adb.insertTableData(materialSql);
                    if (success)
                        Console.WriteLine("插入materila表成功");
                    else
                        Console.WriteLine("插入materila表失败");
                }
            }
        }
    }
}
