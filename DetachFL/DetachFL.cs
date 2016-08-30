using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetachFL
{
    public class DetachFL
    {
        Document doc;
        Database db;
        Editor ed;

        public DetachFL(Document doc)
        {
            this.doc = doc;
            db = doc.Database;
            ed = doc.Editor;
        }

        public void Test()
        {
            var civil = CivilApplication.ActiveDocument;
            var surfIds = civil.GetSurfaceIds();  

            var selOPt = new PromptEntityOptions("\nВыбери хар линию");
            selOPt.SetRejectMessage("\nТолько хар линию");
            selOPt.AddAllowedClass(typeof(FeatureLine), true);
            var selRes = ed.GetEntity(selOPt);
            if (selRes.Status != PromptStatus.OK                 )
            {
                return;
            }
            var fl = selRes.ObjectId.GetObject(OpenMode.ForRead) as FeatureLine;           

            var settingsFl = civil.Settings.GetSettings<SettingsCmdAddSurfaceBreaklines>();
            
            foreach (ObjectId surfId in surfIds)
            {
                var surf = surfId.GetObject(OpenMode.ForRead) as TinSurface;
                var brLines = surf.BreaklinesDefinition;
                var count = brLines.Count;
                var br = brLines[0];
            }
        }
    }
}
