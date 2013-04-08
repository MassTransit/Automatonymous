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
namespace Automatonymous
{
    using TaskComposition;


    public interface Activity :
        AcceptStateMachineInspector
    {
    }


    public interface Activity<in TInstance> :
        Activity
    {
        void Execute(Composer composer, TInstance instance);

        void Execute<T>(Composer composer, TInstance instance, T value);
    }


    public interface Activity<in TInstance, in TData> :
        Activity
    {
        void Execute(Composer composer, TInstance instance, TData value);
    }
}