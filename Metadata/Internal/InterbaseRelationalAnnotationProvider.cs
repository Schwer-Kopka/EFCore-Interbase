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

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SK.EntityFrameworkCore.Interbase.Metadata.Internal;

public class InterbaseRelationalAnnotationProvider : RelationalAnnotationProvider
{
#pragma warning disable EF1001
	public InterbaseRelationalAnnotationProvider(RelationalAnnotationProviderDependencies dependencies)
#pragma warning restore EF1001
			: base(dependencies)
	{ }

	public override IEnumerable<IAnnotation> For(IColumn column, bool designTime)
	{
		if (!designTime)
		{
			yield break;
		}

		var property = column.PropertyMappings.Select(x => x.Property)
			.FirstOrDefault(x => x.GetValueGenerationStrategy() != InterbaseValueGenerationStrategy.None);
		if (property != null)
		{
			var valueGenerationStrategy = property.GetValueGenerationStrategy();
			if (valueGenerationStrategy != InterbaseValueGenerationStrategy.None)
			{
				yield return new Annotation(InterbaseAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy);
			}
		}
	}
}
