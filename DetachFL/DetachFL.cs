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
using Autodesk.AECC.Interop.Land;

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
            if (selRes.Status != PromptStatus.OK)
            {
                return;
            }
            var fl = selRes.ObjectId.GetObject(OpenMode.ForRead) as FeatureLine;

            foreach (ObjectId surfId in surfIds)
            {
                var surf = surfId.GetObject(OpenMode.ForRead) as TinSurface;
                IAeccTinSurface surfCom = (IAeccTinSurface)surf.AcadObject;                

                for (int i = 0; i < surfCom.Breaklines.Count; i++)
                {
                    var brLine = surfCom.Breaklines.Item(i);
                    var brLineEnts = (object[])brLine.BreaklineEntities;
                    List<ObjectId> idEntsToAdd = new List<ObjectId>();
                    bool isFind = false;
                    for (int b = 0; b < brLineEnts.Length; b++)
                    {
                        var brLineId = Autodesk.AutoCAD.DatabaseServices.DBObject.FromAcadObject(brLineEnts[b]);
                        idEntsToAdd.Add(brLineId);
                        if (fl.Id == brLineId)
                        {
                            //surf.BreaklinesDefinition.RemoveAt(i); // не всегда срабатывает!?
                            surfCom.Breaklines.Remove(i);
                            idEntsToAdd.Remove(brLineId);
                            isFind = true;
                        }
                    }
                    if (isFind)
                    {
                        if (idEntsToAdd.Any())
                        {
                            AddOperToSurf(surf, idEntsToAdd);
                        }
                        return;
                    }
                }
            }
        }

        private void AddOperToSurf (TinSurface surf, List<ObjectId> idEntsToAdd)
        {
            ObjectIdCollection ids = new ObjectIdCollection(idEntsToAdd.ToArray());
            surf.BreaklinesDefinition.AddStandardBreaklines(ids, 0.1, 0, 0, 0);
        }
    }
}
