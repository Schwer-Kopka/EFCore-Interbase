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
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace SK.EntityFrameworkCore.Interbase.Update.Internal;

public class InterbaseUpdateSqlGenerator : UpdateSqlGenerator, IInterbaseUpdateSqlGenerator
{
    public InterbaseUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies)
        : base(dependencies)
	{ }

	protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, IColumnModification columnModification)
		=> throw new InvalidOperationException();

	protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
		=> throw new InvalidOperationException();

	public override string GenerateNextSequenceValueOperation(string name, string schema)
	{
		var builder = new StringBuilder();
		builder.Append("SELECT GEN_ID(");
		builder.Append(SqlGenerationHelper.DelimitIdentifier(name));
		builder.Append(", 1) FROM RDB$DATABASE");
		return builder.ToString();
	}
}
