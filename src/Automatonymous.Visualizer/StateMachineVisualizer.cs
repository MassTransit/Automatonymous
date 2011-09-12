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
    using System;
    using System.Windows.Forms;
    using Graphing;
    using Microsoft.VisualStudio.DebuggerVisualizers;


    public class StateMachineDebugVisualizer :
        DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            try
            {
                var data = (StateMachineGraph)objectProvider.GetObject();

                Microsoft.Glee.Drawing.Graph graph = new StateMachineGraphGenerator().CreateGraph(data);

                using (var form = new GraphVisualizerForm(graph, "StateMachine Visualizer"))
                    windowService.ShowDialog(form);
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("The selected data is not of a type compatible with this visualizer.",
                    GetType().ToString());
            }
        }

        public static void TestShowVisualizer(StateMachineGraph data)
        {
            var visualizerHost = new VisualizerDevelopmentHost(data, typeof(StateMachineDebugVisualizer));
            visualizerHost.ShowVisualizer();
        }
    }
}