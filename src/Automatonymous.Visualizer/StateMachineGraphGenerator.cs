// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
namespace Automatonymous.Visualizer
{
    using System;
    using System.Linq;
    using Graphing;
    using QuickGraph;
    using QuickGraph.Graphviz;
    using QuickGraph.Graphviz.Dot;


    public class StateMachineGraphvizGenerator
    {
        readonly AdjacencyGraph<Vertex, Edge<Vertex>> _graph;

        public StateMachineGraphvizGenerator(StateMachineGraph data)
        {
            _graph = CreateAdjacencyGraph(data);
        }

        public string CreateDotFile()
        {
            var algorithm = new GraphvizAlgorithm<Vertex, Edge<Vertex>>(_graph);
            algorithm.FormatEdge += EdgeStyler;
            algorithm.FormatVertex += VertexStyler;
            return algorithm.Generate();
        }

        void EdgeStyler(object sender, FormatEdgeEventArgs<Vertex, Edge<Vertex>> e)
        {
        }

        void VertexStyler(object sender, FormatVertexEventArgs<Vertex> e)
        {
            e.VertexFormatter.Label = e.Vertex.Title;

            if (e.Vertex.VertexType == typeof(Event))
            {
                e.VertexFormatter.FontColor = GraphvizColor.Black;
                e.VertexFormatter.Shape = GraphvizVertexShape.Plaintext;

                if (e.Vertex.TargetType != typeof(Event) && e.Vertex.TargetType != typeof(Exception))
                    e.VertexFormatter.Label += "<" + e.Vertex.TargetType.Name + ">";
            }
            else
            {
                switch (e.Vertex.Title)
                {
                    case "Initial":
                        e.VertexFormatter.FillColor = GraphvizColor.White;
                        break;
                    case "Final":
                        e.VertexFormatter.FillColor = GraphvizColor.White;
                        break;
                    default:
                        e.VertexFormatter.FillColor = GraphvizColor.White;
                        e.VertexFormatter.FontColor = GraphvizColor.Black;
                        break;
                }

                e.VertexFormatter.Shape = GraphvizVertexShape.Ellipse;
            }
        }

        static AdjacencyGraph<Vertex, Edge<Vertex>> CreateAdjacencyGraph(StateMachineGraph data)
        {
            var graph = new AdjacencyGraph<Vertex, Edge<Vertex>>();

            graph.AddVertexRange(data.Vertices);
            graph.AddEdgeRange(data.Edges.Select(x => new Edge<Vertex>(x.From, x.To)));
            return graph;
        }
    }
}