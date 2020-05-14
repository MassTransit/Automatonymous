namespace Automatonymous.UserTypes
{
    using System;
    using System.Data.Common;
    using NHibernate;
    using NHibernate.Engine;
    using NHibernate.SqlTypes;
    using NHibernate.UserTypes;


    /// <summary>
    /// Used to map a CompositeEventStatus property to an int for storage by 
    /// NHibernate.
    /// </summary>
    public class CompositeEventStatusUserType :
        IUserType
    {
        static readonly SqlType[] _types = new[] {NHibernateUtil.Int32.SqlType};

        bool IUserType.Equals(object x, object y)
        {
            var xs = (CompositeEventStatus)x;
            var ys = (CompositeEventStatus)y;

            return xs.Equals(ys);
        }

        public int GetHashCode(object x)
        {
            return ((CompositeEventStatus)x).GetHashCode();
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            object value = NHibernateUtil.Int32.NullSafeGet(rs, names, session, owner);
            if (value == null)
                return new CompositeEventStatus();

            var status = new CompositeEventStatus((Int32)value);

            return status;
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (value == null)
            {
                NHibernateUtil.Int32.NullSafeSet(cmd, 0, index, session);
                return;
            }

            int setValue = ((CompositeEventStatus)value).Bits;

            NHibernateUtil.Int32.NullSafeSet(cmd, setValue, index, session);
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
            get { return typeof(CompositeEventStatus); }
        }

        public bool IsMutable
        {
            get { return false; }
        }
    }
}
