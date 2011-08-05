using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.ViewModel.FindAndReplace
{
    public struct SearchScope : IEquatable<SearchScope>
    {
        int _value;
        string _name;

        public SearchScope(int value, string displayName) {
            _value = value;
            _name = displayName;
        }

        public static SearchScope CurrentDocument = new SearchScope(0, "Current Document");
        public static SearchScope AllDocuments = new SearchScope(1, "All Documents");

        internal int Value { get { return _value; } }

        internal string Name {
            get { return _name; }
            set { _name = value; }
        }


        public static bool operator ==(SearchScope right, SearchScope left) {
            return right.Equals(left);
        }
        public static bool operator !=(SearchScope right, SearchScope left) {
            return !right.Equals(left);
        }

        #region IEquatable

        public bool Equals(SearchScope other) {
            return other.Value.Equals(this.Value);
        }

        public override bool Equals(object obj) {
            if(obj is SearchScope)
                return this.Equals((SearchScope)obj);
            else
                return false;
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        #endregion

        public override string ToString() {
            return Name;
        }

    }
}
