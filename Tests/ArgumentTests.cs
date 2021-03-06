﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Extensions;

// ReSharper disable CheckNamespace
public class ArgumentTests {
    [Theory]
    [PropertyData("NullVariants")]
    public void NotNullVariant_ThrowsArgumentNullException_WithCorrectMessage_WhenValueIsNull(Expression<Action<string>> validateExpression) {
        var validate = validateExpression.Compile();
        var exception = Assert.Throws<ArgumentNullException>(() => validate("x"));
        Assert.Equal(new ArgumentNullException("x").Message, exception.Message);
    }

    [Theory]
    [PropertyData("NullVariants")]
    public void NotNullVariant_ThrowsArgumentNullException_WithCorrectParamName_WhenValueIsNull(Expression<Action<string>> validateExpression) {
        var validate = validateExpression.Compile();
        var exception = Assert.Throws<ArgumentNullException>(() => validate("x"));
        Assert.Equal("x", exception.ParamName);
    }

    [Theory]
    [PropertyData("EmptyVariants")]
    public void NotNullOrEmptyVariant_ThrowsArgumentException_WithCorrectParamName_WhenValueIsEmpty(Expression<Action<string>> validateExpression) {
        var validate = validateExpression.Compile();
        var exception = Assert.Throws<ArgumentException>(() => validate("x"));
        Assert.Equal("x", exception.ParamName);
    }

    [Theory]
    [PropertyData("IncorrectTypeVariants")]
    public void CastVariant_ThrowsArgumentException_WithCorrectParamName_WhenValueHasIncorrectType(Expression<Action<string>> validateExpression) {
        var validate = validateExpression.Compile();
        var exception = Assert.Throws<ArgumentException>(() => validate("x"));
        Assert.Equal("x", exception.ParamName);
    }

    [Theory]
    [PropertyData("OutOfRangeVariants")]
    public void RangeVariant_ThrowsArgumentOutOfRangeException_WithCorrectParamName_WhenValueIsOutOfRange(Expression<Action<string>> validateExpression) {
        var validate = validateExpression.Compile();
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => validate("x"));
        Assert.Equal("x", exception.ParamName);
    }

    [Theory]
    [PropertyData("NotMatchVariants")]
    public void MatchVariant_ThrowsArgumentException_WithCorrectParamName_WhenValueNotMatch(Expression<Action<string>> validateExpression)
    {
        var validate = validateExpression.Compile();
        var exception = Assert.Throws<ArgumentException>(() => validate("x"));
        Assert.Equal("x", exception.ParamName);
    }
  
    [Theory]
    [PropertyData("SuccessVariants")]
    public void AnyVariant_ReturnsArgumentValue_WhenSuccessful<T>(T argument, Expression<Func<T, T>> validateExpression) {
        var validate = validateExpression.Compile();
        var result = validate(argument);

        if (typeof(T).IsValueType) {
            Assert.Equal(argument, result);
        }
        else {
            Assert.Same(argument, result);
        }
    }

    public static IEnumerable<object[]> NullVariants {
        get {
            yield return SimpleDataRow(name => Argument.NotNull(name, (object)null));
            yield return SimpleDataRow(name => Argument.NotNull(name, (int?)null));
            yield return SimpleDataRow(name => Argument.NotNullOrEmpty(name, null));
            yield return SimpleDataRow(name => Argument.NotNullOrEmpty(name, (object[])null));
            yield return SimpleDataRow(name => Argument.NotNullOrEmpty(name, (List<object>)null));
            yield return SimpleDataRow(name => Argument.NotNullAndCast<object>(name, null));
        }
    }

    public static IEnumerable<object[]> EmptyVariants {
        get {
            yield return SimpleDataRow(name => Argument.NotNullOrEmpty(name, ""));
            yield return SimpleDataRow(name => Argument.NotNullOrEmpty(name, new object[0]));
            yield return SimpleDataRow(name => Argument.NotNullOrEmpty(name, new List<object>()));
            yield return SimpleDataRow(name => Argument.NotNullOrEmpty(name, new List<int>()));
            yield return SimpleDataRow(name => Argument.NotNullOrEmpty(name, (IReadOnlyCollection<int>)new List<int>()));
            yield return SimpleDataRow(name => Argument.NotNullOrEmpty(name, ImmutableList.Create<int>()));
        }
    }

    public static IEnumerable<object[]> IncorrectTypeVariants {
        get {
            yield return SimpleDataRow(name => Argument.Cast<string>(name, new object()));
        }
    }

    public static IEnumerable<object[]> OutOfRangeVariants {
        get {
            yield return SimpleDataRow(name => Argument.PositiveNonZero(name, -1));
            yield return SimpleDataRow(name => Argument.PositiveNonZero(name, int.MinValue));
            yield return SimpleDataRow(name => Argument.PositiveNonZero(name, 0));
            yield return SimpleDataRow(name => Argument.PositiveOrZero(name, -1));
            yield return SimpleDataRow(name => Argument.PositiveOrZero(name, int.MinValue));

            yield return SimpleDataRow(name => Argument.NotOutOfRange(name, 0, 1, int.MaxValue));
            yield return SimpleDataRow(name => Argument.NotOutOfRange(name, 11, int.MinValue, 10));
            yield return SimpleDataRow(name => Argument.NotOutOfRange(name, Guid.Empty, Guid.NewGuid(), Guid.NewGuid()));
            yield return SimpleDataRow(name => Argument.NotOutOfRange(name, "", "First", "Last", String.CompareOrdinal));

            yield return SimpleDataRow(name => Argument.NotLess(name, 0, 1));
            yield return SimpleDataRow(name => Argument.NotLess(name, Guid.Empty, Guid.NewGuid()));
            yield return SimpleDataRow(name => Argument.NotLess(name, "", "First", String.CompareOrdinal));

            yield return SimpleDataRow(name => Argument.NotGreater(name, 11, 10));
            yield return SimpleDataRow(name => Argument.NotGreater(name, Guid.NewGuid(), Guid.Empty));
            yield return SimpleDataRow(name => Argument.NotGreater(name, "Last", "First", String.CompareOrdinal));

        }
    }

    public static IEnumerable<object[]> NotMatchVariants
    {
        get
        {
            yield return SimpleDataRow(name => Argument.Match(name, "A123Z", "^a[0-9].z$", RegexOptions.None));
        }
    }

    public static IEnumerable<object[]> SuccessVariants {
        get {
            yield return SuccessDataRow(new object(),  value => Argument.NotNull("x", value));
            yield return SuccessDataRow("abc",         value => Argument.NotNullOrEmpty("x", value));
            yield return SuccessDataRow(new object[1], value => Argument.NotNullOrEmpty("x", value));
            yield return SuccessDataRow(new List<object> { new object() }, value => Argument.NotNullOrEmpty("x", value));

            yield return SuccessDataRow("abc",         value => Argument.Cast<string>("x", value));
            yield return SuccessDataRow("abc",         value => Argument.NotNullAndCast<string>("x", value));
            
            yield return SuccessDataRow(1,             value => Argument.PositiveNonZero("x", value));
            yield return SuccessDataRow(int.MaxValue,  value => Argument.PositiveNonZero("x", value));
            yield return SuccessDataRow(1,             value => Argument.PositiveOrZero("x", value));
            yield return SuccessDataRow(int.MaxValue,  value => Argument.PositiveOrZero("x", value));
            yield return SuccessDataRow(0,             value => Argument.PositiveOrZero("x", value));

            yield return SuccessDataRow(1, value => Argument.NotOutOfRange("x", value, 0, 10));
            yield return SuccessDataRow("bbb", value => Argument.NotOutOfRange("x", value, "aaa", "ccc", String.CompareOrdinal));
            yield return SuccessDataRow(0, value => Argument.NotLess("x", value, 0));
            yield return SuccessDataRow(100, value => Argument.NotGreater("x", value, 100));

            yield return SuccessDataRow("a123z", value => Argument.Match("x", value, "a[0-9]*z", RegexOptions.IgnoreCase));

        }
    }

    private static object[] SimpleDataRow(Expression<Action<string>> action) {
        return new object[] { action };
    }

    private static object[] SuccessDataRow<T>(T argument, Expression<Func<T, T>> validate) {
        return new object[] { argument, validate };
    }
}