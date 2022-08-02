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
using SK.EntityFrameworkCore.Interbase.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace SK.EntityFrameworkCore.Interbase.Scaffolding.Internal;

public class InterbaseProviderCodeGenerator : ProviderCodeGenerator
{
	static readonly MethodInfo UseInterbaseMethodInfo
		= typeof(InterbaseDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
			nameof(InterbaseDbContextOptionsBuilderExtensions.UseInterbase),
			typeof(DbContextOptionsBuilder),
			typeof(string),
			typeof(Action<InterbaseDbContextOptionsBuilder>));

	public InterbaseProviderCodeGenerator(ProviderCodeGeneratorDependencies dependencies)
		: base(dependencies)
	{ }

	public override MethodCallCodeFragment GenerateUseProvider(string connectionString, MethodCallCodeFragment providerOptions)
	{
		return new MethodCallCodeFragment(
			UseInterbaseMethodInfo,
			providerOptions == null
				? new object[] { connectionString }
				: new object[] { connectionString, new NestedClosureCodeFragment("x", providerOptions) });
	}
}