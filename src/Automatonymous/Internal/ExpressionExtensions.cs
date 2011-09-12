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
namespace Automatonymous.Internal
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;


    static class ExpressionExtensions
    {
        public static PropertyInfo GetPropertyInfo(this Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression != null)
                expression = lambdaExpression.Body;

            var memberExpression = expression as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Must be a member expression");

            if (memberExpression.Member.MemberType != MemberTypes.Property)
                throw new ArgumentException("Must be a property expression");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("Not a property, wtF?");

            return property;
        }
    }
}