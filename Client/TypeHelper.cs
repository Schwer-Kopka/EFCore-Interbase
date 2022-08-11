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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

// parts of this file are copied and adjusted from https://github.com/FirebirdSQL/NETProvider/blob/master/src/FirebirdSql.Data.FirebirdClient/Common/TypeHelper.cs

using System;
using System.Data;

namespace SK.InterbaseLibraryAdapter;

internal static class TypeHelper
{
	public static DbDataType GetDbDataTypeFromDbType(DbType type)
	{
		switch (type)
		{
			case DbType.String:
			case DbType.AnsiString:
				return DbDataType.VarChar;

			case DbType.StringFixedLength:
			case DbType.AnsiStringFixedLength:
				return DbDataType.Char;

			case DbType.Byte:
			case DbType.SByte:
			case DbType.Int16:
			case DbType.UInt16:
				return DbDataType.SmallInt;

			case DbType.Int32:
			case DbType.UInt32:
				return DbDataType.Integer;

			case DbType.Int64:
			case DbType.UInt64:
				return DbDataType.BigInt;

			case DbType.Date:
				return DbDataType.Date;

			case DbType.Time:
				return DbDataType.Time;

			case DbType.DateTime:
				return DbDataType.TimeStamp;

			case DbType.Object:
			case DbType.Binary:
				return DbDataType.Binary;

			case DbType.Decimal:
				return DbDataType.Decimal;

			case DbType.Double:
				return DbDataType.Double;

			case DbType.Single:
				return DbDataType.Float;

			case DbType.Guid:
				return DbDataType.Guid;

			case DbType.Boolean:
				return DbDataType.Boolean;

			default:
				throw InvalidDataType((int)type);
		}
	}

	public static DbType GetDbTypeFromDbDataType(DbDataType type)
	{
		switch (type)
		{
			case DbDataType.Array:
			case DbDataType.Binary:
				return DbType.Binary;

			case DbDataType.Text:
			case DbDataType.VarChar:
			case DbDataType.Char:
				return DbType.String;

			case DbDataType.SmallInt:
				return DbType.Int16;

			case DbDataType.Integer:
				return DbType.Int32;

			case DbDataType.BigInt:
				return DbType.Int64;

			case DbDataType.Date:
				return DbType.Date;

			case DbDataType.Time:
				return DbType.Time;

			case DbDataType.TimeStamp:
				return DbType.DateTime;

			case DbDataType.Numeric:
			case DbDataType.Decimal:
				return DbType.Decimal;

			case DbDataType.Float:
				return DbType.Single;

			case DbDataType.Double:
				return DbType.Double;

			case DbDataType.Guid:
				return DbType.Guid;

			case DbDataType.Boolean:
				return DbType.Boolean;

			case DbDataType.TimeStampTZ:
			case DbDataType.TimeStampTZEx:
			case DbDataType.TimeTZ:
			case DbDataType.TimeTZEx:
			case DbDataType.Dec16:
			case DbDataType.Dec34:
			case DbDataType.Int128:
				// nothing better at the moment
				return DbType.Object;

			default:
				throw InvalidDataType((int)type);
		}
	}

	public static InterbaseDbType GetInterbaseDataTypeFromType(Type type)
	{
		if (type.IsEnum)
		{
			return GetInterbaseDataTypeFromType(Enum.GetUnderlyingType(type));
		}

		if (type == typeof(System.DBNull))
		{
			return InterbaseDbType.VarChar;
		}

		if (type == typeof(System.String))
		{
			return InterbaseDbType.VarChar;
		}
		else if (type == typeof(System.Char))
		{
			return InterbaseDbType.Char;
		}
		else if (type == typeof(System.Boolean))
		{
			return InterbaseDbType.Boolean;
		}
		else if (type == typeof(System.Byte) || type == typeof(System.SByte) || type == typeof(System.Int16) || type == typeof(System.UInt16))
		{
			return InterbaseDbType.SmallInt;
		}
		else if (type == typeof(System.Int32) || type == typeof(System.UInt32))
		{
			return InterbaseDbType.Integer;
		}
		else if (type == typeof(System.Int64) || type == typeof(System.UInt64))
		{
			return InterbaseDbType.BigInt;
		}
		else if (type == typeof(System.Single))
		{
			return InterbaseDbType.Float;
		}
		else if (type == typeof(System.Double))
		{
			return InterbaseDbType.Double;
		}
		else if (type == typeof(System.Decimal))
		{
			return InterbaseDbType.Decimal;
		}
		else if (type == typeof(System.DateTime))
		{
			return InterbaseDbType.TimeStamp;
		}
		else if (type == typeof(System.TimeSpan))
		{
			return InterbaseDbType.Time;
		}
		else if (type == typeof(System.Guid))
		{
			return InterbaseDbType.Guid;
		}
		//else if (type == typeof(InterbaseZonedDateTime))
		//{
		//	return InterbaseDbType.TimeStampTZ;
		//}
		//else if (type == typeof(InterbaseZonedTime))
		//{
		//	return InterbaseDbType.TimeTZ;
		//}
		//else if (type == typeof(InterbaseDecFloat))
		//{
		//	return InterbaseDbType.Dec34;
		//}
		else if (type == typeof(System.Numerics.BigInteger))
		{
			return InterbaseDbType.Int128;
		}
		else if (type == typeof(System.Byte[]))
		{
			return InterbaseDbType.Binary;
		}
#if NET6_0_OR_GREATER
		else if (type == typeof(System.DateOnly))
		{
			return InterbaseDbType.Date;
		}
#endif
#if NET6_0_OR_GREATER
		else if (type == typeof(System.TimeOnly))
		{
			return InterbaseDbType.Time;
		}
#endif
		else
		{
			throw new ArgumentException($"Unknown type: {type}.");
		}
	}

	public static Exception InvalidDataType(int type)
				{
		return new ArgumentException($"Invalid data type: {type}.");
	}

	public static DbDataType GetDbDataTypeFromInterbaseDataType(InterbaseDbType type)
	{
		// these are aligned for this conversion
		return (DbDataType)type;
	}

	public static DbType GetDbTypeFromInterbaseDataType(InterbaseDbType type)
	{
		return GetDbTypeFromDbDataType(GetDbDataTypeFromInterbaseDataType(type));
	}
}
