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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SK.EntityFrameworkCore.Interbase.Query.Internal;
using SK.EntityFrameworkCore.Interbase.Storage.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace SK.EntityFrameworkCore.Interbase.Query.ExpressionTranslators.Internal;

public class InterbaseConvertTranslator : IMethodCallTranslator
{
	static readonly List<string> Mappings = new List<string>
	{
		nameof(Convert.ToByte),
		nameof(Convert.ToDecimal),
		nameof(Convert.ToDouble),
		nameof(Convert.ToInt16),
		nameof(Convert.ToInt32),
		nameof(Convert.ToInt64),
		nameof(Convert.ToString),
	};

	static readonly HashSet<Type> SupportedTypes = new HashSet<Type>
	{
		typeof(bool),
		typeof(byte),
		typeof(decimal),
		typeof(double),
		typeof(float),
		typeof(int),
		typeof(long),
		typeof(short),
		typeof(string),
		typeof(DateTime),
	};

	static readonly IEnumerable<MethodInfo> SupportedMethods
		= Mappings
			.SelectMany(t => typeof(Convert).GetTypeInfo().GetDeclaredMethods(t)
				.Where(m => m.GetParameters().Length == 1 && SupportedTypes.Contains(m.GetParameters().First().ParameterType)));

	readonly InterbaseSqlExpressionFactory _interbaseSqlExpressionFactory;

	public InterbaseConvertTranslator(InterbaseSqlExpressionFactory interbaseSqlExpressionFactory)
	{
		_interbaseSqlExpressionFactory = interbaseSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (!SupportedMethods.Contains(method))
			return null;

		return _interbaseSqlExpressionFactory.ApplyDefaultTypeMapping(
			_interbaseSqlExpressionFactory.Convert(_interbaseSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]), method.ReturnType));
	}
}
