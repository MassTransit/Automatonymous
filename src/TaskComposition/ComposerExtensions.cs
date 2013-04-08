// Copyright 2012-2012 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
// except in compliance with the License. You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software distributed under the
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
// ANY KIND, either express or implied. See the License for the specific language governing
// permissions and limitations under the License.
namespace TaskComposition
{
    using System;


    public static class ComposerExtensions
    {
        /// <summary>
        /// Adds a continuation that is always run, regardless of a successful or exceptional condition
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="continuation"></param>
        /// <param name="runSynchronously"></param>
        public static Composer Finally(this Composer composer, Action continuation, bool runSynchronously = true)
        {
            return composer.Finally(taskStatus => continuation(), runSynchronously);
        }
    }
}