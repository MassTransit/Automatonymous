namespace Automatonymous.Graphing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    [Serializable]
    public class StateMachineGraph
    {
        public IEnumerable<Vertex> Vertices { get; }

        public IEnumerable<Edge> Edges { get; }

        public StateMachineGraph(IEnumerable<Vertex> vertices, IEnumerable<Edge> edges)
        {
            Vertices = vertices.ToArray();
            Edges = edges.ToArray();
        }
    }
}
