using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Xunit.Extensions;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace BitFn.CoreUtilities.EnumHelpers.Tests.FlagEnumeratorUInt64
{
	public class Enumerate
	{
		[Theory]
		[InlineData(FiveBitEnum._16_4_2, new[] {FiveBitEnum._16, FiveBitEnum._4, FiveBitEnum._2})]
		[InlineData(FiveBitEnum._16_4_2 | FiveBitEnum._8, new[] {FiveBitEnum._16, FiveBitEnum._8, FiveBitEnum._4, FiveBitEnum._2})]
		public void WhenEnumeratingAll_ShouldIncludeIrreducibleFactors(FiveBitEnum input, FiveBitEnum[] expected)
		{
			// Arrange
			var enumerator = new FlagEnumeratorUInt64<FiveBitEnum>();

			// Act
			var actual = enumerator.Enumerate(input, FlagEnumerationBehavior.All).ToList();

			// Asset
			CollectionAssert.IsSubsetOf(expected, actual);
		}

		[Theory]
		[InlineData(FiveBitEnum._16_4_2, new[] {FiveBitEnum._16_4, FiveBitEnum._16_2, FiveBitEnum._4_2})]
		[InlineData(FiveBitEnum._16_4_2 | FiveBitEnum._8, new[] {FiveBitEnum._16_8_4, FiveBitEnum._16_8_2, FiveBitEnum._16_4_2, FiveBitEnum._8_4_2})]
		public void WhenEnumeratingAll_ShouldIncludereducibleFactors(FiveBitEnum input, FiveBitEnum[] expected)
		{
			// Arrange
			var enumerator = new FlagEnumeratorUInt64<FiveBitEnum>();

			// Act
			var actual = enumerator.Enumerate(input, FlagEnumerationBehavior.All).ToList();

			// Asset
			CollectionAssert.IsSubsetOf(expected, actual);
		}

		[Theory]
		[InlineData(FiveBitEnum._2)]
		[InlineData(FiveBitEnum._16_2)]
		[InlineData(FiveBitEnum._16_4_2)]
		[InlineData(FiveBitEnum._16_8_4_2_1)]
		public void WhenEnumeratingAll_ShouldIncludeSelf(FiveBitEnum input)
		{
			// Arrange
			var enumerator = new FlagEnumeratorUInt64<FiveBitEnum>();

			// Act
			var actual = enumerator.Enumerate(input, FlagEnumerationBehavior.All).ToList();

			// Asset
			CollectionAssert.Contains(actual, input);
		}

		[Theory]
		[InlineData(FiveBitEnum._2, FlagEnumerationBehavior.All)]
		[InlineData(FiveBitEnum._16_2, FlagEnumerationBehavior.ExactOrFullyAggregated)]
		[InlineData(FiveBitEnum._16_4_2, FlagEnumerationBehavior.ExactOrFullyFactorized)]
		[InlineData(FiveBitEnum._16_4_2 | FiveBitEnum._8, FlagEnumerationBehavior.FullyAggregated)]
		[InlineData(FiveBitEnum._16_8_4_2_1, FlagEnumerationBehavior.FullyFactorized)]
		public void WhenEnumerating_ShouldNotIncludeDefault(FiveBitEnum input, FlagEnumerationBehavior behavior)
		{
			// Arrange
			var enumerator = new FlagEnumeratorUInt64<FiveBitEnum>();

			// Act
			var actual = enumerator.Enumerate(input, behavior).ToList();

			// Asset
			CollectionAssert.DoesNotContain(actual, default(FiveBitEnum));
		}

		[Theory]
		[InlineData(FlagEnumerationBehavior.All)]
		[InlineData(FlagEnumerationBehavior.ExactOrFullyAggregated)]
		[InlineData(FlagEnumerationBehavior.ExactOrFullyFactorized)]
		[InlineData(FlagEnumerationBehavior.FullyAggregated)]
		[InlineData(FlagEnumerationBehavior.FullyFactorized)]
		public void WhenGivenDefault_ShouldReturnDefaultRegardlessOfBehavior(FlagEnumerationBehavior behavior)
		{
			// Arrange
			const FiveBitEnum input = FiveBitEnum._0;
			var expected = new[] {FiveBitEnum._0};
			var enumerator = new FlagEnumeratorUInt64<FiveBitEnum>();

			// Act
			var actual = enumerator.Enumerate(input, behavior).ToList();

			// Asset
			CollectionAssert.AreEquivalent(expected, actual);
		}

		[Theory]
		[InlineData(FiveBitEnum._1, FlagEnumerationBehavior.All)]
		[InlineData(FiveBitEnum._2, FlagEnumerationBehavior.ExactOrFullyAggregated)]
		[InlineData(FiveBitEnum._4, FlagEnumerationBehavior.ExactOrFullyFactorized)]
		[InlineData(FiveBitEnum._8, FlagEnumerationBehavior.FullyAggregated)]
		[InlineData(FiveBitEnum._16, FlagEnumerationBehavior.FullyFactorized)]
		public void WhenGivenSingleIrreducibleValue_ShouldReturnRegardlessOfBehavior(FiveBitEnum input,
			FlagEnumerationBehavior behavior)
		{
			// Arrange
			var expected = new[] {input};
			var enumerator = new FlagEnumeratorUInt64<FiveBitEnum>();

			// Act
			var actual = enumerator.Enumerate(input).ToList();

			// Asset
			CollectionAssert.AreEquivalent(expected, actual);
		}

		[Theory]
		[InlineData(FiveBitEnum._2)]
		[InlineData(FiveBitEnum._16_2)]
		[InlineData(FiveBitEnum._16_4_2)]
		[InlineData(FiveBitEnum._16_8_4_2_1)]
		public void WhenGivenSingleValue_ShouldMatchExactly(FiveBitEnum input)
		{
			// Arrange
			var expected = new[] {input};
			var enumerator = new FlagEnumeratorUInt64<FiveBitEnum>();

			// Act
			var actual = enumerator.Enumerate(input, FlagEnumerationBehavior.ExactOrFullyFactorized).ToList();

			// Asset
			CollectionAssert.AreEquivalent(expected, actual);
		}

		[Theory]
		[InlineData(-1234)]
		[InlineData(-1)]
		[InlineData(1234)]
		public void WhenGivenUnknownValue_ShouldThrowArgumentOutOfRangeException(int value)
		{
			// Arrange
			Exception exception = null;
			var input = (FiveBitEnum)value;
			var enumerator = new FlagEnumeratorUInt64<FiveBitEnum>();

			// Act
			try
			{
				var actual = enumerator.Enumerate(input).ToList();
			}
			catch (Exception ex)
			{
				exception = ex;
			}

			// Asset
			Assert.IsNotNull(exception);
			Assert.IsInstanceOfType(exception, typeof(ArgumentOutOfRangeException));
		}

		[Fact]
		public void WhenInitializedWithNonEnum_ShouldThrowException()
		{
			// Arrange
			Exception exception = null;

			// Act
			try
			{
				var enumerator = new FlagEnumeratorUInt64<int>();
			}
			catch (Exception ex)
			{
				exception = ex;
			}

			// Asset
			Assert.IsNotNull(exception);
		}

		[Fact]
		public void WhenInitializedWithOutOfRangeEnum_ShouldThrowException()
		{
			// Arrange
			Exception exception = null;

			// Act
			try
			{
				var enumerator = new FlagEnumeratorUInt64<EnumWithNegatives>();
			}
			catch (Exception ex)
			{
				exception = ex;
			}

			// Asset
			Assert.IsNotNull(exception);
		}

		[Theory]
		[InlineData(FiveBitEnum._16_4_2 | FiveBitEnum._8, new[] {FiveBitEnum._16_8_4, FiveBitEnum._16_8_2, FiveBitEnum._16_4_2, FiveBitEnum._8_4_2})]
		public void WhenNoExactMatch_ShouldFullyAggregate(FiveBitEnum input, FiveBitEnum[] expected)
		{
			// Arrange
			var enumerator = new FlagEnumeratorUInt64<FiveBitEnum>();

			// Act
			var actual = enumerator.Enumerate(input, FlagEnumerationBehavior.ExactOrFullyAggregated).ToList();

			// Asset
			CollectionAssert.AreEquivalent(expected, actual);
		}

		[Theory]
		[InlineData(FiveBitEnum._16_4_2 | FiveBitEnum._8, new[] {FiveBitEnum._16, FiveBitEnum._8, FiveBitEnum._4, FiveBitEnum._2})]
		public void WhenNoExactMatch_ShouldFullyFactorize(FiveBitEnum input, FiveBitEnum[] expected)
		{
			// Arrange
			var enumerator = new FlagEnumeratorUInt64<FiveBitEnum>();

			// Act
			var actual = enumerator.Enumerate(input, FlagEnumerationBehavior.ExactOrFullyFactorized).ToList();

			// Asset
			CollectionAssert.AreEquivalent(expected, actual);
		}

		[Theory]
		[InlineData(FiveBitEnum._2, new[] {FiveBitEnum._2})]
		[InlineData(FiveBitEnum._16_2, new[] {FiveBitEnum._16, FiveBitEnum._2})]
		[InlineData(FiveBitEnum._16_4_2, new[] {FiveBitEnum._16, FiveBitEnum._4, FiveBitEnum._2})]
		[InlineData(FiveBitEnum._16_4_2 | FiveBitEnum._8, new[] {FiveBitEnum._16, FiveBitEnum._8, FiveBitEnum._4, FiveBitEnum._2})]
		[InlineData(FiveBitEnum._16_8_4_2_1, new[] {FiveBitEnum._16, FiveBitEnum._8, FiveBitEnum._4, FiveBitEnum._2, FiveBitEnum._1})]
		public void WhenUsingDefaultBehavior_ShouldReturnFullyFactorized(FiveBitEnum input, FiveBitEnum[] expected)
		{
			// Arrange
			var enumerator = new FlagEnumeratorUInt64<FiveBitEnum>();

			// Act
			var actual = enumerator.Enumerate(input).ToList();

			// Asset
			CollectionAssert.AreEquivalent(expected, actual);
		}
	}
}
