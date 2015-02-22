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
namespace Automatonymous
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class UnhandledEventException :
        AutomatonymousException
    {
        public UnhandledEventException()
        {
        }

        public UnhandledEventException(string machineType, string eventName, string stateName)
            : base(string.Format("The {0} event is not handled during the {1} state for the {2} state machine", eventName, stateName,
                machineType))
        {
        }

        protected UnhandledEventException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}