// Copyright 2011-2014 Chris Patterson, Dru Sellers
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Automatonymous
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;


    [Serializable]
    [DebuggerDisplay("{Status}")]
    public struct CompositeEventStatus :
        IComparable<CompositeEventStatus>
    {
        int _bits;

        public CompositeEventStatus(int bits)
        {
            _bits = bits;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Status
        {
            get
            {
                int bits = _bits;
                return string.Join("", Enumerable.Range(0, 32).Select(x => (bits & (1 << x)) == 0 ? "0" : "1"));
            }
        }

        public int Bits => _bits;

        public int CompareTo(CompositeEventStatus other)
        {
            return other._bits - _bits;
        }

        public bool Equals(CompositeEventStatus other)
        {
            return other._bits == _bits;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (obj.GetType() != typeof(CompositeEventStatus))
                return false;
            return Equals((CompositeEventStatus)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public void Set(int flag)
        {
            _bits |= flag;
        }
    }
}