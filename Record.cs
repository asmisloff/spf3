namespace spf3
{
    class Record
    {
        public Record()
        {
            Header = DEFAULT_HEADER;
            Art = "";
            Name = "";
            Th = 0;
            Len = 0;
            Wi = 0;
            Note = "";
            Qty = "1";
        }

        public Record(string header, string art, string name, int th, int len, int wi, string note, string qty)
        {
            Header = header;
            Art = art;
            Name = name;
            Th = th;
            Len = len;
            Wi = wi;
            Note = note;
            Qty = qty;
        }

        public static Record operator +(Record r1, Record r2)
        {
            string[] q1 = r1.Qty.Split(' ');
            string[] q2 = r2.Qty.Split(' ');
            int iq1 = int.Parse(q1[0]);
            int iq2 = int.Parse(q2[0]);
            string uq1 = q1.Length > 0 ? q1[1] : "";
            string uq2 = q2.Length > 0 ? q2[1] : "";
            int iq = iq1 + iq2;
            string uq = uq1 != "" ? uq1 : uq2;
            return new Record(
                r1.Header, r1.Art, r1.Name, r1.Th, r1.Len, r1.Wi, r1.Note, string.Concat(iq.ToString(), " ", uq));
        }

        public static bool operator ==(Record r1, Record r2)
        {
            return
                r1.Art.ToLower() == r2.Art.ToLower() &&
                r1.Name.ToLower() == r2.Name.ToLower() &&
                r1.Th == r2.Th &&
                r1.Len == r2.Len &&
                r1.Wi == r2.Wi &&
                r1.Note.ToLower() == r2.Note.ToLower();
        }

        public static bool operator !=(Record r1, Record r2)
        {
            return !(r1 == r2);
        }

        public string Header { get; set; }
        public string Art { get; set; }
        public string Name { get; set; }
        public int Th { get; set; }
        public int Len { get; set; }
        public int Wi { get; set; }
        public string Note { get; set; }
        public string Qty { get; set; }

        const string DEFAULT_HEADER = "Элементы коснтрукции";
    }
}
