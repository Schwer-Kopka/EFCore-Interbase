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
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore;

public static class InterbaseModelExtensions
{
	public static void SetValueGenerationStrategy(this IMutableModel model, InterbaseValueGenerationStrategy? value)
		=> model.SetOrRemoveAnnotation(InterbaseAnnotationNames.ValueGenerationStrategy, value);

	public static void SetValueGenerationStrategy(this IConventionModel model, InterbaseValueGenerationStrategy? value, bool fromDataAnnotation = false)
		=> model.SetOrRemoveAnnotation(InterbaseAnnotationNames.ValueGenerationStrategy, value, fromDataAnnotation);

	public static InterbaseValueGenerationStrategy? GetValueGenerationStrategy(this IModel model)
		=> (InterbaseValueGenerationStrategy?)model[InterbaseAnnotationNames.ValueGenerationStrategy];

	public static InterbaseValueGenerationStrategy? GetValueGenerationStrategy(this IMutableModel model)
		=> (InterbaseValueGenerationStrategy?)model[InterbaseAnnotationNames.ValueGenerationStrategy];

	public static InterbaseValueGenerationStrategy? GetValueGenerationStrategy(this IConventionModel model)
		=> (InterbaseValueGenerationStrategy?)model[InterbaseAnnotationNames.ValueGenerationStrategy];
}
