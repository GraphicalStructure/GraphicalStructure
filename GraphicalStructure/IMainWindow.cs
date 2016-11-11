using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphicalStructure
{
    interface IMainWindow
    {
        void showEditLayer(int indexOfLayer, PathFigure pg, Path path);
    }
}
