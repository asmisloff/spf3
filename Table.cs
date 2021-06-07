using Wintellect.PowerCollections;

namespace spf3
{
    class Specification : OrderedBag<Record>
    {
        public new void Add(Record r)
        {
            int index = LastIndexOf(r);
            if (r["art"].StringValue.StartsWith("*U")) return;
            if (index >= 0) {
                try {
                    var cell = this[index];
                    cell["qty"] += r["qty"];
                }
                catch (System.Exception e) {
                    System.Windows.Forms.MessageBox.Show(e.Message);
                }
            }
            else {
                base.Add(r);
            }
        }
    }
}
