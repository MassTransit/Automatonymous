namespace Automatonymous.Visualizer
{
    using System;
    using System.Linq;
    using Graphing;
    using QuikGraph;
    using QuikGraph.Graphviz;
    using QuikGraph.Graphviz.Dot;


    public class StateMachineGraphvizGenerator
    {
        readonly AdjacencyGraph<Vertex, Edge<Vertex>> _graph;

        public StateMachineGraphvizGenerator(StateMachineGraph data)
        {
            _graph = new AdjacencyGraph<Vertex, Edge<Vertex>>();
            _graph.AddVertexRange(data.Vertices);
            _graph.AddEdgeRange(data.Edges.Select(x => new Edge<Vertex>(x.From, x.To)));
        }

        public string CreateDotFile()
        {
            var algorithm = new GraphvizAlgorithm<Vertex, Edge<Vertex>>(_graph);
            algorithm.FormatVertex += (sender, args) =>
            {
                args.VertexFormat.Label = args.Vertex.Title;

                if (args.Vertex.VertexType == typeof(Event))
                {
                    args.VertexFormat.FontColor = GraphvizColor.Black;
                    args.VertexFormat.Shape = GraphvizVertexShape.Rectangle;

                    if (args.Vertex.TargetType != typeof(Event) && args.Vertex.TargetType != typeof(Exception))
                        args.VertexFormat.Label += "<" + args.Vertex.TargetType.Name + ">";
                }
                else
                {
                    switch (args.Vertex.Title)
                    {
                        case "Initial":
                            args.VertexFormat.FillColor = GraphvizColor.White;
                            break;
                        case "Final":
                            args.VertexFormat.FillColor = GraphvizColor.White;
                            break;
                        default:
                            args.VertexFormat.FillColor = GraphvizColor.White;
                            args.VertexFormat.FontColor = GraphvizColor.Black;
                            break;
                    }

                    args.VertexFormat.Shape = GraphvizVertexShape.Ellipse;
                }
            };
            return algorithm.Generate();
        }
    }
}
