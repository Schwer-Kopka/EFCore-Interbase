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

using Microsoft.EntityFrameworkCore.Query;

namespace SK.EntityFrameworkCore.Interbase.Query.Internal;

public class InterbaseSqlTranslatingExpressionVisitorFactory : IRelationalSqlTranslatingExpressionVisitorFactory
{
	readonly RelationalSqlTranslatingExpressionVisitorDependencies _dependencies;

	public InterbaseSqlTranslatingExpressionVisitorFactory(RelationalSqlTranslatingExpressionVisitorDependencies dependencies)
	{
		_dependencies = dependencies;
	}

	public virtual RelationalSqlTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext, QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor)
		=> new InterbaseSqlTranslatingExpressionVisitor(_dependencies, queryCompilationContext, queryableMethodTranslatingExpressionVisitor);
}