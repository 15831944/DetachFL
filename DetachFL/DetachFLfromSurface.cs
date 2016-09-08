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
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;

namespace DetachFL
{
    static class DetachFlFromSurface
    {   // Присваиваем название строке меню
        // Указываем объекты, при выборе которых появляется строка меню
        private const string MenuName = "Удалить ХЛ из поверхности";
        private static RXClass RxClassFeatureLine = RXObject.GetClass(typeof(FeatureLine));
        private static MenuItem Menu;

        public static void AttachContextMenu ()
        {
            // Стандартное описание процесса добавления строки меню, не понятно где ты объявил метод Detach? Можешь тут ответить, имею ввиду почему в сокращенном виде - просто Deatch?
            // Поглядел дальше - там и объявлен? Бля...у меня из-за этого вынос мозга))) Указали что делать по щелчку, не объяснив заранее как это делать...Типа сделай это, но я потом расскажу как) Или это компилятор потом в порядок приводит? Который JIT, сегодня новое слово узнал)))
            var cme = new ContextMenuExtension();
            Menu = new MenuItem(MenuName);
            Menu.Click += (o, e) => Detach();
            cme.MenuItems.Add(Menu);
            cme.MenuItems.Add(new MenuItem(""));
            // пока не имеет смысла, нужно научиться проверять принадлежность хар.линии к поверхности, без перебора всех поверхностей, только по самой линии
            //cme.Popup += Cme_Popup;
            Application.AddObjectContextMenuExtension(RxClassFeatureLine, cme);
        }       

        private static void Cme_Popup (object sender, EventArgs e)
        {
            var contextMenu = sender as ContextMenuExtension;
            if (contextMenu != null)
            {
                var doc = Application.DocumentManager.MdiActiveDocument;
                if (doc == null) return;
                var ed = doc.Editor;

                var menu = contextMenu.MenuItems[0];
                var selImpl = ed.SelectImplied();

                // TODO: проверить принадлежит ли хар.линия какой-либо поверхности. (пока непонятно как это сделать)
                

                var mVisible = true;
                var mEnabled = true;
                if (selImpl.Status == PromptStatus.OK)
                    menu.Enabled = mEnabled;
                menu.Visible = mVisible;
            }
        }        

        public static void Detach ()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var civil = CivilApplication.ActiveDocument;
            var surfIds = civil.GetSurfaceIds();
            
            var selRes = ed.SelectImplied();
            if (selRes.Status != PromptStatus.OK)
            {
                return;
            }
            List<ObjectId> idsFlToDetach = selRes.Value.GetObjectIds().ToList();
            List<ObjectId> idsFlDetached = new List<ObjectId>();
            List<ObjectId> idsEditedSurf = new List<ObjectId>();

            using (var t = doc.TransactionManager.StartTransaction())
            {
                bool isEditedSurf = false;
                foreach (ObjectId surfId in surfIds)
                {
                    var surf = surfId.GetObject(OpenMode.ForRead) as TinSurface;
                    IAeccTinSurface surfCom = (IAeccTinSurface)surf.AcadObject;

                    for (int i = 0; i < surfCom.Breaklines.Count; i++)
                    {
                        var brLine = surfCom.Breaklines.Item(i);
                        var brLineEnts = (object[])brLine.BreaklineEntities;
                        List<ObjectId> idBreaklinesToAdd = new List<ObjectId>();
                        bool isFind = false;
                        for (int b = 0; b < brLineEnts.Length; b++)
                        {
                            var brLineId = Autodesk.AutoCAD.DatabaseServices.DBObject.FromAcadObject(brLineEnts[b]);
                            idBreaklinesToAdd.Add(brLineId);
                            if (idsFlToDetach.Contains(brLineId))
                            {
                                //surf.BreaklinesDefinition.RemoveAt(i); // не всегда срабатывает!?
                                surfCom.Breaklines.Remove(i);
                                idBreaklinesToAdd.Remove(brLineId);
                                isFind = true;
                                idsFlDetached.Add(brLineId);
                            }
                        }
                        if (isFind)
                        {
                            isEditedSurf = true;
                            if (idBreaklinesToAdd.Any())
                            {
                                AddBreaklinesToSurface(surf, idBreaklinesToAdd);
                            }                            
                        }
                    }
                    if (isEditedSurf)
                    {
                        isEditedSurf = false;
                        idsEditedSurf.Add(surfId);
                    }
                }

                // Изменение стиля характерной линии
                StyleHelper.Change(idsFlDetached, "Удаленные из поверхности");               

                t.Commit();
            }

            // Перестройка поверхностей
            using (var t = doc.TransactionManager.StartTransaction())
            {
                foreach (var idSurf in idsEditedSurf)
                {
                    TinSurface surface = idSurf.GetObject(OpenMode.ForWrite) as TinSurface;
                    surface.Rebuild();
                }
                t.Commit();
            }
        }
        /// <summary>
        //// Возвращаем невыбранные пользователем ХЛ обратно в исходную поверхность. Возвращаются ХЛ, которые были в одной операции добавления в поверхность.
        /// </summary>
        /// <param name="surf"></param>
        /// <param name="idEntsToAdd"></param>
        private static void AddBreaklinesToSurface (TinSurface surf, List<ObjectId> idEntsToAdd)
        {
            ObjectIdCollection ids = new ObjectIdCollection(idEntsToAdd.ToArray());
            surf.BreaklinesDefinition.AddStandardBreaklines(ids, 0.1, 0, 0, 0);
           }

      
        }
    }
        

 