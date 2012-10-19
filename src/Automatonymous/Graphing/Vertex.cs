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
namespace Automatonymous.Graphing
{
    using System;


#if !NETFX_CORE
    [Serializable]
#endif
    public class Vertex
    {
        public Vertex(Type type, Type targetType, string title)
        {
            VertexType = type;
            TargetType = targetType;
            Title = title;
        }

        public string Title { get; private set; }

        public Type VertexType { get; private set; }

        public Type TargetType { get; private set; }
    }
}