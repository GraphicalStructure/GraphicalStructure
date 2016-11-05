using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphicalStructure
{
    class ColorProc
    {
        // 保存 层与cover的映射关系
        public static KeyValuePair<PathFigure, Shape> PfToCoverMap;
        public static KeyValuePair<Shape, PathFigure> CoverToPfMap;

        // 添加层时，在canvas上对应位置（绝对坐标）添加cover，并作相应处理
        public static void processWhenAddLayer() {

        }

        // 删除层时，删除cover， 并作相应处理
        public static void processWhenDelLayer()
        {

        }

        // 移动层时，同时移动cover， 并作相应处理
        public static void processWhenMoveLayer()
        {

        }

        // 真正的层点击更改颜色后，改变cover的颜色，并作相应处理
        public static void processWhenChangeLayerColor()
        {

        }

        // cover 的双击事件回调,传递到真正的层上，手动触发真正的层的双击事件
        public static void doubleClickOnCover()
        {

        }
    }
}
