using Godot;
using System;
using System.Threading.Tasks;
public class Global : Node
{
    public static Global Instance{get; private set;}
    public static HttpRequestManager Http {get;} = new HttpRequestManager();
    public static Map ActiveMap{get; set;}
    public static Controller LocalController { get; set; }
    public static void Log(object message)
    {
        GD.Print(message);
    }
    public static Exception Error(object message)
    {
        GD.PrintErr(message);
        return new Exception(message.ToString());
    }

    public static SignalAwaiter WaitFor(int seconds)
    {
        Timer t = new Timer();
        t.WaitTime = seconds;
        t.Autostart = true;
        t.Connect("timeout", Global.Instance, nameof(RemoveNode), new Godot.Collections.Array(){ t });
        Instance.AddChild(t);
        return t.ToSignal(t, "timeout");
    }
    public override void _Ready()
    {
        Instance = this;
    }

    public void RemoveNode(Node node)
    {
        node.QueueFree();
    }
}

public class HttpRequestManager : Godot.Object
{
    public async Task<HttpResponse> Request(string url, int timeout, string data = "")
    {
        HTTPRequest request = new HTTPRequest();
        Global.Instance.AddChild(request);
        Error e = request.Request(url, new string[]{ "Content-Type: application/json" }, false, HTTPClient.Method.Post, data);
        MonitorRequestTimeout(request, timeout, url);
        object[] response = await request.ToSignal(request, "request_completed");
        return new HttpResponse(response, url);
    }

    private async void MonitorRequestTimeout(HTTPRequest request, int timeout, string url)
    {
        Timer t = new Timer();
        t.WaitTime = timeout;
        t.Autostart = true;
        Global.Instance.AddChild(t);
        await ToSignal(t, "timeout");
        t.QueueFree();

        if(request.GetHttpClientStatus() == HTTPClient.Status.Connecting      ||
           request.GetHttpClientStatus() == HTTPClient.Status.ConnectionError ||
           request.GetHttpClientStatus() == HTTPClient.Status.CantConnect)
        {
            // timed out, log the error
            Global.Error(string.Format("Request to {0} timed out after {1} second(s)", url, timeout));
            request.QueueFree();
        }
    }
}

public class HttpResponse
{
    private object[] data;
    public int Result {get; private set;}
    public int ResponseCode {get; private set;}
    public string[] Headers {get; private set;}
    public byte[] RawResponse {get; private set;}
    public string Body {get; private set;}
    public string Url {get; private set;}
    public HttpResponse(object[] rawData, string url)
    {
        data = rawData;
        Result =        (int)       data[0];
        ResponseCode =  (int)       data[1];
        Headers =       (string[])  data[2];
        RawResponse =   (byte[])    data[3];
        Url = url;
        Body = System.Text.Encoding.UTF8.GetString(RawResponse);
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

            [BODY]
            {4}

            [RAW RESPONSE]
            {5}
        ", Url, Result, ResponseCode, string.Join(";\n\t\t", Headers), Body, string.Join(" ", RawResponse));
    }
}
