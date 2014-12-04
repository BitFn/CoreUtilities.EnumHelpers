using System;
using System.Collections.Generic;

namespace BitFn.CoreUtilities.EnumHelpers
{
	public interface IFlagEnumerator<T> where T : struct, IComparable, IFormattable, IConvertible
	{
		IEnumerable<T> Enumerate(T value, FlagEnumerationBehavior behavior = FlagEnumerationBehavior.All);
	}
}