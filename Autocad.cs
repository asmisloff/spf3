using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

namespace spf3
{
    class Autocad
    {
        static Document activeDoc;
        static Database activeDB;
        public static Editor Ed { get; set; }

        static Autocad()
        {
            activeDoc = Application.DocumentManager.MdiActiveDocument;
            activeDB = activeDoc.Database;
            Ed = activeDoc.Editor;
        }

        public static SelectionSet SSGet()
        {
            PromptSelectionResult prompt = Ed.GetSelection();
            if (prompt.Status == PromptStatus.OK)
                return prompt.Value;
            else
                return null;
        }

        public static void SSForeach<T>(SelectionSet sset, Action<T> proc, OpenMode openMode = OpenMode.ForRead)
            where T : class
        {
            if (sset == null)
                return;
            using (Transaction tr = activeDB.TransactionManager.StartTransaction()) {
                foreach (SelectedObject so in sset) {
                    if (so != null) {
                        T ent = tr.GetObject(so.ObjectId, openMode) as T;
                        if (ent != null)
                            proc(ent);
                    }
                }
                tr.Commit();
                tr.Dispose();
            }
        }

        public static Handle AppendToPaperSpace(Entity ent)
        {
            using (Transaction tr = activeDB.TransactionManager.StartTransaction()) {
                BlockTable bt = tr.GetObject(activeDB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord paperSpace = tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;
                paperSpace.AppendEntity(ent);
                tr.AddNewlyCreatedDBObject(ent, true);
                tr.Commit();
                tr.Dispose();
            }
            return ent.Handle;
        }

        public static Record GetContent(BlockReference bref, List<BlockReference> innerBlocks)
        {
            throw new NotImplementedException();
        }
    }
}
