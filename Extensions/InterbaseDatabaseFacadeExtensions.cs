using System.Reflection;
using SK.EntityFrameworkCore.Interbase.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SK.EntityFrameworkCore.Interbase.Extensions;

using System;

/// <summary>
///		Interbase specific extension methods for <see cref="DatabaseFacade"/>.
/// </summary>
public static class InterbaseDatabaseFacadeExtensions
{
	/// <summary>
	///		<para>
	///			Returns true if the database provider currently in use is the Interbase provider.
	///		</para>
	///		<para>
	///			This method can only be used after the <see cref="DbContext" /> has been configured because
	///			it is only then that the provider is known. This means that this method cannot be used
	///			in <see cref="DbContext.OnConfiguring" /> because this is where application code sets the
	///			provider to use as part of configuring the context.
	///		</para>
	/// </summary>
	/// <param name="database">
	///		The facade from <see cref="DbContext.Database" />.
	/// </param>
	/// <returns>
	///		True if Interbase is being used; false otherwise.
	/// </returns>
	public static bool IsInterbase(this DatabaseFacade database)
		=> database.ProviderName.Equals(typeof(InterbaseOptionsExtension).GetTypeInfo().Assembly.GetName().Name, StringComparison.Ordinal);
}
