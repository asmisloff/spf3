using System;
using System.Collections.Generic;
using static System.Math;

namespace spf3
{
    class Record : Dictionary<string, Cell>, IComparable<Record>
    {
        public Record() : base(System.StringComparer.InvariantCultureIgnoreCase)
        {
            this["block_name"] = "";
            this["header"] = DEFAULT_HEADER;
            this["art"] = "";
            this["name"] = "";
            this["th"] = 0;
            this["len"] = 0;
            this["wi"] = 0;
            this["note"] = "";
            this["qty"] = 1;
        }

        public Record Update()
        {
            TrimXrefName();
            Action fill_dim = () => {
                string s = "";
                string th = this["th"];
                string len = this["len"];
                string wi = this["wi"];
                if (th != "0") {
                    s += th + "x";
                }
                if (len != "0") {
                    s += len + "x";
                }
                if (wi != "0") {
                    s += wi;
                }
            };

            Action parse_dim = () => {
                var dims = this["dim"].StringValue.Replace('х', 'x').Split('x');
                switch (dims.Length) {
                    case 0:
                        break;
                    case 1:
                        this["len"] = dims[0];
                        break;
                    case 2:
                        this["len"] = dims[0];
                        this["wi"] = dims[1];
                        break;
                    default:
                        this["th"] = dims[0];
                        this["len"] = dims[1];
                        this["wi"] = dims[2];
                        break;
                }

            };

            if (!this.ContainsKey("dim")) {
                this.Add("dim", "");
            }
            if (this["dim"] == "") {
                fill_dim();
            }
            else {
                parse_dim();
            }

            ParseBlockName();
            this["qty"].ParseAndSetValue(this["qty"]);

            return this;
        }

        string TrimXrefName()
        {
            string block_name = this["block_name"].StringValue;
            if (block_name.Contains("|")) {
                this["block_name"] = block_name.Split('|')[1];
            }
            return this["block_name"].StringValue;
        }

        string Trim__(string s)
        {
            for (int i = 0; i < s.Length; i++) {
                if (s[i] != '_') {
                    return s.Substring(i);
                }
            }
            return "";
        }

        void ParseBlockName()
        {
            string block_name = Trim__(this["block_name"].StringValue);
            var tokens = block_name.Split('_');
            var tags = new string[] { "art", "name", "dim", "note", "qty" };
            int cnt = Min(tokens.Length, tags.Length);
            for (int i = 0; i < cnt; i++) {
                var tag = tags[i];
                var token = tokens[i];
                if (this[tag] == "" || tag == "qty") {
                    this[tag] = token;
                }
            }
        }

        public override string ToString()
        {
            var s = new System.Text.StringBuilder();
            foreach (var key in this.Keys) {
                s.Append(key + " : " + this[key].ToString() + " \n ");
            }
            return s.ToString();
        }

        public string ToRow(char delimiter = '\t')
        {
            var s = new System.Text.StringBuilder();
            foreach (var item in new string[] { "header", "art", "name", "dim", "note", "qty" }) {
                s.Append(this[item].ToString() + delimiter);
            }
            return s.ToString().TrimEnd(delimiter);
        }

        public override bool Equals(object obj)
        {
            var r = obj as Record;
            if (r == null) {
                return false;
            }
            else {
                return (
                    r["header"] == this["header"] &&
                    r["art"] == this["art"] &&
                    r["name"] == this["name"] &&
                    r["dim"] == this["dim"] &&
                    r["note"] == this["note"]);
            }
        }

        public static Cell Default(string tag)
        {
            switch (tag.ToLower()) {
                case "header":
                    return "элементы конструкции";
                case "qty":
                    return 1;
                default:
                    return "";
            }
        }

        public static bool operator ==(Record r1, Record r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(Record r1, Record r2)
        {
            return !r1.Equals(r2);
        }

        public override int GetHashCode()
        {
            return (this["header"].StringValue + this["art"].StringValue + this["name"].StringValue +
                this["dim"].StringValue + this["note"].StringValue).GetHashCode();
        }

        public int CompareTo(Record r)
        {
            var tags = new string[] { "header", "art", "name", "dim", "note" };
            string thisKey = RangeOfHeader(this["header"]).ToString();
            string rKey = RangeOfHeader(r["header"]).ToString();
            foreach (var tag in tags) {
                thisKey += this[tag].StringValue.ToLower();
                rKey += r[tag].StringValue.ToLower();
            }
            return thisKey.CompareTo(rKey);
        }

        int RangeOfHeader(string h)
        {
            switch (h) {
                case "элементы конструкции":
                    return 0;
                case "фурнитура":
                    return 1;
                case "метизы":
                    return 2;
                default:
                    return 100;
            }
        }

        const string DEFAULT_HEADER = "элементы конструкции";
    }
}
