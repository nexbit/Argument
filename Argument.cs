using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

// ReSharper disable CheckNamespace
/// <summary>
/// Provides methods for verification of argument preconditions.
/// </summary>
public static partial class Argument {
// ReSharper restore CheckNamespace   
    /// <summary>
    /// Verifies that a given argument value is not <c>null</c> and returns the value provided.
    /// </summary>
    /// <typeparam name="T">Type of the <paramref name="name" />.</typeparam>
    /// <param name="name">Argument name.</param>
    /// <param name="value">Argument value.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
    /// <returns><paramref name="value"/> if it is not <c>null</c>.</returns>
    [ContractArgumentValidator]
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static T NotNull<T>(string name, T value) 
        where T : class
    {
        if (value == null)
            throw new ArgumentNullException(name);
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Verifies that a given argument value is not <c>null</c> and returns the value provided.
    /// </summary>
    /// <typeparam name="T">Type of the <paramref name="name" />.</typeparam>
    /// <param name="name">Argument name.</param>
    /// <param name="value">Argument value.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
    /// <returns><paramref name="value"/> if it is not <c>null</c>.</returns>
    [ContractArgumentValidator]
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static T NotNull<T>(string name, T? value) 
        where T : struct
    {
        if (value == null)
            throw new ArgumentNullException(name);
        Contract.EndContractBlock();

        return value.Value;
    }

    /// <summary>
    /// Verifies that a given argument value is not <c>null</c> or empty and returns the value provided.
    /// </summary>
    /// <param name="name">Argument name.</param>
    /// <param name="value">Argument value.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is empty.</exception>
    /// <returns><paramref name="value"/> if it is not <c>null</c> or empty.</returns>
    [ContractArgumentValidator]
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static string NotNullOrEmpty(string name, string value)
    {
        NotNull(name, value);
        if (value.Length == 0)
            throw NewArgumentEmptyException(name);
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Verifies that a given argument value is not <c>null</c> or empty and returns the value provided.
    /// </summary>
    /// <param name="name">Argument name.</param>
    /// <param name="value">Argument value.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is empty.</exception>
    /// <returns><paramref name="value"/> if it is not <c>null</c> or empty.</returns>
    [ContractArgumentValidator]
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static T[] NotNullOrEmpty<T>(string name, T[] value)
    {
        NotNull(name, value);
        if (value.Length == 0)
            throw NewArgumentEmptyException(name);
        Contract.EndContractBlock();

        return value;
    }
    
    /// <summary>
    /// Verifies that a given argument value is not <c>null</c> or empty and returns the value provided.
    /// </summary>
    /// <param name="name">Argument name.</param>
    /// <param name="value">Argument value.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is empty.</exception>
    /// <returns><paramref name="value"/> if it is not <c>null</c> or empty.</returns>
    [ContractArgumentValidator]
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static TCollection NotNullOrEmpty<TCollection>(string name, TCollection value) 
        where TCollection : class, IEnumerable
    {
        NotNull(name, value);
        var enumerator = value.GetEnumerator();
        try {
            if (!enumerator.MoveNext())
                throw NewArgumentEmptyException(name);
        }
        finally {
            var disposable = enumerator as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
        Contract.EndContractBlock();

        return value;
    }

    private const string PotentialDoubleEnumeration = "Using NotNullOrEmpty with plain IEnumerable may cause double enumeration. Please use a collection instead.";

    /// <summary>
    /// (DO NOT USE) Ensures that NotNullOrEmpty can not be used with plain <see cref="IEnumerable"/>,
    /// as this may cause double enumeration.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(PotentialDoubleEnumeration, true)]
    // ReSharper disable UnusedParameter.Global
    public static void NotNullOrEmpty(string name, IEnumerable value) 
    {
    // ReSharper restore UnusedParameter.Global
        throw new Exception(PotentialDoubleEnumeration);
    }

    /// <summary> 
    /// (DO NOT USE) Ensures that NotNullOrEmpty can not be used with plain <see cref="IEnumerable{T}" />,
    /// as this may cause double enumeration.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(PotentialDoubleEnumeration, true)]
    // ReSharper disable UnusedParameter.Global
    public static void NotNullOrEmpty<T>(string name, IEnumerable<T> value) 
    {
        // ReSharper restore UnusedParameter.Global
        throw new Exception(PotentialDoubleEnumeration);
    }

    
    [Pure]
    private static Exception NewArgumentEmptyException(string name) 
    {
        return new ArgumentException("Value can not be empty.", name);
    }

    /// <summary>
    /// Casts a given argument into a given type if possible.
    /// </summary>
    /// <typeparam name="T">Type to cast <paramref name="value"/> into.</typeparam>
    /// <param name="name">Argument name.</param>
    /// <param name="value">Argument value.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> can not be cast into type <typeparamref name="T"/>.</exception>
    /// <returns><paramref name="value"/> cast into <typeparamref name="T"/>.</returns>
    [ContractArgumentValidator]
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static T Cast<T>(string name, object value)
    {
        if (!(value is T))
            throw new ArgumentException(string.Format("The value \"{0}\" isn't of type \"{1}\".", value, typeof (T)), name);
        Contract.EndContractBlock();

        return (T)value;
    }

    /// <summary>
    /// Verfies that a given argument is not null and casts it into a given type if possible.
    /// </summary>
    /// <typeparam name="T">Type to cast <paramref name="value"/> into.</typeparam>
    /// <param name="name">Argument name.</param>
    /// <param name="value">Argument value.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> can not be cast into type <typeparamref name="T"/>.</exception>
    /// <returns><paramref name="value"/> cast into <typeparamref name="T"/>.</returns>
    [ContractArgumentValidator]
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static T NotNullAndCast<T>(string name, object value)
    {
        NotNull(name, value);
        return Cast<T>(name, value);
    }

    /// <summary>
    /// Verifies that a given argument value is greater than or equal to zero and returns the value provided.
    /// </summary>
    /// <param name="name">Argument name.</param>
    /// <param name="value">Argument value.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than zero.</exception>
    /// <returns><paramref name="value"/> if it is greater than or equal to zero.</returns>
    [ContractArgumentValidator]
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static int PositiveOrZero(string name, int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(name, 
                string.Format("Argument {0} must be positive or zero. Actual value was {1}.", name, value));
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Verifies that a given argument value is greater than zero and returns the value provided.
    /// </summary>
    /// <param name="name">Argument name.</param>
    /// <param name="value">Argument value.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than or equal to zero.</exception>
    /// <returns><paramref name="value"/> if it is greater than zero.</returns>
    [ContractArgumentValidator]
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static int PositiveNonZero(string name, int value)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(name, 
                string.Format("Argument {0} must be positive and not zero. Actual value was {1}.", name, value));
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Provides an extensibility point that can be used by custom argument validation extension methods.
    /// </summary>
    /// <remarks>Always returns <c>null</c>, extension methods for <see cref="Extensible"/> should ignore <c>this</c> value.</remarks>
    [DebuggerNonUserCode]
    public static Extensible Ex
    {
        get { return null; }
    }

    /// <summary>
    /// Provides an extensibility point that can be used by custom argument validation extension methods.
    /// </summary>
    /// <seealso cref="Argument.Ex"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class Extensible
    {
        #pragma warning disable 1591
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerNonUserCode, DebuggerStepThrough]
        public override bool Equals(object obj) { throw new NotSupportedException(); }

        // ReSharper disable once NonReadonlyFieldInGetHashCode
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerNonUserCode, DebuggerStepThrough]
        public override int GetHashCode() { throw new NotSupportedException(); }

        /// <summary>
        /// Calls Object.GetType method.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerNonUserCode, DebuggerStepThrough]
        public new Type GetType() { return base.GetType(); }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerNonUserCode, DebuggerStepThrough]
        public override String ToString() { throw new NotSupportedException(); }
        #pragma warning disable 1591
    }

    #region Ported (and adapted) from Catel.Core Argument class

    private const string NotOutOfRangeError = "Argument '{0}' should be between {1} and {2}";
    private const string NotLessError = "Argument '{0}' should be greater than or equal {1}";
    private const string NotGreaterError = "Argument '{0}' should be less than {1}";

    #region NotOutOfRange
    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value" /> is out of range.</exception>
    /// <returns><paramref name="value"/> if it is not out of range.</returns>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static byte NotOutOfRange(string name, byte value, byte minimumValue, byte maximumValue)
    {
        // Split the checks to help Code Contract's static code analysis
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value" /> is out of range.</exception>
    /// <returns><paramref name="value"/> if it is not out of range.</returns>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static short NotOutOfRange(string name, short value, short minimumValue, short maximumValue)
    {
        // Split the checks to help Code Contract's static code analysis
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value" /> is out of range.</exception>
    /// <returns><paramref name="value"/> if it is not out of range.</returns>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static int NotOutOfRange(string name, int value, int minimumValue, int maximumValue)
    {
        // Split the checks to help Code Contract's static code analysis
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value" /> is out of range.</exception>
    /// <returns><paramref name="value"/> if it is not out of range.</returns>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static long NotOutOfRange(string name, long value, long minimumValue, long maximumValue)
    {
        // Split the checks to help Code Contract's static code analysis
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value" /> is out of range.</exception>
    /// <returns><paramref name="value"/> if it is not out of range.</returns>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static float NotOutOfRange(string name, float value, float minimumValue, float maximumValue)
    {
        // Split the checks to help Code Contract's static code analysis
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value" /> is out of range.</exception>
    /// <returns><paramref name="value"/> if it is not out of range.</returns>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static double NotOutOfRange(string name, double value, double minimumValue, double maximumValue)
    {
        // Split the checks to help Code Contract's static code analysis
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value" /> is out of range.</exception>
    /// <returns><paramref name="value"/> if it is not out of range.</returns>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static decimal NotOutOfRange(string name, decimal value, decimal minimumValue, decimal maximumValue)
    {
        // Split the checks to help Code Contract's static code analysis
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value" /> is out of range.</exception>
    /// <returns><paramref name="value"/> if it is not out of range.</returns>
    /// <remarks>It compares DateTime values by comparing their number of ticks. 
    /// Before comparing DateTime objects, make sure that the objects represent times in the same time zone.</remarks>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static DateTime NotOutOfRange(string name, DateTime value, DateTime minimumValue, DateTime maximumValue)
    {
        // Split the checks to help Code Contract's static code analysis
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value" /> is out of range.</exception>
    /// <returns><paramref name="value"/> if it is not out of range.</returns>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static char NotOutOfRange(string name, char value, char minimumValue, char maximumValue)
    {
        // Split the checks to help Code Contract's static code analysis
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <remarks>
    /// To use this method with Code Contracts, be sure that the implementation of the
    /// IComparable&lt;T&gt;.CompareTo interface method is decorated with the <see cref="T:System.Diagnostics.Contracts.PureAttribute">Pure</see> attribute (pure
    /// methods do not make any visible state changes).
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static T NotOutOfRange<T>(string name, T value, T minimumValue, T maximumValue)
        where T : IComparable<T>
    {
        // Split the checks to help Code Contract's static code analysis
        if (value.CompareTo(minimumValue) < 0)
            throw new ArgumentOutOfRangeException(name, 
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        if (value.CompareTo(maximumValue) > 0)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        Contract.EndContractBlock();

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <remarks>
    /// Code Contracts always consider Comparison&lt;T&gt; delegates as Pure, so you can use this method without
    /// decorating your code with Pure attribute.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <param name="comparer">The comparer used to compare values.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static T NotOutOfRange<T>(string name, T value, T minimumValue, T maximumValue, Comparison<T> comparer)
    {
        NotNull("comparer", comparer);

        // Split the checks to help Code Contract's static code analysis
        if (comparer(value, minimumValue) < 0)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        if (comparer(value, maximumValue) > 0)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotOutOfRangeError, name, minimumValue, maximumValue));
        Contract.EndContractBlock();

        return value;
    }
    #endregion NotOutOfRange

    #region NotLess

    /// <summary>
    /// Determines whether the specified argument is greater than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static byte NotLess(string name, byte value, byte minimumValue)
    {
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotLessError, name, minimumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is greater than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static short NotLess(string name, short value, short minimumValue)
    {
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotLessError, name, minimumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is greater than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static int NotLess(string name, int value, int minimumValue)
    {
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotLessError, name, minimumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is greater than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static long NotLess(string name, long value, long minimumValue)
    {
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotLessError, name, minimumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is greater than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static float NotLess(string name, float value, float minimumValue)
    {
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotLessError, name, minimumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is greater than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static double NotLess(string name, double value, double minimumValue)
    {
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotLessError, name, minimumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is greater than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static decimal NotLess(string name, decimal value, decimal minimumValue)
    {
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotLessError, name, minimumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is greater than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks>It compares DateTime values by comparing their number of ticks. 
    /// Before comparing DateTime objects, make sure that the objects represent times in the same time zone.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static DateTime NotLess(string name, DateTime value, DateTime minimumValue)
    {
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotLessError, name, minimumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is greater than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static char NotLess(string name, char value, char minimumValue)
    {
        if (value < minimumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotLessError, name, minimumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is greater than a value.
    /// </summary>
    /// <remarks>
    /// To use this method with Code Contracts, be sure that the implementation of the
    /// IComparable&lt;T&gt;.CompareTo interface method is decorated with the <see cref="T:System.Diagnostics.Contracts.PureAttribute">Pure</see> attribute (pure
    /// methods do not make any visible state changes).
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static T NotLess<T>(string name, T value, T minimumValue)
        where T : IComparable<T>
    {
        if (value.CompareTo(minimumValue) < 0)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotLessError, name, minimumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is greater than a value.
    /// </summary>
    /// <remarks>
    /// Code Contracts always consider Comparison&lt;T&gt; delegates as Pure, so you can use this method without
    /// decorating your code with Pure attribute.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="comparer">The comparer used to compare values.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static T NotLess<T>(string name, T value, T minimumValue, Comparison<T> comparer)
    {
        NotNull("comparer", comparer);

        if (comparer(value, minimumValue) < 0)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotLessError, name, minimumValue));
        Contract.EndContractBlock();

        return value;
    }
    #endregion NotLess

    #region NotGreater
    /// <summary>
    /// Determines whether the specified argument is less than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static byte NotGreater(string name, byte value, byte maximumValue)
    {
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotGreaterError, name, maximumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is less than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static short NotGreater(string name, short value, short maximumValue)
    {
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotGreaterError, name, maximumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is less than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static int NotGreater(string name, int value, int maximumValue)
    {
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotGreaterError, name, maximumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is less than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static long NotGreater(string name, long value, long maximumValue)
    {
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotGreaterError, name, maximumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is less than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static float NotGreater(string name, float value, float maximumValue)
    {
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotGreaterError, name, maximumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is less than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static double NotGreater(string name, double value, double maximumValue)
    {
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotGreaterError, name, maximumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is less than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static decimal NotGreater(string name, decimal value, decimal maximumValue)
    {
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotGreaterError, name, maximumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is less than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks>It compares DateTime values by comparing their number of ticks. 
    /// Before comparing DateTime objects, make sure that the objects represent times in the same time zone.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static DateTime NotGreater(string name, DateTime value, DateTime maximumValue)
    {
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotGreaterError, name, maximumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is less than a value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <remarks></remarks>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static char NotGreater(string name, char value, char maximumValue)
    {
        if (value > maximumValue)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotGreaterError, name, maximumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is less than a value.
    /// </summary>
    /// <remarks>
    /// To use this method with Code Contracts, be sure that the implementation of the
    /// IComparable&lt;T&gt;.CompareTo interface method is decorated with the <see cref="T:System.Diagnostics.Contracts.PureAttribute">Pure</see> attribute (pure
    /// methods do not make any visible state changes).
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static T NotGreater<T>(string name, T value, T maximumValue)
        where T : IComparable<T>
    {
        if (value.CompareTo(maximumValue) > 0)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotGreaterError, name, maximumValue));
        Contract.EndContractBlock();

        return value;
    }
    /// <summary>
    /// Determines whether the specified argument is less than a value.
    /// </summary>
    /// <remarks>
    /// Code Contracts always consider Comparison&lt;T&gt; delegates as Pure, so you can use this method without
    /// decorating your code with Pure attribute.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <param name="comparer">The comparer used to compare values.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it is not out of range.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is out
    /// of range.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    [ContractArgumentValidator]
    public static T NotGreater<T>(string name, T value, T maximumValue, Comparison<T> comparer)
    {
        NotNull("comparer", comparer);

        if (comparer(value, maximumValue) > 0)
            throw new ArgumentOutOfRangeException(name,
                string.Format(NotGreaterError, name, maximumValue));
        Contract.EndContractBlock();

        return value;
    }
    #endregion NotGreater

    /// <summary>
    /// Determines whether the specified argument match with a given pattern.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">The parameter value.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="regexOptions">The regular expression options.</param>
    /// <returns>
    /// 	<paramref name="value"/> if it matches the pattern.
    /// </returns>
    /// <remarks>
    /// This method cannot be used with Code Contracts, as it uses the Regex class.</remarks>
    /// <exception cref="System.ArgumentException">The <paramref name="value" /> is <c>null</c>.</exception>
    /// <exception cref="System.ArgumentException">The <paramref name="pattern" /> is <c>null</c>.</exception>
    /// <exception cref="System.ArgumentException">The <paramref name="value" /> doesn't match the <paramref name="pattern" />.</exception>
    [DebuggerNonUserCode, DebuggerStepThrough]
    public static string Match(string name, string value, string pattern, RegexOptions regexOptions = RegexOptions.None)
    {
        NotNull("value", value);
        NotNull("pattern", pattern);

        if (!Regex.IsMatch(value, pattern, regexOptions))
            throw new ArgumentException(string.Format("Argument '{0}' doesn't match with pattern '{1}'", name, pattern), name);

        return value;
    }
    #endregion
}
