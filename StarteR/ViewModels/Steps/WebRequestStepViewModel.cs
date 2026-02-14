using System.Collections.Generic;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarteR.Models.Steps;

namespace StarteR.ViewModels.Steps;

public partial class WebRequestStepViewModel(WebRequestStepModel model) : StepViewModelBase<WebRequestStepModel>(model)
{
    public static HttpMethod[] HttpMethods { get; } = [HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete];
    public static WebRequestBodyType[] BodyTypes { get; } = [WebRequestBodyType.Text, WebRequestBodyType.FormUrlEncoded];
    
    [ObservableProperty]
    private string _headerKey = string.Empty;
    
    [ObservableProperty]
    private string _headerValue = string.Empty;
    
    [RelayCommand]
    private void AddHeader()
    {
        if (string.IsNullOrEmpty(HeaderKey))
        {
            return;
        }
        Model.Headers.Add(new KeyValuePair<string, string>(HeaderKey, HeaderValue));
        HeaderKey = string.Empty;
        HeaderValue = string.Empty;
    }

    [RelayCommand]
    private void RemoveHeader(KeyValuePair<string, string> header)
    {
        Model.Headers.Remove(header);
    }
}