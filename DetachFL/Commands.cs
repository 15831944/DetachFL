using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetachFL
{
    public class Commands
    {
        public const string Group = "Vova";

        [CommandMethod(Group, nameof(Vova_Test), CommandFlags.Modal)]
        public void Vova_Test()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            using (var t = doc.TransactionManager.StartTransaction())
            {
                DetachFL df = new DetachFL(doc);
                df.Test();
                t.Commit();
            }
        }
    }
}
