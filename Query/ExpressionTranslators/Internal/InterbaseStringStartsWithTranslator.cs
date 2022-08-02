/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/raw/master/license.txt.
 *
 *    Software distributed under the License is distributed on
 *    an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *    express or implied. See the License for the specific
 *    language governing rights and limitations under the License.
 *
 *    The Initial Developer(s) of the Original Code are listed below.
 *
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System.Collections.Generic;
using System.Reflection;
using SK.EntityFrameworkCore.Interbase.Query.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace SK.EntityFrameworkCore.Interbase.Query.ExpressionTranslators.Internal;

public class InterbaseStringStartsWithTranslator : IMethodCallTranslator
{
	static readonly MethodInfo StartsWithMethod = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) });

	readonly InterbaseSqlExpressionFactory _interbaseSqlExpressionFactory;

	public InterbaseStringStartsWithTranslator(InterbaseSqlExpressionFactory interbaseSqlExpressionFactory)
	{
		_interbaseSqlExpressionFactory = interbaseSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (!method.Equals(StartsWithMethod))
			return null;

		var patternExpression = _interbaseSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]);

		var startsWithExpression = _interbaseSqlExpressionFactory.Like(
			instance,
			_interbaseSqlExpressionFactory.Add(
				patternExpression,
				_interbaseSqlExpressionFactory.Constant("%")
			)
		);

		if (patternExpression is SqlConstantExpression sqlConstantExpression)
		{
			if (sqlConstantExpression.Value is null)
			{
				throw new System.ArgumentNullException();
			}

			return (string)sqlConstantExpression.Value == string.Empty
				? (SqlExpression)_interbaseSqlExpressionFactory.Constant(true)
				: startsWithExpression;
		}
		else
		{
			return _interbaseSqlExpressionFactory.OrElse(
				startsWithExpression,
				_interbaseSqlExpressionFactory.Equal(
					patternExpression,
					_interbaseSqlExpressionFactory.Constant(string.Empty)
				)
			);
		}
	}
}
