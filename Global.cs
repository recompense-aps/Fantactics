using Godot;
using System;
using System.Threading.Tasks;
public class Global : Node
{
    public static Global Instance{get; private set;}
    public static HttpRequestManager Http {get;} = new HttpRequestManager();
    public static Map ActiveMap{get; set;}
    public static void Log(object message)
    {
        GD.Print(message);
    }
    public static Exception Error(object message)
    {
        GD.PrintErr(message);
        return new Exception(message.ToString());
    }
    public override void _Ready()
    {
        Instance = this;
    }
}

public class HttpRequestManager
{
    public async Task<HttpResponse> Request(string url)
    {
        HTTPRequest request = new HTTPRequest();
        Global.Instance.AddChild(request);
        request.Request(url);
        object[] response = await request.ToSignal(request, "request_completed");
        return new HttpResponse(response, url);
    }
}

public class HttpResponse
{
    private object[] data;
    public int Result {get; private set;}
    public int ResponseCode {get; private set;}
    public string[] Headers {get; private set;}
    public byte[] RawResponse {get; private set;}
    public string Url {get; private set;}
    public HttpResponse(object[] rawData, string url)
    {
        data = rawData;
        Result =        (int)       data[0];
        ResponseCode =  (int)       data[1];
        Headers =       (string[])  data[2];
        RawResponse =   (byte[])    data[3];
    }

    public override string ToString()
    {
        return string.Format(@"
            [HTTP REQUEST TO]
            {0}
            [RESULT]
            {1}
            [RESPONSE CODE]
            {2}
            [HEADERS]
            {3}
            [RAW RESPONSE]
            {4}
        ", Url, Result, ResponseCode, Headers, RawResponse);
    }
}
