using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using System.Globalization;
using System.Diagnostics;

namespace SmartDevelop.Model.Tokenizer
{

    /// <summary>
    /// Represents a simple segment (Offset,Length pair) that is not automatically updated
    /// on document changes.
    /// </summary>
    struct CodeSegment : IEquatable<CodeSegment>, ISegment
    {
        public static readonly CodeSegment Invalid = new CodeSegment(-1, -1);

        public readonly int Offset, Length;

        int ISegment.Offset {
            get { return Offset; }
        }

        int ISegment.Length {
            get { return Length; }
        }

        public int EndOffset {
            get {
                return Offset + Length;
            }
        }

        public CodeSegment(int offset, int length) {
            this.Offset = offset;
            this.Length = length;
        }

        public CodeSegment(ISegment segment) {
            Debug.Assert(segment != null);
            this.Offset = segment.Offset;
            this.Length = segment.Length;
        }

        public override int GetHashCode() {
            unchecked {
                return Offset + 10301 * Length;
            }
        }

        public override bool Equals(object obj) {
            return (obj is CodeSegment) && Equals((CodeSegment)obj);
        }

        public bool Equals(CodeSegment other) {
            return this.Offset == other.Offset && this.Length == other.Length;
        }

        public static bool operator ==(CodeSegment left, CodeSegment right) {
            return left.Equals(right);
        }

        public static bool operator !=(CodeSegment left, CodeSegment right) {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public override string ToString() {
            return "[Offset=" + Offset.ToString(CultureInfo.InvariantCulture) + ", Length=" + Length.ToString(CultureInfo.InvariantCulture) + "]";
        }
    }
}
