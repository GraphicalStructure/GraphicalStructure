   M S B A M L   `   `   `  ����   GraphicalStructure'    !GraphicalStructure.DataBaseWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  DataBaseWindow��$	��450��$	��739��.��� .+��� -   $��listView��$	��280��$��10,0,384,10q� �� View �� -   gridview��.P��� $��  ID  �� �� DisplayMemberBinding ��  �� Path$ ID��� $��  Material  �� �� $ m_name��� $��  Density  �� �� $ 	m_density��� $��	  Color  �� �� $ m_color��� -   $��	dataTable��$
��Left=�$��
357,19,0,0q�$	��Top=�$	��120���� -   $��读数据表��$
��Left=�$��
508,21,0,0q�$	��Top=�$��75���� $��当前显示的数据：��$
��Left=�$��
352,55,0,0q�$	��Top=��� $��ID：��$
��Left=�$��
353,85,0,0q�$	��Top=��� -   $��	currentID��$
��Left=�$��23��$��
384,87,0,0q� �� TextWrapping$
 Wrap=�$	��Top=�$��63������  �� ElementName$ listView��$ SelectedItem.ID��� $��Material：��$
��Left=�$��353,115,0,0q�$	��Top=��� -   $��currentMaterial��$
��Left=�$��23��$��420,115,0,0q�$
 Wrap=�$	��Top=�$��91������ $ listView��$ SelectedItem.m_name��� $��
Density：��$
��Left=�$��354,145,0,0q�$	��Top=��� -   $��currentDensity��$
��Left=�$��23��$��417,147,0,0q�$
 Wrap=�$	��Top=�$	��116������ $ listView��$ SelectedItem.m_density��� $��Color：��$
��Left=�$��354,175,0,0q�$	��Top=��� -   $��currentColor��$
��Left=�$��23��$��406,177,0,0q�$
 Wrap=�$	��Top=�$��95������ $ listView��$ SelectedItem.m_color��� $��增加数据：��$
��Left=�$��
536,55,0,0q�$	��Top=��� $��ID：��$
��Left=�$��
537,85,0,0q�$	��Top=��� -	   $��addID��$
��Left=�$��23��$��
568,87,0,0q�$
 Wrap=�$�� ��$	��Top=�$��63���� $��Material：��$
��Left=�$��537,115,0,0q�$	��Top=��� -
   $��addMaterial��$
��Left=�$��23��$��604,115,0,0q�$
 Wrap=�$�� ��$	��Top=�$��91���� $��
Density：��$
��Left=�$��538,145,0,0q�$	��Top=��� -   $��
addDensity��$
��Left=�$��23��$��601,147,0,0q�$
 Wrap=�$�� ��$	��Top=�$	��116���� $��Color：��$
��Left=�$��538,175,0,0q�$	��Top=��� -   $��addColor��$
��Left=�$��23��$��590,177,0,0q�$
 Wrap=�$�� ��$	��Top=�$��94���� $ ��输入删除数据的ID：��$
��Left=�$��352,324,0,0q�$	��Top=��� $��ID：��$
��Left=�$��353,352,0,0q�$	��Top=��� -   $��deleteID��$
��Left=�$��23��$��384,354,0,0q�$
 Wrap=�$�� ��$	��Top=�$��63���� -   $��删除��$
��Left=�$��462,355,0,0q�$	��Top=�$��49���� -   $��添加��$
��Left=�$��581,216,0,0q�$	��Top=�$��50���� -   $��更新��$
��Left=�$��406,216,0,0q�$	��Top=�$��50���� -   $��查找��$
��Left=�$��498,288,0,0q�$	��Top=�$��50���� $$��输入查询数据的名称：��$
��Left=�$��350,257,0,0q�$	��Top=��� $��Material：��$
��Left=�$��352,285,0,0q�$	��Top=��� -    ��Name$ queryMaterial��$
��Left=�$��23��$��420,285,0,0q�$
 Wrap=�$�� ��$	��Top=�$��63���� $��×10³(kg/m³)��$
��Left=�$��636,145,0,0q�$	��Top=��� $��×10³(kg/m³)��$
��Left=�$��452,145,0,0q�$	��Top=�