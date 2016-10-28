using System;
using static System.Math;

namespace spf3
{
    class Cell
    {
        public Cell()
        {
            StringValue = "";
            NumberValue = 0;
        }

        public static implicit operator Cell(int value)
        {
            Cell c = new Cell();
            c.NumberValue = value;
            return c;
        }

        public static implicit operator Cell(string value)
        {
            Cell c = new Cell();
            c.StringValue = value;
            return c;
        }

        public static implicit operator int(Cell c)
        {
            return (int)c.NumberValue;
        }

        public static implicit operator double(Cell c)
        {
            return c.NumberValue;
        }

        public static implicit operator string(Cell c)
        {
            return c.ToString();
        }

        public static Cell operator +(Cell _this, Cell other)
        {
            Cell c = new Cell();
            c.NumberValue = _this.NumberValue + other.NumberValue;
            c.StringValue = _this.StringValue != "" ? _this.StringValue : other.StringValue;
            return c;
        }

        public string StringValue
        {
            get; set;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Cell c, string s)
        {
            string s1 = "";
            if (c.NumberValue != 0) {
                s1 += c.NumberValue;
            }
            s1 += c.StringValue;
            return s == s1;
        }

        public static bool operator ==(Cell c1, Cell c2)
        {
            return
                c1.NumberValue == c2.NumberValue &&
                c1.StringValue.ToLower() == c2.StringValue.ToLower();
        }

        public static bool operator !=(Cell c, string s)
        {
            return !(c == s);
        }

        public static bool operator !=(Cell c1, Cell c2)
        {
            return !(c1 == c2);
        }

        public double NumberValue
        {
            get; set;
        }

        public Cell ParseAndSetValue(string s)
        {
            int i = 0;
            string number = "";
            string unit = "";
            int len = s.Length;

            if (Char.IsDigit(s[0])) {
                for (i = 0; i < len; i++) {
                    char c = s[i];
                    if (Char.IsDigit(c)) {
                        number += c;
                    }
                    else if (c == '.' || c == ',') {
                        number += '.';
                    }
                    else if (c == ' ') {
                        i++;
                        break;
                    }
                    else {
                        i--;
                        break;
                    }
                }
                NumberValue = double.Parse(number);
            }
            for (int j = i; j < len; j++) {
                unit += s[j];
            }
            StringValue = unit;
            return this;
        }

        public override string ToString()
        {
            string s = "";
            if (NumberValue != 0) {
                s += NumberValue.ToString() + " ";
            }
            s += StringValue;
            return s;
        }
    }
}
