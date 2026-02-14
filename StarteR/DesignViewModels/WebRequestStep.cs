using System.Collections.Generic;
using System.Net.Http;
using StarteR.Models.Steps;
using StarteR.ViewModels.Steps;

namespace StarteR.DesignViewModels;

public class WebRequestStep() : WebRequestStepViewModel(new WebRequestStepModel
{
    Body = "{\"key\": \"value\"}",
    BodyType = WebRequestBodyType.Text,
    Headers =
    [
        new KeyValuePair<string, string>("Content-Type", "application/json"),
        new KeyValuePair<string, string>("Authorization", "Bearer token value"),
        new KeyValuePair<string, string>("Custom-Header", "CustomValue")
    ],
    Method = HttpMethod.Post,
    Url = "https://example.com/api"
});