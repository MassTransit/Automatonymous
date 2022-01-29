namespace Automatonymous.Graphing
{
    using System;


    [Serializable]
    public class Vertex : IEquatable<Vertex>
    {
        public Vertex(Type type, Type targetType, string title, bool isComposite)
        {
            VertexType = type;
            TargetType = targetType;
            Title = title;
            IsComposite = isComposite;
        }

        public string Title { get; }
        public bool IsComposite { get; }

        public Type VertexType { get; }

        public Type TargetType { get; }

        public bool Equals(Vertex other) =>
            !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || string.Equals(Title, other.Title) && VertexType == other.VertexType && TargetType == other.TargetType);

        public override bool Equals(object obj) =>
            !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Vertex)obj));

        public override string ToString() =>
            Title;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Title?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (VertexType?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (TargetType?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
