// Copyright 2011-2016 Chris Patterson, Dru Sellers
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
namespace Automatonymous.Events
{
    using System;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;


    public class DataEvent<TData> :
        TriggerEvent,
        Event<TData>,
        IEquatable<DataEvent<TData>>
    {
        public DataEvent(string name)
            : base(name)
        {
        }

        public override void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x =>
            {
            });
        }

        public override void Probe(ProbeContext context)
        {
            base.Probe(context);

            context.Add("dataType", TypeNameCache<TData>.ShortName);
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
            return $"{Name}<{typeof(TData).Name}> (Event)";
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
            return base.GetHashCode() * 27 + typeof(TData).GetHashCode();
        }
    }
}