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
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;

namespace SK.EntityFrameworkCore.Interbase.Storage.Internal;

public class InterbaseSqlGenerationHelper : RelationalSqlGenerationHelper, IInterbaseSqlGenerationHelper
{
	public InterbaseSqlGenerationHelper(RelationalSqlGenerationHelperDependencies dependencies)
		: base(dependencies)
	{ }

	public virtual string StringLiteralQueryType(string s)
	{
		var length = MinimumStringQueryTypeLength(s);
		EnsureStringLiteralQueryTypeLength(length);
		return $"VARCHAR({length}) CHARACTER SET UTF8";
	}

	public virtual string StringParameterQueryType(bool isUnicode)
	{
		var size = isUnicode ? InterbaseTypeMappingSource.UnicodeVarcharMaxSize : InterbaseTypeMappingSource.VarcharMaxSize;
		return $"VARCHAR({size})";
	}

	public virtual void GenerateBlockParameterName(StringBuilder builder, string name)
	{
		builder.Append(":").Append(name);
	}

	public string AlternativeStatementTerminator => "~";

	static int MinimumStringQueryTypeLength(string s)
	{
		var length = s?.Length ?? 0;
		if (length == 0)
			length = 1;
		return length;
	}

	static void EnsureStringLiteralQueryTypeLength(int length)
	{
		if (length > InterbaseTypeMappingSource.UnicodeVarcharMaxSize)
			throw new ArgumentOutOfRangeException(nameof(length));
	}
}
