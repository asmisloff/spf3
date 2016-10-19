using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace spf3
{
    public static class Commands
    {
        [CommandMethod("test")]
        public static void SpAll(Entity ent)
        {
            var sset = Autocad.SSGet();
            Autocad.SSForeach<BlockReference>(
                sset,
                bref => {
                    var atts = Autocad.GetAttributes(bref);
                    foreach (var att in atts) {
                        Autocad.Ed.WriteMessage(String.Format("{0} -> {1}\n", att.Key, att.Value));
                    }
                });
        }
    }
}
