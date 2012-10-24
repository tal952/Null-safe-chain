using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NullSafeChain
{
    public class NullSafeChain
    {
        #region Data Members

        private readonly IEnumerable<Expression> _disassembleExpression;

        #endregion
        
        #region ctors

        public NullSafeChain(LambdaExpression callChain)
            : this(callChain.Body)
        {
        }

        protected NullSafeChain(Expression callChain)
        {
            _disassembleExpression = DisassembleExpression(callChain);
        }

        #endregion

        #region Public Methods

        public virtual object ExploreChain(object target)
        {
            object result = null;

            foreach (var expression in _disassembleExpression)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        {
                            result = GetMemberAccessValue((MemberExpression) expression, result);
                            if (result == null) return null;

                            break;
                        }
                    case ExpressionType.Call:
                        {
                            result = GetMethodCallValue((MethodCallExpression) expression, result);
                            if (result == null) return null;

                            break;
                        }
                    case ExpressionType.Parameter:
                        {
                            result = target;
                            break;
                        }
                    case ExpressionType.Constant:
                        {
                            result = ((ConstantExpression) expression).Value;
                            break;
                        }
                    case ExpressionType.Convert:
                        {
                            OnExplicitCast((UnaryExpression) expression, result);

                            break;
                        }
                    case ExpressionType.TypeAs:
                        {
                            result = GetTypeAsValue((UnaryExpression) expression, result);
                            if (result == null) return null;

                            break;
                        }
                    case ExpressionType.Lambda:
                        {
                            result = expression;
                            break;
                        }
                    default:
                        throw new NotSupportedException(string.Format("Expression type '{0}' is not supported.",
                                                                      expression.NodeType));
                }
            }

            return result;
        }

        #endregion
        
        #region Protected Methods

        protected virtual object GetMemberAccessValue(MemberExpression memberExpression, object target)
        {
            if (memberExpression.Member is PropertyInfo)
                return ((PropertyInfo) memberExpression.Member).GetValue(target, null);
            else
                return ((FieldInfo) memberExpression.Member).GetValue(target);
        }

        protected virtual object GetMethodCallValue(MethodCallExpression methodCallExpression, object target)
        {
            var arguments =
                methodCallExpression.Arguments.Select(a => new NullSafeChain(a).ExploreChain(target)).ToArray();

            // Converting expression parameters to delegate calls
            var parameters = methodCallExpression.Method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (typeof (Delegate).IsAssignableFrom(parameters[i].ParameterType))
                {
                    arguments[i] = ((LambdaExpression) arguments[i]).Compile();
                }
            }

            if (arguments.Length > 0 && 
                arguments[0] == null && 
                methodCallExpression.Method.IsStatic &&
                methodCallExpression.Method.IsDefined(typeof (ExtensionAttribute), false))
                // For extension method
            {
                return null;
            }
            else
            {
                return methodCallExpression.Method.Invoke(target, arguments);
            }
        }

        protected virtual void OnExplicitCast(UnaryExpression unaryExpression, object target)
        {
            if (!unaryExpression.Type.IsInstanceOfType(target))
            {
                throw new InvalidCastException(
                    string.Format("Unable to cast object of type '{0}' to type '{1}'.",
                                  target.GetType(), unaryExpression.Type));
            }
        }

        protected virtual object GetTypeAsValue(UnaryExpression unaryExpression, object target)
        {
            if (target == null || !unaryExpression.Type.IsInstanceOfType(target))
            {
                return null;
            }

            return target;
        }

        protected virtual IEnumerable<Expression> DisassembleExpression(Expression expression)
        {
            var expressionList = new LinkedList<Expression>();
            Expression current = expression;

            while (current != null)
            {
                expressionList.AddFirst(current);

                switch (current.NodeType)
                {
                    case ExpressionType.Constant:
                    case ExpressionType.Parameter:
                    case ExpressionType.Lambda:
                        {
                            current = null;
                            break;
                        }
                    case ExpressionType.TypeAs:
                    case ExpressionType.Convert:
                        {
                            current = ((UnaryExpression) current).Operand;
                            break;
                        }
                    case ExpressionType.MemberAccess:
                        {
                            current = ((MemberExpression) current).Expression;
                            break;
                        }
                    case ExpressionType.Call:
                        {
                            current = ((MethodCallExpression) current).Object;
                            break;
                        }
                    default:
                        throw new NotSupportedException(string.Format("Expression type '{0}' is not supported.",
                                                                      current.NodeType));
                }
            }

            return expressionList;
        }

        #endregion
    }
}