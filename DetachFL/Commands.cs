using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetachFL
{
    public class Commands : IExtensionApplication
    {           
        public void Initialize ()
        {
            DetachFlFromSurface.AttachContextMenu();
        }

        public void Terminate ()
        {            
        }
    }
}
