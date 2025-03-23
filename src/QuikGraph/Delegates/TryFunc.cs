namespace QuikGraph
{
    /// <summary>
    /// Delegate that has one parameter and returns an out value of the type specified by the <typeparamref name="TResult"/> parameter.
    /// This method can fail so the boolean return type indicate the state succeeded or not of the method.
    /// </summary>
    /// <typeparam name="T">The type of the parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <param name="arg">The parameter of the method that this delegate encapsulates.</param>
    /// <param name="result">The result of the method if it succeeds.</param>
    /// <returns>Boolean indicating if the method succeeded or not.</returns>
    public delegate bool TryFunc<in T, TResult>(T arg, out TResult result);

    /// <summary>
    /// Delegate that has 2 parameters and returns an out value of the type specified by the <typeparamref name="TResult"/> parameter.
    /// This method can fail so the boolean return type indicate the state succeeded or not of the method.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="result">The result of the method if it succeeds.</param>
    /// <returns>Boolean indicating if the method succeeded or not.</returns>
    public delegate bool TryFunc<in T1, in T2, TResult>(T1 arg1, T2 arg2, out TResult result);

    /// <summary>
    /// Delegate that has 3 parameters and returns an out value of the type specified by the <typeparamref name="TResult"/> parameter.
    /// This method can fail so the boolean return type indicate the state succeeded or not of the method.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="result">The result of the method if it succeeds.</param>
    /// <returns>Boolean indicating if the method succeeded or not.</returns>
    public delegate bool TryFunc<in T1, in T2, in T3, TResult>(T1 arg1, T2 arg2, T3 arg3, out TResult result);

    /// <summary>
    /// Delegate that has 4 parameters and returns an out value of the type specified by the <typeparamref name="TResult"/> parameter.
    /// This method can fail so the boolean return type indicate the state succeeded or not of the method.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="result">The result of the method if it succeeds.</param>
    /// <returns>Boolean indicating if the method succeeded or not.</returns>
    public delegate bool TryFunc<in T1, in T2, in T3, in T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, out TResult result);
}