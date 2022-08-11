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

public class InterbaseStringSubstringTranslator : IMethodCallTranslator
{
	static readonly MethodInfo SubstringOnlyStartMethod = typeof(string).GetRuntimeMethod(nameof(string.Substring), new[] { typeof(int) });
	static readonly MethodInfo SubstringStartAndLengthMethod = typeof(string).GetRuntimeMethod(nameof(string.Substring), new[] { typeof(int), typeof(int) });

	readonly InterbaseSqlExpressionFactory _interbaseSqlExpressionFactory;

	public InterbaseStringSubstringTranslator(InterbaseSqlExpressionFactory interbaseSqlExpressionFactory)
	{
		_interbaseSqlExpressionFactory = interbaseSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (!(method.Equals(SubstringOnlyStartMethod) || method.Equals(SubstringStartAndLengthMethod)))
			return null;

		var fromExpression = _interbaseSqlExpressionFactory.ApplyDefaultTypeMapping(_interbaseSqlExpressionFactory.Add(arguments[0], _interbaseSqlExpressionFactory.Constant(1)));
		var forExpression = arguments.Count == 2 ? _interbaseSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]) : null;
		var substringArguments = forExpression != null
			? new[] { instance, _interbaseSqlExpressionFactory.Fragment("FROM"), fromExpression, _interbaseSqlExpressionFactory.Fragment("FOR"), forExpression }
			: new[] { instance, _interbaseSqlExpressionFactory.Fragment("FROM"), fromExpression };
		var nullability = forExpression != null
			? new[] { true, false, true, false, true }
			: new[] { true, false, true };
		return _interbaseSqlExpressionFactory.SpacedFunction(
			"SUBSTRING",
			substringArguments,
			true,
			nullability,
			typeof(string));
	}
}
