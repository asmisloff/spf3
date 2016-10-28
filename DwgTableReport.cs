using System;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Colors;
using static spf3.Autocad;

namespace spf3
{
    class DwgTableReport : Report
    {
        public DwgTableReport(Specification t)
        {
            this.Spf = t;
        }

        public override void Save()
        {
            if (Spf.Count == 0) {
                return;
            }

            var t = MakeTable();
            t.SetSize(1, 6);
            t.Columns[0].Width = 30;
            t.Columns[1].Width = 100;
            t.Columns[2].Width = 100;
            t.Columns[3].Width = 60;
            t.Columns[4].Width = 100;
            t.Columns[5].Width = 30;

            var headers = new string[] { "Поз.", "Обозначение", "Наименование", "Размеры", "Примечание", "Кол." };
            for (int i = 0; i < headers.Length; i++) {
                t.Cells[0, i].TextString = headers[i];
            }

            int cnt = 0;
            int pos = 1;
            string current_header = "";
            foreach (var rec in Spf) {
                t.InsertRows(++cnt, 6, 1);
                if (current_header != rec["header"].StringValue.ToLower()) {
                    current_header = rec["header"].StringValue.ToLower();
                    t.Cells[cnt, 0].TextString = FirstCharToUpper(current_header);
                    var range = CellRange.Create(t, cnt, 0, cnt, 5);
                    range.Alignment = CellAlignment.MiddleLeft;
                    range.BackgroundColor = Color.FromRgb(R, G, B);
                    t.MergeCells(range);
                    t.InsertRows(++cnt, 6, 1);
                }
                t.Cells[cnt, 0].TextString = (pos++).ToString();
                t.Cells[cnt, 1].TextString = rec["art"];
                t.Cells[cnt, 2].TextString = rec["name"];
                t.Cells[cnt, 3].TextString = rec["dim"];
                t.Cells[cnt, 4].TextString = rec["note"];
                t.Cells[cnt, 5].TextString = rec["qty"];
            }

            var h = AppendToPaperSpace(t);
            IAcadApplication axAcad = System.Runtime.InteropServices.Marshal.GetActiveObject("Autocad.Application") as IAcadApplication;
            IAcadDocument axDoc = axAcad.ActiveDocument;
            IAcadTable axTable = axDoc.HandleToObject(h.ToString()) as IAcadTable;
            for (int i = 0; i < axTable.Columns; i++) {
                TuneColumnWidth(axTable, i);
                axTable.RowHeight = 1;
            }
        }

        void TuneColumnWidth(IAcadTable table, int index)
        {
            double h = table.Height;
            double w = table.GetColumnWidth(index);
            while (table.Height <= h)
                table.SetColumnWidth(index, --w);
            table.SetColumnWidth(index, ++w);
        }
    }
}
