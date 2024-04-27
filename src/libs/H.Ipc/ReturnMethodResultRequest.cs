namespace H.IpcGenerators;

/// <summary>
/// Representation of the result of an IPC call. This is the non-generic class, which holds
/// the most basic information, such as IsSuccessful, ErrMsg and ResultType.
/// This class functions as the base class for the generic version, which contains the actual
/// result type as defined in the user's interface.
/// As the generated server-side code does with the <see cref="RpcRequest"/>, the generated
/// client-side code first deserializes the returned JSON string into an object of this base
/// type, checks its ResultType property, and deserializes the JSON again, but this time into
/// the derived generic ReturnMethodResultRequest{T} with T being the return type specified in
/// the user's defined protocol interface.
/// </summary>
public class ReturnMethodResultRequest : RpcRequest
{
    /// <summary>
    /// Needed by the JSON serializer to be able to deserialize. Defaults to a failed return value,
    /// i.e. IsSuccessful set to false.
    /// </summary>
    public ReturnMethodResultRequest() : base(RpcRequestType.ReturnMethodResult)
    {
        IsSuccessful = false;
        ErrMsg = string.Empty;
        ResultType = string.Empty;
    }
    
    /// <summary>
    /// Typically used to signify a failed return value, including an error message.
    /// </summary>
    /// <param name="isSuccessful"></param>
    /// <param name="errorMessage"></param>
    public ReturnMethodResultRequest(bool isSuccessful, string? errorMessage) : this()
    {
        IsSuccessful = isSuccessful;
        ErrMsg = errorMessage;
    }

    /// <summary>
    /// Used by the derived generic type's constructor. This is used for successful returns, and
    /// as such requires the concrete return type.
    /// </summary>
    /// <param name="resultType"></param>
    protected ReturnMethodResultRequest(string resultType) : base(RpcRequestType.ReturnMethodResult)
    {
        ResultType = resultType;
        IsSuccessful = true;
        ErrMsg = string.Empty;
    }

    /// <summary>
    /// The type name of the result. Used in the client-side code to know which result
    /// deserialization to use.
    /// </summary>
    public string ResultType { get; set; }

    /// <summary>
    /// Indication of whether the method call was successful.
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// Any error message to accompany a failure result.
    /// </summary>
    public string? ErrMsg { get; set; }
}

/// <summary>
/// The generic version derived from the <see cref="ReturnMethodResultRequest"/>.
/// This will contain the result value as the concrete type specified in the protocol interface.
/// </summary>
public class ReturnMethodResultRequest<T> : ReturnMethodResultRequest
{
    /// <summary>
    /// Needed by JSON deserialization
    /// </summary>
    public ReturnMethodResultRequest()
    {
        Result = default;
    }

    /// <summary>
    /// Represents a request to return the result of a method call.
    /// </summary>
    public ReturnMethodResultRequest(T result) : base(typeof(T).Name)
    {
        Result = result;
    }

    /// <summary>
    /// The Result property is used in the client-side code to store the result value of a method call.
    /// The concrete type of the result is specified in the protocol interface.
    /// </summary>
    public T? Result { get; set; }
}

/// <summary>
/// Simple factory class to create instances of the generic ReturnMethodResultRequest{T}
/// class, allowing for implicit type deduction. This allows the code generator to be
/// a bit simpler as it does not need to specify the result types explicitly.
/// </summary>
public static class ReturnMethodResultFactory
{
    /// <summary>
    /// Create a new instance of the generic <see cref="ReturnMethodResultRequest{T}"/> class.
    /// Used by the generated server-side code to create the specific return method result request
    /// from the value returned from the actual method call. 
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ReturnMethodResultRequest<T> Create<T>(T result)
    {
        return new ReturnMethodResultRequest<T>(result);
    }
}