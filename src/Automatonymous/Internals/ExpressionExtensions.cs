// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous.Internals
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;


    static class ExpressionExtensions
    {
        public static PropertyInfo GetPropertyInfo<T, TMember>(this Expression<Func<T, TMember>> expression)
        {
            return expression.GetMemberExpression().Member as PropertyInfo;
        }

        public static PropertyInfo GetPropertyInfo<T>(this Expression<Func<T>> expression)
        {
            return expression.GetMemberExpression().Member as PropertyInfo;
        }

        public static MemberExpression GetMemberExpression<T, TMember>(this Expression<Func<T, TMember>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return GetMemberExpression(expression.Body);
        }

        public static MemberExpression GetMemberExpression<T>(this Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            return GetMemberExpression(expression.Body);
        }

        static MemberExpression GetMemberExpression(Expression body)
        {
            if (body == null)
                throw new ArgumentNullException("body");

            MemberExpression memberExpression = null;
            if (body.NodeType == ExpressionType.Convert)
            {
                var unaryExpression = (UnaryExpression)body;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else if (body.NodeType == ExpressionType.MemberAccess)
                memberExpression = body as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException("Expression is not a member access");

            return memberExpression;
        }
    }
}