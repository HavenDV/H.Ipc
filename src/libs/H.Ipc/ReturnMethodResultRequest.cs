namespace H.IpcGenerators;

/// <summary>
/// Representation of the result of an IPC call. This is the non-generic class, which holds
/// the most basic information, such as IsSuccessful, ErrMsg and ResultType.
/// This class functions as the base class for the generic version, which contains the actual
/// result type as defined in the user's interface.
/// Instances of this class (where not part of a derived class) represents a failure return value.
/// </summary>
public class ReturnMethodResultRequest : RpcRequest
{
    public ReturnMethodResultRequest() : base(RpcRequestType.ReturnMethodResult)
    {
        IsSuccessful = false;
        ErrMsg = string.Empty;
    }
    
    public ReturnMethodResultRequest(bool isSuccessful, string? errorMessage) : this()
    {
        IsSuccessful = isSuccessful;
        ErrMsg = errorMessage;
    }

    protected ReturnMethodResultRequest(string resultType) : base(RpcRequestType.ReturnMethodResult)
    {
        ResultType = resultType;
        IsSuccessful = true;
        ErrMsg = string.Empty;
    }

    public string ResultType { get; set; }

    public string? ErrMsg { get; set; }

    public bool IsSuccessful { get; set; }
}

/// <summary>
/// 
/// </summary>
public class ReturnMethodResultRequest<T> : ReturnMethodResultRequest
{
    public ReturnMethodResultRequest()
    {
        Result = default;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public ReturnMethodResultRequest(T result) : base(typeof(T).Name)
    {
        Result = result;
    }
    
    public T? Result { get; set; }
}

public class ReturnMethodResultFactory
{
    public static ReturnMethodResultRequest<T> Create<T>(T result)
    {
        return new ReturnMethodResultRequest<T>(result);
    }
}