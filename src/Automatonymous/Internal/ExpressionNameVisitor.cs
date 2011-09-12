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
    using System.Linq.Expressions;
    using System.Text;


    class ExpressionNameVisitor :
        ExpressionVisitor
    {
        readonly StringBuilder _text = new StringBuilder();

        public override string ToString()
        {
            return _text.ToString();
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            _text.AppendFormat("Call {0}{1}", GetStaticMethodPrefixIfRequired(m), m.Method.Name);

            return base.VisitMethodCall(m);
        }

        static string GetStaticMethodPrefixIfRequired(MethodCallExpression m)
        {
            if (m.Object != null)
                return "";

            return m.Method.DeclaringType.Name + ".";
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            _text.Append(string.Format("{0}", m.Member.Name));

            return base.VisitMember(m);
        }


        public override Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;

            switch (exp.NodeType)
            {
                case ExpressionType.MemberAccess:
                case ExpressionType.Parameter:
                case ExpressionType.Call:
                    break;

                default:
                    _text.AppendFormat("({0})", exp.NodeType);
                    break;
            }

            return base.Visit(exp);
        }
    }
}