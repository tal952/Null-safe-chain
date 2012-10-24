using System;
using System.Linq.Expressions;

namespace NullSafeChain
{
    public static class NullSafeChainExtensions
    {
        public static TResult NullSafeChain<TTarget,TResult>(this TTarget target, Expression<Func<TTarget,TResult>> callChain)
            where TTarget : class 
            where TResult : class
        {
            return (TResult) new NullSafeChain(callChain).ExploreChain(target);
        }
    }
}
