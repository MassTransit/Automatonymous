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
namespace Automatonymous.NHibernateIntegration
{
    using System;
    using System.Data;
    using NHibernate;
    using NHibernate.SqlTypes;
    using NHibernate.UserTypes;


    public class CompositeEventStateUserType :
        IUserType
    {
        static readonly SqlType[] _types = new[] {NHibernateUtil.Int32.SqlType};

        bool IUserType.Equals(object x, object y)
        {
            var xs = (State)x;
            var ys = (State)y;

            return xs.Name.Equals(ys.Name);
        }

        public int GetHashCode(object x)
        {
            return ((State)x).Name.GetHashCode();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var value = (Int32)NHibernateUtil.Int32.NullSafeGet(rs, names);

            var status = new CompositeEventStatus(value);

            return status;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null)
            {
                NHibernateUtil.Int32.NullSafeSet(cmd, 0, index);
                return;
            }

            int setValue = ((CompositeEventStatus)value).Bits;

            NHibernateUtil.Int32.NullSafeSet(cmd, setValue, index);
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public SqlType[] SqlTypes
        {
            get { return _types; }
        }

        public Type ReturnedType
        {
            get { return typeof(State); }
        }

        public bool IsMutable
        {
            get { return false; }
        }
    }
}