namespace Automatonymous.Graphing
{
    using System;


    [Serializable]
    public class Edge : IEquatable<Edge>
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

        public bool Equals(Edge other) =>
            !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || Equals(To, other.To) && Equals(From, other.From) && string.Equals(Title, other.Title));

        public override bool Equals(object obj) =>
            !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Edge)obj));

        public override string ToString() => Title;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = To?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (From?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Title?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
