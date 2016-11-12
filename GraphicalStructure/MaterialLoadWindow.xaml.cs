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
    /// MaterialLoadWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MaterialLoadWindow : Window
    {
        public delegate void PassValuesHandler(object sender, Validity data);
        public event PassValuesHandler PassValuesEvent;

        private UseAccessDB adb;

        public MaterialLoadWindow()
        {
            // 添加逻辑：初始时查库，得到所有material，及其数据
            findFromDataBaseMaterials = new List<Dictionary<string, Dictionary<string, string>>>();
            updatedMaterialIndexInfindFromDataBaseMaterials = new HashSet<int>();
            deleteMaterialIndexInfindFromDataBaseMaterials = new HashSet<int>();
            updatedMaterials = new Dictionary<int, Dictionary<string, Dictionary<string, string>>>();
            currentClickRowOfListBox = -1;
            isUpdate = false;
            InitializeComponent();

            adb = new UseAccessDB();
            adb.OpenDb();
        } 
        // curd类型
        private string curdType;

        //到更新的界面时点击取消时为false，点击确定时为true,只有当isUpdate为true时才实施更新操作
        private Boolean isUpdate;

        //一次修改一条数据, 选择一条数据
        private Dictionary<string, Dictionary<string, string>> currentMaterial;


        // 从库中查得的数据，未经修改，修改之后放入updatedMaterials， ListBoxItem指某行listBox
        private List<Dictionary<string, Dictionary<string, string>>> findFromDataBaseMaterials;

        private List<Dictionary<string, Dictionary<string, string>>> sortedMaterials;

        private int currentClickRowOfListBox;

        // int 指修改了第几行ListBoxItem的数据，Dictionary<string, Dictionary<string, string>>指一条数据
        private Dictionary<int, Dictionary<string, Dictionary<string, string>>> updatedMaterials;

        private HashSet<int> updatedMaterialIndexInfindFromDataBaseMaterials;
        private HashSet<int> deleteMaterialIndexInfindFromDataBaseMaterials;

        private Dictionary<string, string> gatherMat;

        private Dictionary<string, string> gatherSoe;

        private void ReceivedVals(object sender, Validity val) {
            if (val.isConfirm) {
                isUpdate = true;
                if (val.type == "materialName") {
                    // 修改材料名及参考文献
                    string materialName = val.data["materialName"]["content"];
                    string refer = val.data["refer"]["content"];
                    currentMaterial["materialName"]["content"] = materialName;
                    currentMaterial["refer"]["content"] = refer;
                    ((ListBoxItem)listBox_materialName.SelectedItem).Content = materialName;
                    // 将修改的material统计起来
                    int _index = Convert.ToInt32(((ListBoxItem)listBox_materialName.SelectedItem).Tag);
                    if (updatedMaterialIndexInfindFromDataBaseMaterials.Contains(_index))
                    {
                        updatedMaterials[_index]["materialName"]["content"] = materialName;
                        updatedMaterials[_index]["refer"]["content"] = refer;
                    }
                    else {
                        updatedMaterialIndexInfindFromDataBaseMaterials.Add(_index);
                        updatedMaterials.Add(_index, new Dictionary<string, Dictionary<string, string>>(currentMaterial));
                    }
                    
                } else if (val.type == "mat") {
                    // 修改mat模型参数
                    gatherMat = val.data["mat"];
                    currentMaterial["mat"] = gatherMat;
                    int _index = Convert.ToInt32(((ListBoxItem)listBox_materialName.SelectedItem).Tag);
                    if (updatedMaterialIndexInfindFromDataBaseMaterials.Contains(_index))
                    {
                        updatedMaterials[_index]["mat"] = new Dictionary<string, string>(currentMaterial["mat"]);
                    }
                    else
                    {
                        updatedMaterialIndexInfindFromDataBaseMaterials.Add(_index);
                        updatedMaterials.Add(_index, new Dictionary<string, Dictionary<string, string>>(currentMaterial));
                    }
                }
                else if(val.type == "soe"){
                    // 修改soe参数
                    gatherSoe = val.data["soe"];
                    currentMaterial["soe"] = gatherSoe;
                    int _index = Convert.ToInt32(((ListBoxItem)listBox_materialName.SelectedItem).Tag);
                    if (updatedMaterialIndexInfindFromDataBaseMaterials.Contains(_index))
                    {
                        updatedMaterials[_index]["soe"] = new Dictionary<string, string>(currentMaterial["soe"]);
                    }
                    else
                    {
                        updatedMaterialIndexInfindFromDataBaseMaterials.Add(_index);
                        updatedMaterials.Add(_index, new Dictionary<string, Dictionary<string, string>>(currentMaterial));
                    }
                }
            }
        }


        public void setCURDType(string t, List<Dictionary<string, Dictionary<string, string>>> list) {
            // 设置是在做什么动作，curd
            this.curdType = t;
            if (t == "r")
            {
                // retrive
                button_curd.Content = "确定";

            }
            findFromDataBaseMaterials = new List<Dictionary<string, Dictionary<string, string>>>(list.ToArray<Dictionary<string, Dictionary<string, string>>>());
            int index = 0;
            foreach (Dictionary<string, Dictionary<string, string>> dict in findFromDataBaseMaterials) {
                ListBoxItem materialNameListBoxItem = new ListBoxItem();
                materialNameListBoxItem.Content = dict["materialName"]["content"];
                // Tag中存放该条数据在findFromDataBaseMaterials中的位置， 从0开始
                materialNameListBoxItem.Tag = index;
                MouseButtonEventHandler materialNameListBoxItem_MouseDoubleHandler = new MouseButtonEventHandler(updateMaterialName);
                materialNameListBoxItem.MouseDoubleClick += materialNameListBoxItem_MouseDoubleHandler;
                listBox_materialName.Items.Add(materialNameListBoxItem);

                ListBoxItem matNameListBoxItem = new ListBoxItem();
                // Tag中存放该条数据在findFromDataBaseMaterials中的位置
                matNameListBoxItem.Tag = index;
                MouseButtonEventHandler matNameListBoxItem_MouseDoubleHandler = new MouseButtonEventHandler(updateMat);
                matNameListBoxItem.MouseDoubleClick += matNameListBoxItem_MouseDoubleHandler;
                matNameListBoxItem.Content = dict["matName"]["content"];
                listBox_matName.Items.Add(matNameListBoxItem);

                ListBoxItem soeNameListBoxItem = new ListBoxItem();
                // Tag中存放该条数据在findFromDataBaseMaterials中的位置
                soeNameListBoxItem.Tag = index;
                MouseButtonEventHandler soeNameListBoxItem_MouseDoubleHandler = new MouseButtonEventHandler(updateSoe);
                soeNameListBoxItem.MouseDoubleClick += soeNameListBoxItem_MouseDoubleHandler;
                soeNameListBoxItem.Content = dict["soeName"]["content"];
                listBox_soeName.Items.Add(soeNameListBoxItem);
                index++;
            }
        }


        // 双击ListBoxItem_materialName进入该材料的编辑界面
        private void updateMaterialName(object sender, RoutedEventArgs e) {
            if (curdType == "u") {
                currentClickRowOfListBox = listBox_materialName.SelectedIndex; // 从0开始索引
                // 改变currentMaterial，未改变findFromDataBaseMaterials
                // 如果已经修改过其他两个属性，则不去new
                if (currentMaterial == null) {
                    currentMaterial = new Dictionary<string, Dictionary<string, string>>(findFromDataBaseMaterials[currentClickRowOfListBox]);
                }
                string materialName = currentMaterial["materialName"]["content"];
                string refer = currentMaterial["refer"]["content"];
                EditMaterialAttrWindow edaw = new EditMaterialAttrWindow();
                edaw.setMaterialName(materialName, refer);
                edaw.Topmost = true;
                edaw.PassValuesEvent += new EditMaterialAttrWindow.PassValuesHandler(ReceivedVals);
                edaw.Show();
            }
        }

        // 双击ListBoxItem_matName进入该材料的编辑界面
        private void updateMat(object sender, RoutedEventArgs e) {
            ListBoxItem listBoxItem = sender as ListBoxItem;
            if (curdType == "u")
            {
                int _index = Convert.ToInt32(listBoxItem.Tag);
                currentClickRowOfListBox = listBox_matName.SelectedIndex;
                EditMaterialAttrWindow edaw = new EditMaterialAttrWindow();
                // 如果已经修改过其他两个属性，则不去new
                if (currentMaterial == null)
                {
                    currentMaterial = new Dictionary<string, Dictionary<string, string>>(findFromDataBaseMaterials[_index]);
                }
                Dictionary<string, string> matData = new Dictionary<string, string>(currentMaterial["mat"]);
                edaw.PassValuesEvent += new EditMaterialAttrWindow.PassValuesHandler(ReceivedVals);
                edaw.setMat(matData);
                edaw.Topmost = true;
                edaw.Show();
            }
        }
        // 双击ListBoxItem_soeName进入该材料的编辑界面
        private void updateSoe(object sender, RoutedEventArgs e)
        {
            if (curdType == "u" && ((System.Windows.Controls.ListBoxItem)sender).Content.ToString() != "") {
                currentClickRowOfListBox = listBox_soeName.SelectedIndex;
                EditMaterialAttrWindow edaw = new EditMaterialAttrWindow();
                // 如果已经修改过其他两个属性，则不去new
                if (currentMaterial == null)
                {
                    currentMaterial = new Dictionary<string, Dictionary<string, string>>(findFromDataBaseMaterials[currentClickRowOfListBox]);
                }
                Dictionary<string, string> soeData = new Dictionary<string, string>(currentMaterial["soe"]);
                edaw.PassValuesEvent += new EditMaterialAttrWindow.PassValuesHandler(ReceivedVals);
                edaw.setSoe(soeData);
                edaw.Topmost = true;
                edaw.Show();
            }
        }

        private void selectionHandle(object sender, RoutedEventArgs e) {
            ListBox selectedListBox = (ListBox)sender;
            currentClickRowOfListBox = selectedListBox.SelectedIndex;
            if (selectedListBox.Name == "listBox_materialName")
            {
                listBox_matName.SelectedIndex = currentClickRowOfListBox;
                listBox_soeName.SelectedIndex = currentClickRowOfListBox;
            } else if (selectedListBox.Name == "listBox_matName") {
                listBox_materialName.SelectedIndex = currentClickRowOfListBox;
                listBox_soeName.SelectedIndex = currentClickRowOfListBox;
            } else if (selectedListBox.Name == "listBox_soeName") {
                listBox_materialName.SelectedIndex = currentClickRowOfListBox;
                listBox_matName.SelectedIndex = currentClickRowOfListBox;
            }
        }

        // 删除该条ListBoxItem，并从数据库中删除
        private void deleteMaterialInDatabase(object sender, RoutedEventArgs e) {
            if (currentClickRowOfListBox == -1) {
                return;
            }
            ListBoxItem currentClickMaterialNameItem = (ListBoxItem)(listBox_materialName.Items[currentClickRowOfListBox]);
            ListBoxItem currentClickMatNameItem = (ListBoxItem)(listBox_matName.Items[currentClickRowOfListBox]);
            ListBoxItem currentClickSoeNameItem = (ListBoxItem)(listBox_soeName.Items[currentClickRowOfListBox]);
            // 删除
            //从数据库中删除
            //维持findFromDataBaseMaterials不变，只是统计删除ListBoxItem的tag值，最后再调用接口删除
            int index = Convert.ToInt32(currentClickMaterialNameItem.Tag);
            //findFromDataBaseMaterials.RemoveAt(index);
            if (!deleteMaterialIndexInfindFromDataBaseMaterials.Contains(index)) {
                deleteMaterialIndexInfindFromDataBaseMaterials.Add(index);
            }
            // 最终从数据库中删除哪些material从findFromDataBaseMaterials和deleteMaterialIndexInfindFromDataBaseMaterials得出
            // 在关闭本窗口前从数据库中删除！！！

            //从数据库中删除数据
            for (int i = 0; i < deleteMaterialIndexInfindFromDataBaseMaterials.Count; i++)
            {
                //要从三个表中删除相应数据
                //根据material表获取另外两个表的id
                int temp = deleteMaterialIndexInfindFromDataBaseMaterials.ElementAt(i);
                Dictionary<string, string> mat = ((Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)findFromDataBaseMaterials[temp])["mat"]);
                string matId = mat["MID"];
                Dictionary<string, string> eos = ((Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)findFromDataBaseMaterials[temp])["soe"]);
                string eosId = eos["EOSID"];
                string materialName = ((Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)findFromDataBaseMaterials[temp])["materialName"])["content"];
                string matName = ((Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)findFromDataBaseMaterials[temp])["matName"])["content"];
                string eosName = ((Dictionary<string, string>)((Dictionary<string, Dictionary<string, string>>)findFromDataBaseMaterials[temp])["soeName"])["content"];

                string sql = "delete from Material where material_name = '" + materialName + "'";
                bool success = adb.deleteFromTableData(sql, 0);
                if (success)
                    Console.WriteLine("从material表中删除成功");
                else
                    Console.WriteLine("从material表中删除失败");

                string matTableName = "Mat_" + matName;
                success = adb.deleteFromTableData("delete from "+matTableName+" where MID = '" + matId + "'", 0);
                if (success)
                    Console.WriteLine("从mat表中删除成功");
                else
                    Console.WriteLine("从mat表中删除失败");

                string eosTableName = "Eos_" + eosName;
                success = adb.deleteFromTableData("delete from " + eosTableName + " where EOSID = '" + eosId +"'", 0);
                if (success)
                    Console.WriteLine("从eos表中删除成功");
                else
                    Console.WriteLine("从eos表中删除失败");
            }

            // end
            // 从ListBox中删除
            listBox_materialName.Items.Remove(currentClickMaterialNameItem);
            listBox_matName.Items.Remove(currentClickMatNameItem);
            listBox_soeName.Items.Remove(currentClickSoeNameItem);

        }

        // load界面确定键点击之后处理逻辑
        private void confirmButtonClick(object sender, RoutedEventArgs e) {
            if (curdType == "u")
            {
                if (isUpdate)
                {
                    // 保存已修改的数据,确认保存，保存修改的数据至数据库
                    // 保存后不关闭次窗口，可继续修改

                    // 注意：改变顺序之后就不是了
                    // ==========================需要修改
                    // 更改的数据不应该放入findFromDataBaseMaterials
                    // 涉及到排序，改变
                    // 这里需要改
                    foreach(int i in updatedMaterialIndexInfindFromDataBaseMaterials) {
                        Dictionary<string, Dictionary<string, string>> _mar = updatedMaterials[i];
                        foreach (KeyValuePair<string, Dictionary<string, string>> kvp in _mar)
                        {
                            Dictionary<string, string> _v = new Dictionary<string, string>();
                            foreach (KeyValuePair<string, string> _kvp in kvp.Value)
                            {
                                _v.Add(_kvp.Key, _kvp.Value);
                            }
                            findFromDataBaseMaterials[i][kvp.Key] = _v;
                        }
                    }
                    // end

                    // 入库代码, 根据updatedMaterials中的数据写回库中

                    // end
                    MessageBox.Show("修改成功");
                }
                else {
                    MessageBox.Show("no change!");
                }
                
            }
            else if (curdType == "d")
            { // 处理删除逻辑，根据当前选中的行
              // 删除后不关闭此窗口，可继续删除
                int deleteRow = listBox_materialName.SelectedIndex;
                if (deleteRow > -1) {
                    listBox_materialName.Items.RemoveAt(deleteRow);
                    listBox_matName.Items.RemoveAt(deleteRow);
                    listBox_soeName.Items.RemoveAt(deleteRow);
                    // 直接删除库中数据，根据tag中保存的index在findFromDataBaseMaterials中找到对应数据，
                    // 然后根据本条数据的主键删除数据库中数据。
                    // 删除时findFromDataBaseMaterials不变，仍然保留，如果变了会影响index
                    // 在下一次重新进入loadwindow时，会重新从库中拉去数据
                    MessageBox.Show("删除成功");
                }
            }
            else if (curdType == "r") {
                // 处理查找逻辑，根据当前选中的行，返回一行数据至definition window.
                // 点击确定之后，返回数据，并关闭窗口
                if (currentClickRowOfListBox == -1) {
                    return;
                }
                Validity val = new Validity();
                val.type = "r";
                val.isConfirm = true;
                val.data = new Dictionary<string, Dictionary<string, string>>();
                foreach (KeyValuePair<string, Dictionary<string, string>> item in findFromDataBaseMaterials[currentClickRowOfListBox])
                {
                    Dictionary<string, string> copyItemValue = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> _item in item.Value) {
                        copyItemValue.Add(_item.Key, _item.Value);
                    }
                    val.data.Add(item.Key, copyItemValue);
                }
                PassValuesEvent(this, val);
                // 点击选择数据确定前，删除数据
                if (deleteMaterialIndexInfindFromDataBaseMaterials.Count > 0)
                {
                    foreach (int i in deleteMaterialIndexInfindFromDataBaseMaterials)
                    {
                        // 根据数据库接口补充完整
                        // delete->findFromDataBaseMaterials[i];
                    }
                }
                Close();
            }
        }

        // load界面取消键点击之后处理逻辑
        private void cancelButtonClick(object sender, RoutedEventArgs e) {
            // 点击取消是，若之前有删除动作，则从deleteMaterialIndexInfindFromDataBaseMaterials中找到删除
            if (deleteMaterialIndexInfindFromDataBaseMaterials.Count > 0) {
                foreach (int i in deleteMaterialIndexInfindFromDataBaseMaterials) {
                    // 根据数据库接口补充完整
                    // delete->findFromDataBaseMaterials[i];
                }
            }
            Close();
        }

        /*
        private static int sortedAlphaBeta(Dictionary<string, Dictionary<string, string>> dict1, Dictionary<string, Dictionary<string, string>> dict2) {
            int res = 0;
            if () {

            }
        }*/


        private void sortedByMaterialName(object sender, EventArgs e) {
            // materialsForSort不包含删除的
            List<Dictionary<string, Dictionary<string, string>>> materialsCopy = new List<Dictionary<string, Dictionary<string, string>>>(findFromDataBaseMaterials);
            List<Dictionary<string, Dictionary<string, string>>> materialsForSort = new List<Dictionary<string, Dictionary<string, string>>>();
            for(int i = 0; i < findFromDataBaseMaterials.Count; i++) {
                if(!deleteMaterialIndexInfindFromDataBaseMaterials.Contains(i))
                {
                    materialsForSort.Add(materialsCopy[i]);
                }
            }
            sortedMaterials = new List<Dictionary<string, Dictionary<string, string>>>();
            if (expander_materialName.IsExpanded)
            {
                // 由小到大
                IEnumerable<Dictionary<string, Dictionary<string, string>>> query = null;
                query = from item in materialsForSort orderby item["materialName"]["content"]  select item;
                foreach (var item in query){
                    sortedMaterials.Add(item);
                }
            }
            else {
                // 由大到小
                IEnumerable<Dictionary<string, Dictionary<string, string>>> query = null;
                query = from item in materialsForSort orderby item["materialName"]["content"] descending select item;
                foreach (var item in query)
                {
                    sortedMaterials.Add(item);
                }
            }

            // 显示部分
            listBox_materialName.Items.Clear();
            listBox_matName.Items.Clear();
            listBox_soeName.Items.Clear();
            for (int i = 0; i < sortedMaterials.Count; i++) {
                // materialsCopy的index和findFromDataBaseMaterials中一样，因此根据index从findFromDataBaseMaterials中取出
                int index = materialsCopy.IndexOf(sortedMaterials[i]);
                ListBoxItem lbi_materialName = new ListBoxItem();
                lbi_materialName.Content = sortedMaterials[i]["materialName"]["content"];
                lbi_materialName.Tag = index;
                listBox_materialName.Items.Add(lbi_materialName);
                ListBoxItem lbi_matName = new ListBoxItem();
                lbi_matName.Content = sortedMaterials[i]["matName"]["content"];
                lbi_matName.Tag = index;
                listBox_matName.Items.Add(lbi_matName);
                ListBoxItem lbi_soeName = new ListBoxItem();
                lbi_soeName.Content = sortedMaterials[i]["soeName"]["content"];
                lbi_soeName.Tag = index;
                listBox_soeName.Items.Add(lbi_soeName);
            }
        }

        private void sortedByMatName(object sender, EventArgs e)
        {
            // materialsForSort不包含删除的
            List<Dictionary<string, Dictionary<string, string>>> materialsCopy = new List<Dictionary<string, Dictionary<string, string>>>(findFromDataBaseMaterials);
            List<Dictionary<string, Dictionary<string, string>>> materialsForSort = new List<Dictionary<string, Dictionary<string, string>>>();
            for (int i = 0; i < findFromDataBaseMaterials.Count; i++)
            {
                if (!deleteMaterialIndexInfindFromDataBaseMaterials.Contains(i))
                {
                    materialsForSort.Add(materialsCopy[i]);
                }
            }
            sortedMaterials = new List<Dictionary<string, Dictionary<string, string>>>();
            if (expander_matName.IsExpanded)
            {
                // 由小到大
                IEnumerable<Dictionary<string, Dictionary<string, string>>> query = null;
                query = from item in materialsForSort orderby item["matName"]["content"] select item;
                foreach (var item in query)
                {
                    sortedMaterials.Add(item);
                }
            }
            else
            {
                // 由大到小
                IEnumerable<Dictionary<string, Dictionary<string, string>>> query = null;
                query = from item in materialsForSort orderby item["matName"]["content"] descending select item;
                foreach (var item in query)
                {
                    sortedMaterials.Add(item);
                }
            }

            // 显示部分
            listBox_materialName.Items.Clear();
            listBox_matName.Items.Clear();
            listBox_soeName.Items.Clear();
            for (int i = 0; i < sortedMaterials.Count; i++)
            {
                // materialsCopy的index和findFromDataBaseMaterials中一样，因此根据index从findFromDataBaseMaterials中取出
                int index = materialsCopy.IndexOf(sortedMaterials[i]);
                ListBoxItem lbi_materialName = new ListBoxItem();
                lbi_materialName.Content = sortedMaterials[i]["materialName"]["content"];
                lbi_materialName.Tag = index;
                listBox_materialName.Items.Add(lbi_materialName);
                ListBoxItem lbi_matName = new ListBoxItem();
                lbi_matName.Content = sortedMaterials[i]["matName"]["content"];
                lbi_matName.Tag = index;
                listBox_matName.Items.Add(lbi_matName);
                ListBoxItem lbi_soeName = new ListBoxItem();
                lbi_soeName.Content = sortedMaterials[i]["soeName"]["content"];
                lbi_soeName.Tag = index;
                listBox_soeName.Items.Add(lbi_soeName);
            }
        }

        private void sortedBySoeName(object sender, EventArgs e)
        {
            // materialsForSort不包含删除的
            List<Dictionary<string, Dictionary<string, string>>> materialsCopy = new List<Dictionary<string, Dictionary<string, string>>>(findFromDataBaseMaterials);
            List<Dictionary<string, Dictionary<string, string>>> materialsForSort = new List<Dictionary<string, Dictionary<string, string>>>();
            for (int i = 0; i < findFromDataBaseMaterials.Count; i++)
            {
                if (!deleteMaterialIndexInfindFromDataBaseMaterials.Contains(i))
                {
                    materialsForSort.Add(materialsCopy[i]);
                }
            }
            sortedMaterials = new List<Dictionary<string, Dictionary<string, string>>>();
            if (expander_soeName.IsExpanded)
            {
                // 由小到大
                IEnumerable<Dictionary<string, Dictionary<string, string>>> query = null;
                query = from item in materialsForSort orderby item["soeName"]["content"] select item;
                foreach (var item in query)
                {
                    sortedMaterials.Add(item);
                }
            }
            else
            {
                // 由大到小
                IEnumerable<Dictionary<string, Dictionary<string, string>>> query = null;
                query = from item in materialsForSort orderby item["soeName"]["content"] descending select item;
                foreach (var item in query)
                {
                    sortedMaterials.Add(item);
                }
            }

            // 显示部分
            listBox_materialName.Items.Clear();
            listBox_matName.Items.Clear();
            listBox_soeName.Items.Clear();
            for (int i = 0; i < sortedMaterials.Count; i++)
            {
                // materialsCopy的index和findFromDataBaseMaterials中一样，因此根据index从findFromDataBaseMaterials中取出
                int index = materialsCopy.IndexOf(sortedMaterials[i]);
                ListBoxItem lbi_materialName = new ListBoxItem();
                lbi_materialName.Content = sortedMaterials[i]["materialName"]["content"];
                lbi_materialName.Tag = index;
                listBox_materialName.Items.Add(lbi_materialName);
                ListBoxItem lbi_matName = new ListBoxItem();
                lbi_matName.Content = sortedMaterials[i]["matName"]["content"];
                lbi_matName.Tag = index;
                listBox_matName.Items.Add(lbi_matName);
                ListBoxItem lbi_soeName = new ListBoxItem();
                lbi_soeName.Content = sortedMaterials[i]["soeName"]["content"];
                lbi_soeName.Tag = index;
                listBox_soeName.Items.Add(lbi_soeName);
            }
        }
    }
}
