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


    public class TriggerEvent :
        Event
    {
        readonly string _name;

        public TriggerEvent(string name)
        {
            _name = name;
        }

        public string Name => _name;

        public virtual void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x =>
            {
            });
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("event");
            scope.Add("name", _name);
        }

        public int CompareTo(Event other)
        {
            return String.Compare(_name, other.Name, StringComparison.Ordinal);
        }

        public bool Equals(TriggerEvent other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(TriggerEvent))
                return false;
            return Equals((TriggerEvent)obj);
        }

        public override int GetHashCode()
        {
            return _name?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return $"{_name} (Event)";
        }
    }
}