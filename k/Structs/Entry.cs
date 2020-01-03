using System;
using System.Collections.Generic;
using System.Text;

namespace k.Structs
{
    public struct Entry
    {
        public int Index;        // Index of next entry, -1 if last
        public string Key;           // Key of entry
        public Dynamic Value;         // Value of entry
    }
}
