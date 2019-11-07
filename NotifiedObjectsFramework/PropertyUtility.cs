using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NotifiedObjectsFramework
{
    public static class PropertyUtility
    {
        public static string GetPropertyName<TObject>(this TObject type, Expression<Func<TObject, object>> propertySelectorExpr)
        {
            if(propertySelectorExpr is null) {
                throw new ArgumentNullException(nameof(propertySelectorExpr));
            }

            return GetPropertyName(propertySelectorExpr.Body);
        }

        public static string GetPropertyName<TObject>(Expression<Func<TObject, object>> propertySelectorExpr)
        {
            if(propertySelectorExpr is null) {
                throw new ArgumentNullException(nameof(propertySelectorExpr));
            }

            return GetPropertyName(propertySelectorExpr.Body);
        }

        public static string GetPropertyName<T>(Expression<Func<T>> propertySelectorExpr)
        {
            if(propertySelectorExpr == null){
                throw new ArgumentNullException(nameof(propertySelectorExpr));
            }
            MemberExpression body = propertySelectorExpr.Body as MemberExpression;
            if(body == null) {
                throw new ArgumentException("The body must be a member expression");
            }
            return body.Member.Name;
        }

        private static string GetPropertyName(Expression propertySelectorExpr)
        {
            if(propertySelectorExpr == null)
                throw new ArgumentNullException(nameof(propertySelectorExpr));

            MemberExpression memberExpr = propertySelectorExpr as MemberExpression;
            if(memberExpr == null) {
                UnaryExpression unaryExpr = propertySelectorExpr as UnaryExpression;
                if(unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                    memberExpr = unaryExpr.Operand as MemberExpression;
            }

            if(memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property) {
                return memberExpr.Member.Name;
            }

            throw new ArgumentException("No property reference expression was found.", nameof(propertySelectorExpr));
        }
    }
}

