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
using System.Reflection;
using SK.EntityFrameworkCore.Interbase.Query.Expressions.Internal;
using SK.EntityFrameworkCore.Interbase.Query.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace SK.EntityFrameworkCore.Interbase.Query.ExpressionTranslators.Internal;

public class InterbaseTimeOnlyPartComponentTranslator : IMemberTranslator
{
	const string SecondPart = "SECOND";
	const string MillisecondPart = "MILLISECOND";
	private static readonly Dictionary<MemberInfo, string> MemberMapping = new Dictionary<MemberInfo, string>
		{
			{  typeof(TimeOnly).GetProperty(nameof(TimeOnly.Hour)), "HOUR" },
			{  typeof(TimeOnly).GetProperty(nameof(TimeOnly.Minute)), "MINUTE" },
			{  typeof(TimeOnly).GetProperty(nameof(TimeOnly.Second)), SecondPart },
			{  typeof(TimeOnly).GetProperty(nameof(TimeOnly.Millisecond)), MillisecondPart },
		};

	readonly InterbaseSqlExpressionFactory _interbaseSqlExpressionFactory;

	public InterbaseTimeOnlyPartComponentTranslator(InterbaseSqlExpressionFactory interbaseSqlExpressionFactory)
	{
		_interbaseSqlExpressionFactory = interbaseSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (!MemberMapping.TryGetValue(member, out var part))
			return null;

		var result = (SqlExpression)_interbaseSqlExpressionFactory.SpacedFunction(
			"EXTRACT",
			new[] { _interbaseSqlExpressionFactory.Fragment(part), _interbaseSqlExpressionFactory.Fragment("FROM"), instance },
			true,
			new[] { false, false, true },
			typeof(int));
		if (part == SecondPart || part == MillisecondPart)
		{
			result = _interbaseSqlExpressionFactory.Function("TRUNC", new[] { result }, true, new[] { true }, typeof(int));
		}
		return result;
	}
}
