// Copyright 2011 Chris Patterson, Dru Sellers
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
namespace Automatonymous.Impl
{
    using System;


    public class DataEvent<TData> :
        SimpleEvent,
        Event<TData>,
        IEquatable<DataEvent<TData>>
        where TData : class
    {
        public DataEvent(string name)
            : base(name)
        {
        }

        public override void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }

        public bool Equals(DataEvent<TData> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other.Name, Name);
        }

        public override string ToString()
        {
            return string.Format("{0}<{1}> (Event)", Name, typeof(TData).Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return Equals(obj as DataEvent<TData>);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode()*27 + typeof(TData).GetHashCode();
        }
    }
}