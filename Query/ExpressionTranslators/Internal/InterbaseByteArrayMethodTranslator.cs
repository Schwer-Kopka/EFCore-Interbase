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

public class InterbaseByteArrayMethodTranslator : IMethodCallTranslator
{
	readonly InterbaseSqlExpressionFactory _interbaseSqlExpressionFactory;

	public InterbaseByteArrayMethodTranslator(InterbaseSqlExpressionFactory interbaseSqlExpressionFactory)
	{
		_interbaseSqlExpressionFactory = interbaseSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (method.IsGenericMethod
			&& method.GetGenericMethodDefinition().Equals(EnumerableMethods.Contains)
			&& arguments[0].Type == typeof(byte[]))
		{
			var value = arguments[1] is SqlConstantExpression constantValue
				? _interbaseSqlExpressionFactory.Function("ASCII_CHAR", new[] { _interbaseSqlExpressionFactory.Constant((byte)constantValue.Value) }, false, new[] { false }, typeof(string))
				: _interbaseSqlExpressionFactory.Function("ASCII_CHAR", new[] { _interbaseSqlExpressionFactory.Convert(_interbaseSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]), typeof(byte)) }, true, new[] { true }, typeof(string));

			return _interbaseSqlExpressionFactory.GreaterThan(
				_interbaseSqlExpressionFactory.Function(
					"POSITION",
					new[] { value, _interbaseSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]) },
					true,
					new[] { true, true },
					typeof(int)),
				_interbaseSqlExpressionFactory.Constant(0));
		}
		return null;
	}
}
