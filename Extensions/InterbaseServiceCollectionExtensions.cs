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
using SK.EntityFrameworkCore.Interbase;
using SK.EntityFrameworkCore.Interbase.Diagnostics.Internal;
using SK.EntityFrameworkCore.Interbase.Infrastructure;
using SK.EntityFrameworkCore.Interbase.Infrastructure.Internal;
using SK.EntityFrameworkCore.Interbase.Internal;
using SK.EntityFrameworkCore.Interbase.Metadata.Conventions;
using SK.EntityFrameworkCore.Interbase.Metadata.Internal;
using SK.EntityFrameworkCore.Interbase.Migrations;
using SK.EntityFrameworkCore.Interbase.Migrations.Internal;
using SK.EntityFrameworkCore.Interbase.Query.ExpressionTranslators.Internal;
using SK.EntityFrameworkCore.Interbase.Query.Internal;
using SK.EntityFrameworkCore.Interbase.Storage.Internal;
using SK.EntityFrameworkCore.Interbase.Update.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore;

public static class InterbaseServiceCollectionExtensions
{
	public static IServiceCollection AddInterbase<TContext>(this IServiceCollection serviceCollection, string connectionString, Action<InterbaseDbContextOptionsBuilder> interbaseOptionsAction = null, Action<DbContextOptionsBuilder> optionsAction = null)
		where TContext : DbContext
	{
		return serviceCollection.AddDbContext<TContext>(
			(serviceProvider, options) =>
			{
				optionsAction?.Invoke(options);
				options.UseInterbase(connectionString, interbaseOptionsAction);
			});
	}

	public static IServiceCollection AddEntityFrameworkInterbase(this IServiceCollection serviceCollection)
	{
		var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
			.TryAdd<LoggingDefinitions, InterbaseLoggingDefinitions>()
			.TryAdd<IDatabaseProvider, DatabaseProvider<InterbaseOptionsExtension>>()
			.TryAdd<IRelationalDatabaseCreator, InterbaseDatabaseCreator>()
			.TryAdd<IRelationalTypeMappingSource, InterbaseTypeMappingSource>()
			.TryAdd<ISqlGenerationHelper, InterbaseSqlGenerationHelper>()
			.TryAdd<IRelationalAnnotationProvider, InterbaseRelationalAnnotationProvider>()
			.TryAdd<IProviderConventionSetBuilder, InterbaseConventionSetBuilder>()
			.TryAdd<IUpdateSqlGenerator>(p => p.GetService<IInterbaseUpdateSqlGenerator>())
			.TryAdd<IModificationCommandBatchFactory, InterbaseModificationCommandBatchFactory>()
			.TryAdd<IRelationalConnection>(p => p.GetService<IInterbaseRelationalConnection>())
			.TryAdd<IRelationalTransactionFactory, InterbaseTransactionFactory>()
			.TryAdd<IMigrationsSqlGenerator, InterbaseMigrationsSqlGenerator>()
			.TryAdd<IHistoryRepository, InterbaseHistoryRepository>()
			.TryAdd<IMemberTranslatorProvider, InterbaseMemberTranslatorProvider>()
			.TryAdd<IMethodCallTranslatorProvider, InterbaseMethodCallTranslatorProvider>()
			.TryAdd<IQuerySqlGeneratorFactory, InterbaseQuerySqlGeneratorFactory>()
			.TryAdd<ISqlExpressionFactory, InterbaseSqlExpressionFactory>()
			.TryAdd<ISingletonOptions, IInterbaseOptions>(p => p.GetService<IInterbaseOptions>())
			.TryAdd<IRelationalSqlTranslatingExpressionVisitorFactory, InterbaseSqlTranslatingExpressionVisitorFactory>()
			.TryAddProviderSpecificServices(b => b
				.TryAddSingleton<IInterbaseOptions, InterbaseOptions>()
				.TryAddSingleton<IInterbaseMigrationSqlGeneratorBehavior, InterbaseMigrationSqlGeneratorBehavior>()
				.TryAddSingleton<IInterbaseUpdateSqlGenerator, InterbaseUpdateSqlGenerator>()
				.TryAddScoped<IInterbaseRelationalConnection, InterbaseRelationalConnection>()
				.TryAddScoped<IInterbaseRelationalTransaction, InterbaseRelationalTransaction>());

		builder.TryAddCoreServices();

		return serviceCollection;
	}
}
