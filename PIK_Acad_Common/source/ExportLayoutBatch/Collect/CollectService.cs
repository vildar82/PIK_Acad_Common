﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;

namespace PIK_Acad_Common.ExportLayoutsBatch
{
    public class CollectService
    {
        public void Collect (List<FileSheets> filesSheets)
        {
            Point3d ptInsert = Point3d.Origin;

            var docNew = Application.DocumentManager.Add(null);
            Application.DocumentManager.MdiActiveDocument = docNew;
            using (docNew.LockDocument())
            {
                foreach (var item in filesSheets)
                {
                    try
                    {
                        item.Insert(ref ptInsert, docNew.Database);
                        item.Bind(docNew.Database);
                    }
                    catch {
                        Inspector.AddError($"Ошибка вставки и внедрения ссылки - {item.Name}");
                    }
                }
            }        
        }
    }
}
