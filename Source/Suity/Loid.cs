using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    public struct Loid : IFormattable, IComparable, IComparable<Loid>, IEquatable<Loid>
    {
        public static readonly Loid Empty = new Loid(string.Empty);
        public static Loid Create(int len)
        {
            string id = Generate(len);
            return new Loid(id);
        }
        public static Loid Create(string prefix, int len)
        {
            string id = Generate(len);
            return new Loid($"{prefix}-{id}");
        }

        static Random _rnd = new Random();
        static char[] Chars = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
        };
        static string Generate(int len)
        {
            char[] chars = new char[len];

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = Chars[_rnd.Next(0, Chars.Length - 1)];
            }

            return new string(chars);
        }


        readonly string _id;

        public string Id => _id;

        public Loid(string id)
        {
            _id = id ?? string.Empty;
        }




        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return -1;
            }
            else
            {
                return ToString().CompareTo(obj.ToString());
            }
        }
        public int CompareTo(Loid other)
        {
            return _id.CompareTo(other._id);
        }
        public bool Equals(Loid other)
        {
            return _id == other._id;
        }
        public override bool Equals(object obj)
        {
            if (obj is Loid loid)
            {
                return _id == loid._id;
            }
            else
            {
                return false;
            }
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString();
        }
        public override string ToString()
        {
            return $"({_id})";
        }
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(Loid v1, Loid v2)
        {
            return v1._id == v2._id;
        }
        public static bool operator !=(Loid v1, Loid v2)
        {
            return v1._id != v2._id;
        }
    }

}
