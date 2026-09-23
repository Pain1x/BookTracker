using BookTracker.DAL.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BookTracker.DAL.Entities.Languages;

namespace BookTracker.DAL.Services
{
    public class ConfigurableTextTranslator(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration) : ITextTranslator
    {
        public async Task<string> TranslateAsync(string sourceText, Languages targetLanguage, string contentType, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sourceText))
            {
                return sourceText;
            }

            var provider = configuration["Translation:Provider"]?.Trim();

            return provider?.ToLowerInvariant() switch
            {
                "lmstudio" => await TranslateWithLMStudioAsync(sourceText, targetLanguage, contentType, cancellationToken),
                _ => sourceText
            };
        }

        private async Task<string> TranslateWithLMStudioAsync(string sourceText, Languages targetLanguage, string contentType, CancellationToken cancellationToken)
        {
            var endpoint = configuration["Translation:LMStudio:Endpoint"];
            var model = configuration["Translation:LMStudio:Model"];
            var promptTemplate = configuration["Translation:LMStudio:Templates:" + contentType + ":" + targetLanguage];

            if (string.IsNullOrWhiteSpace(model))
            {
                return sourceText;
            }

            var baseUrl = string.IsNullOrWhiteSpace(endpoint)
                ? "http://192.168.0.250:1234"
                : endpoint.Trim();

            var template = string.IsNullOrWhiteSpace(promptTemplate)
                ? "Translate to " + targetLanguage + ". Return only the translation."
                : promptTemplate;

            var prompt = template.Replace("{{text}}", sourceText);
            
            var requestBody = new
            {
                model, 
                input = prompt,
                messages = new[]
                {
                    new { role = "system", content = "You are a professional and neutral translation engine. " +
                                                     "Translate the provided text accurately to the target language and provide only the translated text, " +
                                                     "with no additional commentary or formatting." },
                    new { role = "user", content = prompt}
                },
                temperature = 0.2
            };

            try
            {
                var client = httpClientFactory.CreateClient();
                var requestUri = baseUrl.TrimEnd('/') + "/api/v1/chat";
                
                string jsonPayload = JsonSerializer.Serialize(requestBody);
                var reqcontent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = await client.PostAsync(requestUri,  reqcontent);
                var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
                using var jsonDocument = JsonDocument.Parse(jsonString);

                if (jsonDocument.RootElement.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0 &&
                    choices[0].TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var content))
                {
                    var translated = content.GetString()?.Trim();
                    return string.IsNullOrWhiteSpace(translated) ? sourceText : translated;
                }

                return sourceText;
            }
            catch
            {
                return sourceText;
            }
        }
    }
}
