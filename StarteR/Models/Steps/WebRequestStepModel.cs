using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StarteR.Models.Steps;

public partial class WebRequestStepModel : StepModelBase
{
    [JsonIgnore] public override StepType Type => StepType.WebRequest;
    [JsonIgnore] public override string DisplayName => "Web Request";
    
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
            request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
        }
        if (!string.IsNullOrEmpty(Body))
        {
            request.Content = BodyType switch
            {
                WebRequestBodyType.Text => new StringContent(Body),
                WebRequestBodyType.FormUrlEncoded => new FormUrlEncodedContent(JsonSerializer.Deserialize<Dictionary<string, string>>(Body) ?? new Dictionary<string, string>()),
                _ => throw new ArgumentOutOfRangeException()
            };
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