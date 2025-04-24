﻿using System.Linq.Expressions;
using System.Reflection;

namespace Application.Filters.Abstractions;

internal static class QueryableExtensions
{
	public static IOrderedQueryable<T> NewOrder<T>(
		this IQueryable<T> source,
		string property,
		QueryableOrderType queryableOrderType)
		=> ApplyOrder(source, property, queryableOrderType.ToString());

	public static IOrderedQueryable<T> OrderBy<T>(
		this IQueryable<T> source,
		string property)
		=> source.NewOrder(property, QueryableOrderType.OrderBy);

	public static IOrderedQueryable<T> OrderByDescending<T>(
		this IQueryable<T> source,
		string property)
		=> source.NewOrder(property, QueryableOrderType.OrderByDescending);

	public static IOrderedQueryable<T> ThenBy<T>(
		this IOrderedQueryable<T> source,
		string property)
		=> source.NewOrder(property, QueryableOrderType.ThenBy);

	public static IOrderedQueryable<T> ThenByDescending<T>(
		this IOrderedQueryable<T> source,
		string property)
		=> source.NewOrder(property, QueryableOrderType.ThenByDescending);

	private static IOrderedQueryable<T> ApplyOrder<T>(
		IQueryable<T> source,
		string property,
		string methodName)
	{
		string[] props = property.Split('.');
		Type type = typeof(T);
		ParameterExpression arg = Expression.Parameter(type, "x");
		Expression expr = arg;
		foreach (string prop in props)
		{
			// use reflection (not ComponentModel) to mirror LINQ
			PropertyInfo pi = type.GetProperty(prop);
			expr = Expression.Property(expr, pi);
			type = pi.PropertyType;
		}
		Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
		LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

		object result = typeof(Queryable).GetMethods().Single(
				method => method.Name == methodName
						&& method.IsGenericMethodDefinition
						&& method.GetGenericArguments().Length == 2
						&& method.GetParameters().Length == 2)
				.MakeGenericMethod(typeof(T), type)
				.Invoke(null, new object[] { source, lambda });
		return (IOrderedQueryable<T>)result;
	}
}