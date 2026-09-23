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
        private readonly HttpClient _httpClient;

        public ConfigurableTextTranslator(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : this(httpClientFactory.CreateClient(), configuration) { }


        public async Task<string> TranslateAsync(string sourceText, Languages targetLanguage, string contentType, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sourceText))
            {
                return sourceText;
            }

            var provider = configuration["Translation:Provider"]?.Trim();

            if (string.IsNullOrEmpty(provider))
            {
                // No translation provider configured, return original text.
                return sourceText;
            }

            return provider.ToLowerInvariant() switch
            {
                "lmstudio" => await TranslateWithLMStudioAsync(sourceText, targetLanguage, contentType, cancellationToken),
                "anthropic" => await TranslateWithAnthropicAsync(sourceText, targetLanguage, contentType, cancellationToken), // NEW PATH
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
                ? $"Translate the text to {targetLanguage}. Return only the translation."
                : promptTemplate;

            // The context instruction from the original system message is folded into the main prompt for compatibility with simpler APIs.
            var prompt = template.Replace("{{text}}", sourceText);

            // Simplified request body payload structure (LM Studio compatible)
            var requestBody = new
            {
                model,
                input = prompt,
                temperature = 0.2
            };

            try
            {
                var client = _httpClient; // Use injected client
                var requestUri = baseUrl.TrimEnd('/') + "/api/v1/chat";

                string jsonPayload = JsonSerializer.Serialize(requestBody);
                var reqcontent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(requestUri, reqcontent);
                var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
                using var jsonDocument = JsonDocument.Parse(jsonString);

                // Assuming the API structure still contains "choices" and "message" properties at this level for success
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
            catch (Exception ex)
            {
                // Log the exception in a real application, but for now, just return original text.
                System.Diagnostics.Debug.WriteLine($"LMStudio translation failed: {ex.Message}");
                return sourceText;
            }
        }

        private async Task<string> TranslateWithAnthropicAsync(string sourceText, Languages targetLanguage, contentType, CancellationToken cancellationToken)
        {
            var model = configuration["Translation:Anthropic:Model"];
            // In a real setup, the API Key would be read securely from environment variables or secret manager.
            // For this demonstration, we assume it's configured in IConfiguration/HttpClientFactory setup.
            var apiKey = configuration["Translation:Anthropic:ApiKey"];

            if (string.IsNullOrWhiteSpace(model) || string.IsNullOrWhiteSpace(apiKey))
            {
                System.Diagnostics.Debug.WriteLine("Anthropic model or API Key not configured.");
                return sourceText;
            }

            // Anthropic uses a specific base URL and requires the Authorization header to be set up in the HttpClient factory,
            // but we'll assume _httpClient is already correctly configured with the BaseAddress/Authorization headers for simplicity here.
            var apiEndpoint = "https://api.anthropic.com/v1/messages";

            // Constructing the message payload according to Anthropic's standards.
            var promptTemplate = configuration["Translation:Anthropic:Templates:" + contentType + ":" + targetLanguage];
            var template = string.IsNullOrWhiteSpace(promptTemplate)
                ? $"Translate the text to {targetLanguage}. Return only the translation."
                : promptTemplate;

            var userPrompt = template.Replace("{{text}}", sourceText);


            // The required structure for Anthropic messages array.
            var requestBody = new
            {
                model, // e.g., "claude-3-opus-20240229"
                messages = new[]
                {
                    new { role = "system", content = $"You are a professional and neutral translation engine. Translate the following text accurately to {targetLanguage} and provide only the translated text, with no additional commentary or formatting." },
                    new { role = "user", content = userPrompt }
                },
                max_tokens = 1024,
                temperature = 0.1 // Anthropic recommends keeping temperature low for translation/extraction tasks.
            };

            try
            {
                var client = _httpClient;
                // NOTE: The actual endpoint might be different depending on the wrapper used in the project (e.g., a custom API facade).
                var requestUri = apiEndpoint;

                string jsonPayload = JsonSerializer.Serialize(requestBody);
                var reqcontent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(requestUri, reqcontent, cancellationToken);
                response.EnsureSuccessStatusCode(); // Throw exception on non-success status code

                var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
                using var jsonDocument = JsonDocument.Parse(jsonString);

                // Anthropic specific parsing: content is usually nested under "content" property of the last message object.
                if (jsonDocument.RootElement.TryGetProperty("content", out var rootContent) &&
                    rootContent.ValueKind == StringValueKind)
                {
                    var translated = rootContent.GetString()?.Trim();
                    return string.IsNullOrWhiteSpace(translated) ? sourceText : translated;
                }

                return sourceText;
            }
            catch (HttpRequestException ex) when (ex.StatusCode != null && ((int)ex.StatusCode) == 401)
            {
                System.Diagnostics.Debug.WriteLine("Anthropic API Error: Unauthorized. Check API Key and permissions.");
                return sourceText; // Handle auth failure gracefully
            }
            catch (Exception ex)
            {
                // Log the exception in a real application, but for now, just return original text.
                System.Diagnostics.Debug.WriteLine($"Anthropic translation failed: {ex.Message}");
                return sourceText;
            }
        }
    }
}