using System;

namespace BitFn.CoreUtilities.EnumHelpers.Tests
{
	[Flags]
	public enum EnumWithNegatives
	{
		One = 1,
		NegativeOne = -1,
	}
}
