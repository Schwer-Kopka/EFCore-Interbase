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
using SK.EntityFrameworkCore.Interbase.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace SK.EntityFrameworkCore.Interbase.Query.Internal;

public class InterbaseSqlExpressionFactory : SqlExpressionFactory
{
	public InterbaseSqlExpressionFactory(SqlExpressionFactoryDependencies dependencies)
		: base(dependencies)
	{ }

	public InterbaseSpacedFunctionExpression SpacedFunction(string name, IEnumerable<SqlExpression> arguments, bool nullable, IEnumerable<bool> argumentsPropagateNullability, Type type, RelationalTypeMapping typeMapping = null)
		=> (InterbaseSpacedFunctionExpression)ApplyDefaultTypeMapping(new InterbaseSpacedFunctionExpression(name, arguments, nullable, argumentsPropagateNullability, type, typeMapping));

	public override SqlExpression ApplyTypeMapping(SqlExpression sqlExpression, RelationalTypeMapping typeMapping)
		=> sqlExpression == null || sqlExpression.TypeMapping != null
			? sqlExpression
			: sqlExpression switch
			{
				InterbaseSpacedFunctionExpression e => e.ApplyTypeMapping(typeMapping),
				_ => base.ApplyTypeMapping(sqlExpression, typeMapping)
			};
}
