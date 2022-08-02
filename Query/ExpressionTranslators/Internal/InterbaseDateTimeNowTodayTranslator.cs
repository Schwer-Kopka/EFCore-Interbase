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
using System.Reflection;
using SK.EntityFrameworkCore.Interbase.Query.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace SK.EntityFrameworkCore.Interbase.Query.ExpressionTranslators.Internal;

public class InterbaseDateTimeNowTodayTranslator : IMemberTranslator
{
	readonly InterbaseSqlExpressionFactory _interbaseSqlExpressionFactory;

	public InterbaseDateTimeNowTodayTranslator(InterbaseSqlExpressionFactory interbaseSqlExpressionFactory)
	{
		_interbaseSqlExpressionFactory = interbaseSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (member.DeclaringType == typeof(DateTime) && member.Name == nameof(DateTime.Now))
		{
			return _interbaseSqlExpressionFactory.ApplyDefaultTypeMapping(_interbaseSqlExpressionFactory.NiladicFunction("CURRENT_TIMESTAMP", false, typeof(DateTime)));
		}
		if (member.DeclaringType == typeof(DateTime) && member.Name == nameof(DateTime.Today))
		{
			return _interbaseSqlExpressionFactory.ApplyDefaultTypeMapping(_interbaseSqlExpressionFactory.NiladicFunction("CURRENT_DATE", false, typeof(DateTime)));
		}
		return null;
	}
}
