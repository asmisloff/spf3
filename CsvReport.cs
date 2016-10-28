using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;

namespace spf3
{
    class CsvReport : Report
    {
        public CsvReport(Specification spf) : base()
        {
            Spf = spf;
        }

        public override void Save()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.SupportMultiDottedExtensions = false;
            dlg.Filter = "Csv files(*.csv)|*.csv";
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK) {
                using (StreamWriter writer = new StreamWriter(dlg.OpenFile(), Encoding.GetEncoding(1251))) {
                    writer.WriteLine("Обозначение\tНаименование\tТолщина\tДлина\tШирина\tПримечание\tКол.");
                    string currentHeader = "";

                    foreach (var rec in Spf) {
                        if (currentHeader != rec["header"].StringValue.ToLower()) {
                            writer.WriteLine(string.Concat(Report.FirstCharToUpper(rec["header"]), "\t\t\t\t\t\t"));
                            currentHeader = rec["header"].StringValue.ToLower();
                        }
                        writer.WriteLine(string.Concat(
                            rec["art"], '\t', rec["name"], '\t', rec["th"], '\t', rec["len"], '\t', rec["wi"],
                            '\t', rec["note"], '\t', rec["qty"]));
                    }
                }
            }
        }
    }
}
