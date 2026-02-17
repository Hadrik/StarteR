using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using StarteR.StepManagement;

namespace StarteR.Models.Steps;

[RegisterStep("web-request", "Web Request")]
public partial class WebRequestStepModel : StepModelBase
{
    [ObservableProperty]
    private string _url = string.Empty;
    
    [ObservableProperty]
    private HttpMethod _method = HttpMethod.Get;
    
    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _headers = [];
    
    [ObservableProperty]
    private string _body = string.Empty;
    
    [ObservableProperty]
    private WebRequestBodyType _bodyType = WebRequestBodyType.Text;
    
    private static readonly HttpClient Client = new();
    
    protected override async Task ExecuteAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = Method,
            RequestUri = new Uri(Url),
        };
        foreach (var keyValuePair in Headers)
        {
            try
            {
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Invalid header: {keyValuePair.Key}: {keyValuePair.Value}", e);
            }
        }
        if (!string.IsNullOrEmpty(Body))
        {
            try
            {
                request.Content = BodyType switch
                {
                    WebRequestBodyType.Text => new StringContent(Body),
                    WebRequestBodyType.FormUrlEncoded => new FormUrlEncodedContent(JsonSerializer.Deserialize<Dictionary<string, string>>(Body) ?? new Dictionary<string, string>()),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            catch (JsonException e)
            {
                throw new InvalidOperationException("Invalid body format for FormUrlEncoded. Expected a JSON object with string key-value pairs.", e);
            }
        }
        
        var task = Client.SendAsync(request);
        if (WaitForCompletion)
        {
            await task;
        }
    }
}

public enum WebRequestBodyType
{
    Text,
    FormUrlEncoded
}