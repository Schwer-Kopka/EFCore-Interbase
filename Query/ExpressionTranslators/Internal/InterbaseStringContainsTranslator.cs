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

public class InterbaseStringContainsTranslator : IMethodCallTranslator
{
	static readonly MethodInfo MethodInfo = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) });

	readonly InterbaseSqlExpressionFactory _interbaseSqlExpressionFactory;

	public InterbaseStringContainsTranslator(InterbaseSqlExpressionFactory interbaseSqlExpressionFactory)
	{
		_interbaseSqlExpressionFactory = interbaseSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (!method.Equals(MethodInfo))
			return null;

		var patternExpression = _interbaseSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]);
		var positionExpression = _interbaseSqlExpressionFactory.GreaterThan(
			_interbaseSqlExpressionFactory.Function(
				"POSITION",
				new[] { patternExpression, instance },
				true,
				new[] { true, true },
				typeof(int)),
			_interbaseSqlExpressionFactory.Constant(0));
		return patternExpression is SqlConstantExpression sqlConstantExpression
			? ((string)sqlConstantExpression.Value)?.Length == 0
				? (SqlExpression)_interbaseSqlExpressionFactory.Constant(true)
				: positionExpression
			: _interbaseSqlExpressionFactory.OrElse(
				positionExpression,
				_interbaseSqlExpressionFactory.Equal(
					patternExpression,
					_interbaseSqlExpressionFactory.Constant(string.Empty)));
	}
}
