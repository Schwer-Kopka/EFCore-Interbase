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

using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;

using System.Text;

namespace SK.InterbaseLibraryAdapter;

[ListBindable(false)]
public sealed class InterbaseParameterCollection : DbParameterCollection
{
	#region Fields

	private List<InterbaseParameter> _parameters;
	private bool? _hasParameterWithNonAsciiName;

	#endregion

	#region Indexers

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new InterbaseParameter this[string parameterName]
	{
		get { return this[IndexOf(parameterName)]; }
		set { this[IndexOf(parameterName)] = value; }
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new InterbaseParameter this[int index]
	{
		get { return _parameters[index]; }
		set { _parameters[index] = value; }
	}

	#endregion

	#region DbParameterCollection overriden properties

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override int Count
	{
		get { return _parameters.Count; }
	}

	public override bool IsFixedSize
	{
		get { return ((IList)_parameters).IsFixedSize; }
	}

	public override bool IsReadOnly
	{
		get { return ((IList)_parameters).IsReadOnly; }
	}

	public override bool IsSynchronized
	{
		get { return ((ICollection)_parameters).IsSynchronized; }
	}

	public override object SyncRoot
	{
		get { return ((ICollection)_parameters).SyncRoot; }
	}

	#endregion

	#region Internal properties

	internal bool HasParameterWithNonAsciiName
	{
		get
		{
			return _hasParameterWithNonAsciiName ?? (bool)(_hasParameterWithNonAsciiName = _parameters.Any(x => x.IsUnicodeParameterName));
		}
	}

	#endregion

	#region Constructors

	internal InterbaseParameterCollection()
	{
		_parameters = new List<InterbaseParameter>();
		_hasParameterWithNonAsciiName = null;
	}

	#endregion

	#region DbParameterCollection overriden methods

	public void AddRange(IEnumerable<InterbaseParameter> values)
	{
		foreach (var p in values)
		{
			Add(p);
		}
	}

	public override void AddRange(Array values)
	{
		AddRange(values.Cast<object>().Select(x => { EnsureInterbaseParameterType(x); return (InterbaseParameter)x; }));
	}

	public InterbaseParameter AddWithValue(string parameterName, object value)
	{
		return Add(new InterbaseParameter(parameterName, value));
	}

	public InterbaseParameter Add(string parameterName, object value)
	{
		return Add(new InterbaseParameter(parameterName, value));
	}

	public InterbaseParameter Add(string parameterName, InterbaseDbType type)
	{
		return Add(new InterbaseParameter(parameterName, type));
	}

	public InterbaseParameter Add(string parameterName, InterbaseDbType interbaseType, int size)
	{
		return Add(new InterbaseParameter(parameterName, interbaseType, size));
	}

	public InterbaseParameter Add(string parameterName, InterbaseDbType interbaseType, int size, string sourceColumn)
	{
		return Add(new InterbaseParameter(parameterName, interbaseType, size, sourceColumn));
	}

	public InterbaseParameter Add(InterbaseParameter value)
	{
		EnsureInterbaseParameterAddOrInsert(value);

		AttachParameter(value);
		_parameters.Add(value);
		return value;
	}

	public override int Add(object value)
	{
		EnsureInterbaseParameterType(value);

		return IndexOf(Add((InterbaseParameter)value));
	}

	public bool Contains(InterbaseParameter value)
	{
		return _parameters.Contains(value);
	}

	public override bool Contains(object value)
	{
		EnsureInterbaseParameterType(value);

		return Contains((InterbaseParameter)value);
	}

	public override bool Contains(string parameterName)
	{
		return IndexOf(parameterName) != -1;
	}

	public int IndexOf(InterbaseParameter value)
	{
		return _parameters.IndexOf(value);
	}

	public override int IndexOf(object value)
	{
		EnsureInterbaseParameterType(value);

		return IndexOf((InterbaseParameter)value);
	}

	public override int IndexOf(string parameterName)
	{
		return IndexOf(parameterName, -1);
	}

	internal int IndexOf(string parameterName, int luckyIndex)
	{
		var isNonAsciiParameterName = InterbaseParameter.IsNonAsciiParameterName(parameterName);
		var usedComparison = isNonAsciiParameterName || HasParameterWithNonAsciiName
			? StringComparison.CurrentCultureIgnoreCase
			: StringComparison.OrdinalIgnoreCase;
		var normalizedParameterName = InterbaseParameter.NormalizeParameterName(parameterName);
		if (luckyIndex != -1 && luckyIndex < _parameters.Count)
		{
			if (_parameters[luckyIndex].InternalParameterName.Equals(normalizedParameterName, usedComparison))
			{
				return luckyIndex;
			}
		}

		return _parameters.FindIndex(x => x.InternalParameterName.Equals(normalizedParameterName, usedComparison));
	}

	public void Insert(int index, InterbaseParameter value)
	{
		EnsureInterbaseParameterAddOrInsert(value);

		AttachParameter(value);
		_parameters.Insert(index, value);
	}

	public override void Insert(int index, object value)
	{
		EnsureInterbaseParameterType(value);

		Insert(index, (InterbaseParameter)value);
	}

	public void Remove(InterbaseParameter value)
	{
		if (!_parameters.Remove(value))
		{
			throw new ArgumentException("The parameter does not exist in the collection.");
		}

		ReleaseParameter(value);
	}

	public override void Remove(object value)
	{
		EnsureInterbaseParameterType(value);

		Remove((InterbaseParameter)value);
	}

	public override void RemoveAt(int index)
	{
		if (index < 0 || index > Count)
		{
			throw new IndexOutOfRangeException("The specified index does not exist.");
		}

		var parameter = this[index];
		_parameters.RemoveAt(index);
		ReleaseParameter(parameter);
	}

	public override void RemoveAt(string parameterName)
	{
		RemoveAt(IndexOf(parameterName));
	}

	public void CopyTo(InterbaseParameter[] array, int index)
	{
		_parameters.CopyTo(array, index);
	}

	public override void CopyTo(Array array, int index)
	{
		((IList)_parameters).CopyTo(array, index);
	}

	public override void Clear()
	{
		var parameters = _parameters.ToArray();
		_parameters.Clear();
		foreach (var parameter in parameters)
		{
			ReleaseParameter(parameter);
		}
	}

	public override IEnumerator GetEnumerator()
	{
		return _parameters.GetEnumerator();
	}

	#endregion

	#region DbParameterCollection overriden protected methods

	protected override DbParameter GetParameter(string parameterName)
	{
		return this[parameterName];
	}

	protected override DbParameter GetParameter(int index)
	{
		return this[index];
	}

	protected override void SetParameter(int index, DbParameter value)
	{
		this[index] = (InterbaseParameter)value;
	}

	protected override void SetParameter(string parameterName, DbParameter value)
	{
		this[parameterName] = (InterbaseParameter)value;
	}

	#endregion

	#region Internal Methods

	internal void ParameterNameChanged()
	{
		_hasParameterWithNonAsciiName = null;
	}

	#endregion

	#region Private Methods

	private string GenerateParameterName()
	{
		var index = Count + 1;
		while (true)
		{
			var name = "Parameter" + index.ToString(CultureInfo.InvariantCulture);
			if (!Contains(name))
			{
				return name;
			}
			index++;
		}
	}

	private void EnsureInterbaseParameterType(object value)
	{
		if (!(value is InterbaseParameter))
		{
			throw new InvalidCastException($"The parameter passed was not a {nameof(InterbaseParameter)}.");
		}
	}

	private void EnsureInterbaseParameterAddOrInsert(InterbaseParameter value)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		if (value.Parent != null)
		{
			throw new ArgumentException($"The {nameof(InterbaseParameter)} specified in the value parameter is already added to this or another {nameof(InterbaseParameterCollection)}.");
		}
		if (value.ParameterName == null || value.ParameterName.Length == 0)
		{
			value.ParameterName = GenerateParameterName();
		}
		else
		{
			if (Contains(value.ParameterName))
			{
				throw new ArgumentException($"{nameof(InterbaseParameterCollection)} already contains {nameof(InterbaseParameter)} with {nameof(InterbaseParameter.ParameterName)} '{value.ParameterName}'.");
			}
		}
	}

	private void AttachParameter(InterbaseParameter parameter)
	{
		parameter.Parent = this;
	}

	private void ReleaseParameter(InterbaseParameter parameter)
	{
		parameter.Parent = null;
	}

	#endregion


	#region customizations

	public IList<InterbaseParameter> ToList() => new List<InterbaseParameter>(_parameters);

	#endregion
}
