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
    public class Commands
    {
        public const string Group = "Vova";

        public static RXClass RxClassFeatureLine { get; private set; }

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
        private static FeatureLine GetSelectedFeatureLine(Editor ed)
        {
            FeatureLine FL = null;
            PromptSelectionResult selImplRes = ed.SelectImplied();
            if (selImplRes.Status == PromptStatus.OK)
            {
                foreach (SelectedObject selEnt in selImplRes.Value)
                {
                    if (selEnt.ObjectId.ObjectClass == RxClassFeatureLine)
                    {
                        FL = selEnt.ObjectId.GetObject(OpenMode.ForRead) as FeatureLine;
                        break;
                    }
                }
            }
            return FL;
        }

      }
   

}
