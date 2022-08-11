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

using SK.EntityFrameworkCore.Interbase.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace SK.EntityFrameworkCore.Interbase.Metadata.Conventions;

public class InterbaseValueGenerationConvention : RelationalValueGenerationConvention
{
	public InterbaseValueGenerationConvention(ProviderConventionSetBuilderDependencies dependencies, RelationalConventionSetBuilderDependencies relationalDependencies)
		: base(dependencies, relationalDependencies)
	{ }

	public override void ProcessPropertyAnnotationChanged(IConventionPropertyBuilder propertyBuilder, string name, IConventionAnnotation annotation, IConventionAnnotation oldAnnotation, IConventionContext<IConventionAnnotation> context)
	{
		if (name == InterbaseAnnotationNames.ValueGenerationStrategy)
		{
			propertyBuilder.ValueGenerated(GetValueGenerated(propertyBuilder.Metadata));
			return;
		}
		base.ProcessPropertyAnnotationChanged(propertyBuilder, name, annotation, oldAnnotation, context);
	}

	protected override ValueGenerated? GetValueGenerated(IConventionProperty property)
		=> RelationalValueGenerationConvention.GetValueGenerated(property)
			?? (property.GetValueGenerationStrategy() != InterbaseValueGenerationStrategy.None
				? ValueGenerated.OnAdd
				: (ValueGenerated?)null);
}
