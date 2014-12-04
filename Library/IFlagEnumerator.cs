using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace BitFn.CoreUtilities.EnumHelpers
{
	[ContractClass(typeof(FlagEnumeratorContract<>))]
	public interface IFlagEnumerator<T> where T : struct, IComparable, IFormattable, IConvertible
	{
		IEnumerable<T> Enumerate(T value, FlagEnumerationBehavior behavior = FlagEnumerationBehavior.All);
	}

	[ContractClassFor(typeof(IFlagEnumerator<>))]
	internal abstract class FlagEnumeratorContract<T> : IFlagEnumerator<T> where T : struct, IComparable, IFormattable, IConvertible
	{
		public IEnumerable<T> Enumerate(T value, FlagEnumerationBehavior behavior = FlagEnumerationBehavior.All)
		{
			Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

			throw new NotImplementedException();
		}
	}
}
