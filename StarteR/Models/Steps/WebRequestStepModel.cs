using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StarteR.Models.Steps;

public class WebRequestStepModel : StepModelBase
{
    [JsonIgnore] public override StepType Type => StepType.WebRequest;
    [JsonIgnore] public override string DisplayName => "Web Request";
    
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
    public string Headers { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    
    protected override async Task ExecuteAsync()
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(new HttpMethod(Method), Url);
        var task = client.SendAsync(request);
        if (WaitForCompletion)
        {
            await task;
        }
    }
}