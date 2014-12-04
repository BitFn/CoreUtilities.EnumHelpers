using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BitFn.CoreUtilities.EnumHelpers
{
	public class FlagEnumeratorUInt64<T> : IFlagEnumerator<T> where T : struct, IComparable, IFormattable, IConvertible
	{
		private readonly ulong _maskOutOfRange;


		public FlagEnumeratorUInt64()
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("Type parameter must be an Enum type.");
			}
			if (typeof(T).GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0)
			{
				throw new ArgumentException("Enum type must have the Flags attribute.");
			}
			try
			{
				_maskOutOfRange = ~Enum.GetValues(typeof(T)).Cast<T>().Aggregate((ulong)0, (mask, _) => mask | Convert.ToUInt64(_));
			}
			catch (OverflowException ex)
			{
				throw new ArgumentException("Enum type members out of range.", ex);
			}
		}

		public IEnumerable<T> Enumerate(T value, FlagEnumerationBehavior behavior = FlagEnumerationBehavior.FullyFactorized)
		{
			ulong bitmask;
			try
			{
				bitmask = Convert.ToUInt64(value);
			}
			catch (OverflowException ex)
			{
				throw new ArgumentOutOfRangeException("value", ex);
			}
			if ((bitmask & _maskOutOfRange) != 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}

			if (Equals(value, default(T)) && EnumInfo.Get(default(T)) != null)
			{
				return new[] {default(T)};
			}

			var info = EnumInfo.Get(value);
			if (info != null && (behavior == FlagEnumerationBehavior.ExactOrFullyFactorized ||
				behavior == FlagEnumerationBehavior.ExactOrFullyAggregated))
			{
				return new[] {info.Value};
			}

			var factors = info != null
				? info.Factors.Concat(new[] {info})
				: GetAllFlags(bitmask).Select(EnumInfo.Get);
			ISet<EnumInfo> result = new HashSet<EnumInfo>(factors);
			switch (behavior)
			{
				case FlagEnumerationBehavior.All:
					break;
				case FlagEnumerationBehavior.FullyFactorized:
				case FlagEnumerationBehavior.ExactOrFullyFactorized:
					result = new HashSet<EnumInfo>(result.Where(_ => _.Irreducible));
					break;
				case FlagEnumerationBehavior.FullyAggregated:
				case FlagEnumerationBehavior.ExactOrFullyAggregated:
					foreach (var factor in result.ToList().SelectMany(_ => _.Factors))
					{
						result.Remove(factor);
					}
					break;
				default:
					throw new ArgumentOutOfRangeException("behavior");
			}
			return result.OrderBy(_ => _.BitCount).ThenBy(_ => _.BitMask).Select(_ => _.Value);
		}

		private static IEnumerable<T> GetAllFlags(ulong bitmask, bool exclusive = false)
		{
			foreach (var value in Enum.GetValues(typeof(T)))
			{
				var valuemask = Convert.ToUInt64(value);
				if (valuemask == default(ulong))
				{
					continue;
				}
				if (exclusive && (valuemask == bitmask))
				{
					continue;
				}
				if ((bitmask & valuemask) == valuemask)
				{
					yield return (T)value;
				}
			}
		}

		[DebuggerDisplay("{DebuggerDisplay,nq}")]
		private sealed class EnumInfo
		{
			// ReSharper disable StaticFieldInGenericType
			private static readonly IDictionary<T, EnumInfo> Cache = new Dictionary<T, EnumInfo>();
			private static readonly ISet<T> ValueSet;
			// ReSharper restore StaticFieldInGenericType

			private readonly string _name;

			static EnumInfo()
			{
				ValueSet = new HashSet<T>(Enum.GetValues(typeof(T)).Cast<T>());
			}

			public static EnumInfo Get(T value)
			{
				EnumInfo result;
				if (!Cache.TryGetValue(value, out result))
				{
					if (!ValueSet.Contains(value))
					{
						return null;
					}
					result = new EnumInfo(value);
					Cache.Add(value, result);
				}
				return result;
			}

			private EnumInfo(T value)
			{
				Value = value;
				_name = Enum.GetName(typeof(T), value);
				BitMask = Convert.ToUInt64(value);
				BitCount = CountBits(BitMask);
				if (BitCount > 1)
				{
					Factors = GetAllFlags(BitMask, true).Select(Get).ToList();
				}
				else
				{
					Factors = new EnumInfo[0];
				}
			}

			public T Value { get; private set; }
			public int BitCount { get; private set; }
			public ulong BitMask { get; private set; }
			public IList<EnumInfo> Factors { get; private set; }

			public bool Irreducible
			{
				get { return Factors.Count == 0; }
			}

			private static int CountBits(ulong value)
			{
				var count = 0;
				while (value != 0)
				{
					count++;
					value &= value - 1;
				}
				return count;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
				{
					return false;
				}
				if (ReferenceEquals(this, obj))
				{
					return true;
				}
				if (obj.GetType() != GetType())
				{
					return false;
				}
				return Equals((EnumInfo)obj);
			}

			private bool Equals(EnumInfo other)
			{
				Contract.Requires(other != null);

				return Value.Equals(other.Value);
			}

			public override int GetHashCode()
			{
				return Value.GetHashCode();
			}

			public static bool operator ==(EnumInfo left, EnumInfo right)
			{
				return Equals(left, right);
			}

			public static bool operator !=(EnumInfo left, EnumInfo right)
			{
				return !Equals(left, right);
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private string DebuggerDisplay
			{
				get { return _name ?? ToString(); }
			}

			[ContractInvariantMethod]
			private void ObjectInvariant()
			{
				Contract.Invariant(Factors != null);
			}
		}
	}
}
