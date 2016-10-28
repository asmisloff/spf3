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
            Init();
        }

        public static void Init()
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

        /// <summary>
        /// Extract attributes from BlockReference.
        /// </summary>
        /// <param name="bref">BlockReference for data extraction</param>
        /// <param name="innerBlocks">
        /// List of BlockReferences incapsulated in bref.
        /// Can be used by upper-level code for recursively walking throw bref.</param>
        /// <returns></returns>
        public static Record GetContent(BlockReference bref, List<BlockReference> innerBlocks = null)
        {
            Record res = new Record();
            using (Transaction tr = activeDB.TransactionManager.StartTransaction()) {
                ObjectId btrId = bref.BlockTableRecord;
                BlockTableRecord rec = tr.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;
                AttributeCollection attCol;

                foreach (ObjectId id in rec) {
                    DBObject ob = tr.GetObject(id, OpenMode.ForRead);
                    Type obTp = ob.GetType();
                    if (obTp == typeof(AttributeDefinition)) {
                        AttributeDefinition adef = (AttributeDefinition)ob;
                        if (/*adef.Tag.ToLower() != "qty" && */adef.TextString != "") {
                            res[adef.Tag] = adef.TextString;
                        }
                        else {
                            res[adef.Tag] = Record.Default(adef.Tag);
                        }
                    }
                    if (obTp == typeof(BlockReference) && innerBlocks != null) {
                        innerBlocks.Add((BlockReference)ob);
                    }
                }

                attCol = bref.AttributeCollection;
                foreach (ObjectId id in attCol) {
                    AttributeReference aref = tr.GetObject(id, OpenMode.ForRead) as AttributeReference;
                    if (aref.TextString != "") {
                        res[aref.Tag] = aref.TextString;
                    }
                    else {
                        res[aref.Tag] = Record.Default(aref.Tag);
                    }
                }
            }
            res["block_name"] = bref.Name;
            return res.Update();
        }

        public static Table MakeTable()
        {
            return new Table();
        }
    }
}
