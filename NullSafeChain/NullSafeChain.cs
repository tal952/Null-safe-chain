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
            object currentResult = null;

            foreach (var expression in _disassembleExpression)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        {
                            currentResult = GetMemberAccessValue((MemberExpression) expression, currentResult);
                            if (currentResult == null) return null;

                            break;
                        }
                    case ExpressionType.Call:
                        {
                            currentResult = GetMethodCallValue((MethodCallExpression)expression, target, currentResult);
                            if (currentResult == null) return null;

                            break;
                        }
                    case ExpressionType.Parameter:
                        {
                            currentResult = target;
                            break;
                        }
                    case ExpressionType.Constant:
                        {
                            currentResult = ((ConstantExpression) expression).Value;
                            break;
                        }
                    case ExpressionType.Convert:
                        {
                            OnExplicitCast((UnaryExpression) expression, currentResult);

                            break;
                        }
                    case ExpressionType.TypeAs:
                        {
                            currentResult = GetTypeAsValue((UnaryExpression) expression, currentResult);
                            if (currentResult == null) return null;

                            break;
                        }
                    case ExpressionType.Lambda:
                        {
                            currentResult = expression;
                            break;
                        }
                    default:
                        throw new NotSupportedException(string.Format("Expression type '{0}' is not supported.",
                                                                      expression.NodeType));
                }
            }

            return currentResult;
        }

        #endregion
        
        #region Protected Methods

        protected virtual object GetMemberAccessValue(MemberExpression memberExpression, object currentResult)
        {
            if (memberExpression.Member is PropertyInfo)
                return ((PropertyInfo) memberExpression.Member).GetValue(currentResult, null);
            else
                return ((FieldInfo) memberExpression.Member).GetValue(currentResult);
        }

        protected virtual object GetMethodCallValue(MethodCallExpression methodCallExpression, object target, object currentResult)
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
                return methodCallExpression.Method.Invoke(currentResult, arguments);
            }
        }

        protected virtual void OnExplicitCast(UnaryExpression unaryExpression, object currentResult)
        {
            if (!unaryExpression.Type.IsInstanceOfType(currentResult))
            {
                throw new InvalidCastException(
                    string.Format("Unable to cast object of type '{0}' to type '{1}'.",
                                  currentResult.GetType(), unaryExpression.Type));
            }
        }

        protected virtual object GetTypeAsValue(UnaryExpression unaryExpression, object currentResult)
        {
            if (currentResult == null || !unaryExpression.Type.IsInstanceOfType(currentResult))
            {
                return null;
            }

            return currentResult;
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