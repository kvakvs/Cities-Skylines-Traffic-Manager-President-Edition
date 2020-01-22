namespace CSUtil.Commons {
    /// <summary>
    /// Tri-bool: a boolean value which can also be undefined.
    /// </summary>
    public enum TernaryBool {
        /// <summary>The value is not known or not defined, but neither true nor false.</summary>
        Undefined = 0,

        /// <summary>The value is false.</summary>
        False = 1,

        /// <summary>The value is true.</summary>
        True = 2,
    }
}