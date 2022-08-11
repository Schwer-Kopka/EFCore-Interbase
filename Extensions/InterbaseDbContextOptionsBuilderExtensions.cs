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

using System;
using System.Data.Common;
using SK.EntityFrameworkCore.Interbase.Infrastructure;
using SK.EntityFrameworkCore.Interbase.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

public static class InterbaseDbContextOptionsBuilderExtensions
{
	public static DbContextOptionsBuilder UseInterbase(this DbContextOptionsBuilder optionsBuilder, string connectionString, Action<InterbaseDbContextOptionsBuilder> interbaseOptionsAction = null)
	{
		var extension = (InterbaseOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);
		((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
		interbaseOptionsAction?.Invoke(new InterbaseDbContextOptionsBuilder(optionsBuilder));
		return optionsBuilder;
	}

	public static DbContextOptionsBuilder UseInterbase(this DbContextOptionsBuilder optionsBuilder, DbConnection connection, Action<InterbaseDbContextOptionsBuilder> interbaseOptionsAction = null)
	{
		var extension = (InterbaseOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnection(connection);
		((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
		interbaseOptionsAction?.Invoke(new InterbaseDbContextOptionsBuilder(optionsBuilder));
		return optionsBuilder;
	}

	public static DbContextOptionsBuilder<TContext> UseInterbase<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, string connectionString, Action<InterbaseDbContextOptionsBuilder> interbaseOptionsAction = null)
		where TContext : DbContext
	{
		return (DbContextOptionsBuilder<TContext>)UseInterbase((DbContextOptionsBuilder)optionsBuilder, connectionString, interbaseOptionsAction);
	}

	public static DbContextOptionsBuilder<TContext> UseInterbase<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, DbConnection connection, Action<InterbaseDbContextOptionsBuilder> interbaseOptionsAction = null)
		where TContext : DbContext
	{
		return (DbContextOptionsBuilder<TContext>)UseInterbase((DbContextOptionsBuilder)optionsBuilder, connection, interbaseOptionsAction);
	}

	static InterbaseOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.Options.FindExtension<InterbaseOptionsExtension>()
			?? new InterbaseOptionsExtension();
}
