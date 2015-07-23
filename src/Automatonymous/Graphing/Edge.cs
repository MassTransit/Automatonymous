// Copyright 2011-2015 Chris Patterson, Dru Sellers
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
namespace Automatonymous.Graphing
{
    using System;


    [Serializable]
    public class Edge : 
        IEquatable<Edge>
    {
        public Edge(Vertex from, Vertex to, string title)
        {
            From = from;
            To = to;
            Title = title;
        }

        public Vertex To { get; }

        public Vertex From { get; }

        string Title { get; }

        public bool Equals(Edge other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(To, other.To) && Equals(From, other.From) && string.Equals(Title, other.Title);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Edge)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = To?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (From?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Title?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}