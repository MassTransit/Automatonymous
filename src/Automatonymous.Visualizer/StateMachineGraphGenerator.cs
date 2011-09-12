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
namespace Automatonymous.Visualizer
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using Graphing;
    using Microsoft.Glee.Drawing;
    using Microsoft.Glee.GraphViewerGdi;
    using QuickGraph;
    using QuickGraph.Glee;


    public class StateMachineGraphGenerator
    {
        public Microsoft.Glee.Drawing.Graph CreateGraph(StateMachineGraph data)
        {
            var graph = new AdjacencyGraph<Vertex, Edge<Vertex>>();

            graph.AddVertexRange(data.Vertices);
            graph.AddEdgeRange(data.Edges.Select(x => new Edge<Vertex>(x.From, x.To)));

            GleeGraphPopulator<Vertex, Edge<Vertex>> glee = graph.CreateGleePopulator();

            glee.NodeAdded += NodeStyler;
            glee.EdgeAdded += EdgeStyler;
            glee.Compute();

            Microsoft.Glee.Drawing.Graph gleeGraph = glee.GleeGraph;

            return gleeGraph;
        }

        public void SaveGraphToFile(StateMachineGraph data, int width, int height, string filename)
        {
            Microsoft.Glee.Drawing.Graph gleeGraph = CreateGraph(data);

            var renderer = new GraphRenderer(gleeGraph);
            renderer.CalculateLayout();

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            renderer.Render(bitmap);

            bitmap.Save(filename, ImageFormat.Png);
        }

        void NodeStyler(object sender, GleeVertexEventArgs<Vertex> args)
        {
            args.Node.Attr.Fontcolor = Microsoft.Glee.Drawing.Color.White;
            args.Node.Attr.Fontsize = 8;
            args.Node.Attr.FontName = "Arial";
            args.Node.Attr.Padding = 1.2;

            if (args.Vertex.VertexType == typeof(Event))
            {
                if (args.Vertex.Title.EndsWith("Exception"))
                {
                    args.Node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Red;
                    args.Node.Attr.Shape = Shape.Box;
                }
                else
                {
                    args.Node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Yellow;
                    args.Node.Attr.Shape = Shape.Ellipse;
                }
                args.Node.Attr.Label = args.Vertex.Title;
                args.Node.Attr.Fontcolor = Microsoft.Glee.Drawing.Color.Black;
            }
            else
            {
                switch (args.Vertex.Title)
                {
                    case "Initial":
                        args.Node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Black;
                        break;
                    case "Completed":
                        args.Node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Black;
                        break;
                    default:
                        args.Node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Blue;
                        break;
                }
                args.Node.Attr.Label = args.Vertex.Title;
                args.Node.Attr.Shape = Shape.Circle;
            }
        }

        static void EdgeStyler(object sender, GleeEdgeEventArgs<Vertex, Edge<Vertex>> e)
        {
            e.GEdge.EdgeAttr.FontName = "Tahoma";
            e.GEdge.EdgeAttr.Fontsize = 6;
        }
    }
}