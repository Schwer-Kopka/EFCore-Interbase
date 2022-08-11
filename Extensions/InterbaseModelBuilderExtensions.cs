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

using SK.EntityFrameworkCore.Interbase.Metadata;
using SK.EntityFrameworkCore.Interbase.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore;

public static class InterbaseModelBuilderExtensions
{
	public static ModelBuilder UseIdentityColumns(this ModelBuilder modelBuilder)
	{
		var model = modelBuilder.Model;
		model.SetValueGenerationStrategy(InterbaseValueGenerationStrategy.IdentityColumn);
		return modelBuilder;
	}

	public static ModelBuilder UseSequenceTriggers(this ModelBuilder modelBuilder)
	{
		var model = modelBuilder.Model;
		model.SetValueGenerationStrategy(InterbaseValueGenerationStrategy.SequenceTrigger);
		return modelBuilder;
	}

	public static IConventionModelBuilder HasValueGenerationStrategy(this IConventionModelBuilder modelBuilder, InterbaseValueGenerationStrategy? valueGenerationStrategy, bool fromDataAnnotation = false)
	{
		if (modelBuilder.CanSetAnnotation(InterbaseAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy, fromDataAnnotation))
		{
			modelBuilder.Metadata.SetValueGenerationStrategy(valueGenerationStrategy, fromDataAnnotation);
			if (valueGenerationStrategy != InterbaseValueGenerationStrategy.IdentityColumn)
			{
			}
			if (valueGenerationStrategy != InterbaseValueGenerationStrategy.SequenceTrigger)
			{
			}
			return modelBuilder;
		}
		return null;
	}
}
