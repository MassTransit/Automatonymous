﻿// Copyright 2011 Chris Patterson, Dru Sellers
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
namespace Automatonymous
{
    using System;
    using System.Runtime.Serialization;


#if !NETFX_CORE
    [Serializable]
#endif
    public class AutomatonymousException :
        Exception
    {
        public AutomatonymousException()
        {
        }

        public AutomatonymousException(string message)
            : base(message)
        {
        }

        public AutomatonymousException(Type machineType, string message)
            : base(string.Format("{0}: {1}", machineType.Name, message))
        {
        }

        public AutomatonymousException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AutomatonymousException(Type machineType, string message, Exception innerException)
            : base(string.Format("{0}: {1}", machineType.Name, message), innerException)
        {
        }
#if !NETFX_CORE
        protected AutomatonymousException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}