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

//$Authors = Siegfried Pammer (siegfried.pammer@gmail.com), Jiri Cincura (jiri@cincura.net)

using System;
using System.Data.Common;
using SK.InterbaseLibraryAdapter;
using Microsoft.EntityFrameworkCore.Storage;

namespace SK.EntityFrameworkCore.Interbase.Storage.Internal;

public class InterbaseTimeSpanTypeMapping : TimeSpanTypeMapping
{
	readonly InterbaseDbType _interbaseDbType;

	public InterbaseTimeSpanTypeMapping(string storeType, InterbaseDbType interbaseDbType)
		: base(storeType)
	{
		_interbaseDbType = interbaseDbType;
	}

	protected InterbaseTimeSpanTypeMapping(RelationalTypeMappingParameters parameters, InterbaseDbType interbaseDbType)
		: base(parameters)
	{
		_interbaseDbType = interbaseDbType;
	}

	protected override void ConfigureParameter(DbParameter parameter)
	{
		((InterbaseParameter)parameter).InterbaseDbType = _interbaseDbType;
	}

	protected override string GenerateNonNullSqlLiteral(object value)
	{
		switch (_interbaseDbType)
		{
			case InterbaseDbType.Time:
				return $"CAST('{value:hh\\:mm\\:ss\\.ffff}' AS TIME)";
			default:
				throw new ArgumentOutOfRangeException(nameof(_interbaseDbType), $"{nameof(_interbaseDbType)}={_interbaseDbType}");
		}
	}

	protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
		=> new InterbaseTimeSpanTypeMapping(parameters, _interbaseDbType);
}
