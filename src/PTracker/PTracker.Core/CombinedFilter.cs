using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTracker.Core
{
    public class CombinedFilter<T> : IFilter<T> where T: class
    {
        IFilter<T> _left;
        IFilter<T> _right;
        Condition _condition;
        public CombinedFilter(IFilter<T> left, IFilter<T> right, Condition condition)
        {
            Guard.Check<IFilter<T>>(left,"left");
            _left = left;
            _right = right;
            _condition = condition;
        }

        #region IFilter<T> Members

        public bool Test(T obj)
        {
            switch (_condition)
            {
                case Condition.And:
                    return _left.Test(obj) && _right.Test(obj);
                case Condition.Or:
                default:
                    return _left.Test(obj) || _right.Test(obj);
            }
        }

        public IFilter<T> Combine(IFilter<T> other, Condition condition)
        {
            return new CombinedFilter<T>(this, other, condition);
        }

        public static IFilter<T> Combine(IFilter<T> left, IFilter<T> right, Condition condition)
        {
            return new CombinedFilter<T>(left, right, condition);
        }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", _left.ToString(),_condition,_right.ToString());
        }

        #endregion
    }
}
