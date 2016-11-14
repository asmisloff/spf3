using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using static spf3.Autocad;

namespace spf3
{
    public static class Commands
    {
        [CommandMethod("spall")]
        public static void SpAll(Entity ent)
        {
            Autocad.Init();

            var sset = SSGet();
            var spec = new Specification();

            Action<BlockReference> ProcessBref = null;
            ProcessBref = (bref) => {
                List<BlockReference> innerBlocks = new List<BlockReference>();
                Record r = GetContent(bref, innerBlocks);
                if (!r["block_name"].StringValue.StartsWith("_")) {
                    spec.Add(r);
                }
                foreach (BlockReference b in innerBlocks) {
                    ProcessBref(b);
                }
            };

            SSForeach<BlockReference>(sset, ProcessBref);

            //foreach (var rec in spec) {
            //    Ed.WriteMessage(rec.ToString());
            //}

            var report = new DwgTableReport(spec);
            report.Save();
            Ed.WriteMessage("OK");
        }

        [CommandMethod("spallf")]
        public static void SpAllF(Entity ent)
        {
            Autocad.Init();

            var sset = SSGet();
            var spec = new Specification();

            Action<BlockReference> ProcessBref = null;
            ProcessBref = (bref) => {
                List<BlockReference> innerBlocks = new List<BlockReference>();
                Record r = GetContent(bref, innerBlocks);
                if (!r["block_name"].StringValue.StartsWith("_")) {
                    spec.Add(r);
                }
                foreach (BlockReference b in innerBlocks) {
                    ProcessBref(b);
                }
            };

            SSForeach<BlockReference>(sset, ProcessBref);
            
            var report = new CsvReport(spec);
            report.Save();
            Ed.WriteMessage("OK");
        }

        [CommandMethod("sptop")]
        public static void SpTop(Entity ent)
        {
            Autocad.Init();

            var sset = SSGet();
            var table = new Specification();

            Action<BlockReference> ProcessBref = null;
            ProcessBref = (bref) => {
                List<BlockReference> innerBlocks = new List<BlockReference>();
                Record r = GetContent(bref, innerBlocks);
                if (r["block_name"].StringValue.StartsWith("__")) {
                    table.Add(r);
                    return;
                }
                if (!r["block_name"].StringValue.StartsWith("_")) {
                    table.Add(r);
                }
                foreach (BlockReference b in innerBlocks) {
                    ProcessBref(b);
                }
            };

            SSForeach<BlockReference>(sset, ProcessBref);

            var report = new DwgTableReport(table);
            report.Save();
            Ed.WriteMessage("OK");
        }

    [CommandMethod("bbox")]
    public static void BBox()
    {
        SSForeach<Entity>(SSGet(),
            ent => Ed.WriteMessage(GetBoundingBox(ent)));
    }
    }
}
