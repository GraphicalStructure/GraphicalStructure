   M S B A M L   `   `   `  ����   GraphicalStructure*    $GraphicalStructure.NewMaterialWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  输入材料属性��$	��500��$	��500��.���  � ColumnDefinitions �� $
��144* ��� $	��25* ��� $
��291* ��� $	��32* �.+��� -    ��Name$ label_materialName��$��材料名称��$��25��$��
25,30,15,0q�$	��Top=��� -   $ label_matName��$��强度模型��$��25��$��
25,80,15,0q�$	��Top=��� -   $ label_soeName��$��状态方程��$��25,130,15,0q�$��25��$	��Top=��� -   $ label_reference��$��引用文献��$��25,181,15,160q��� -    ��Name$ textBox_materialName��$��2��$��20��$��14.561,35,49,0q� �� TextWrapping$
 Wrap=�$	��Top=�$��0,0,0,-2q� � UseLayoutRounding . $��13.333(��� -    ��Name$ MatComboBox��$��2��$��20��$��
15,82,50,0q�$	��Top=� �� 
IsReadOnly . .@��� $
��NULL���� $��HIGH_EXPLOSIVE_BURN���� $��JOHNSON_COOK���� $��PLASTIC_KINEMATIC���� -   $ textBox_soe��$��2��$��20��$��15,135,50,0q�$
 Wrap=�$	��Top=� �� 
IsReadOnly . 	 �� IsUndoEnabled	 .  �� -   $ textBox_refer��$��2��$��15,181,50,160q�$
 Wrap=��� -	   
 ��Name$
 button��$
��edit��$��2��$��Right=�$��0,87,5,0q�   SimpleButton#���   $	��Top=�$��40��$��18�� J� RenderTransformOrigin$ 	0.5,0.444&����	#FF878787$��0q�����  �� EndPoint$	 0,1&� �� 
StartPoint$	 0,0&�.���  � Color$ 	#FFEEEEEE�� � Offset$ 0Q�� $ White��$ 1Q����� $	 0,1&�$	 0,0&�.��� $ 	#FFCCCCCC��$ 0Q�� $ White��$ 1Q��� -
   $
 button_Copy��$
��edit��$��2��$��Right=�$��0,141.433,5,0q�#���   $	��Top=�$��40��$��18��$ 	0.5,0.444&����	#FF878787$��0q����� $	 0,1&�$	 0,0&�.��� $ 	#FFEEEEEE��$ 0Q�� $ White��$ 1Q����� $	 0,1&�$	 0,0&�.��� $ 	#FFCCCCCC��$ 0Q�� $ White��$ 1Q��� -   $
 button1��$��确定��$��2��$
��Left=�$��40��$��	15,0,0,79q�#���   $��Bottom=�$	��101���� -   $
 button1_Copy��$��取消��$��2��$��40��$��140,0,50,79q�#���   $��Bottom=�