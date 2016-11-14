using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;

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

        public static string GetBoundingBox(Entity ent)
        {
            using (Transaction tr = activeDB.TransactionManager.StartTransaction()) {
                var btr = tr.GetObject(ent.Id, OpenMode.ForWrite);
                var bref = (ent as BlockReference);
                if (bref != null) {
                    var normal = bref.Normal;
                    Ed.WriteMessage(normal.ToString());
                    var M = Matrix3d.PlaneToWorld(normal);
                    var copy = bref.GetTransformedCopy(M.Inverse()) as BlockReference;
                    var n2 = copy.Normal;
                    Ed.WriteMessage("\n" + n2.ToString());
                    var extents = copy.GeometryExtentsBestFit();
                    var max = extents.MaxPoint.ToArray();
                    var min = extents.MinPoint.ToArray();
                    var dims = new double[3];
                    for (int i = 0; i < 3; i++) {
                        dims[i] = max[i] - min[i];
                    }
                    var sortedDims = dims.OrderBy(x => x).Reverse().ToArray();
                    Func<double, double> round = x => Math.Round(x, MidpointRounding.AwayFromZero);
                    if (dims[2] == sortedDims[2]) {
                        return String.Format("{0}x{1}x{2}", round(dims[2]), round(dims[0]), round(dims[1]));
                    }
                    else {
                        return String.Format("{0}x{1}x{2}", round(sortedDims[2]), round(sortedDims[0]), round(sortedDims[1]));
                    }
                }
                tr.Commit();
            }
            return "";
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
            if (res["header"] == "элементы конструкции") {
                res["dim"] = GetBoundingBox(bref);
            }
            return res.Update();
        }

        public static Table MakeTable()
        {
            return new Table();
        }
    }
}
