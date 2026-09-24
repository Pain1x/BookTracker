using BookTracker.DAL.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using BookTracker.DAL.Entities.Languages;

namespace BookTracker.DAL.Services
{
    public class ConfigurableTextTranslator(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        : ITextTranslator
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

        private readonly string _modelName = configuration["Translation:LMStudio:Model"];

        private string _provider = configuration["Translation:Provider"]?.Trim();
        
        private string _endpoint = configuration["Translation:LMStudio:Endpoint"]?.Trim();


        public async Task<string> TranslateAsync(string sourceText, Languages targetLanguage)
        {
            if (string.IsNullOrWhiteSpace(sourceText))
            {
                return sourceText;
            }
            if (string.IsNullOrEmpty(_provider))
            {
                // No translation provider configured, return original text.
                return sourceText;
            }

            return _provider.ToLowerInvariant() switch
            {
                "lmstudio" => await TranslateWithEvaluationAsync(sourceText, targetLanguage),
                _ => sourceText
            };
        }

        private async Task<string> TranslateWithEvaluationAsync(string textToTranslate, Languages targetLanguage)
        {
            var languageToTranslate = targetLanguage == Languages.Ukrainian 
                ? Languages.English 
                : Languages.Ukrainian;

            var step1SystemPrompt =
                $"You are an expert, professional translator specializing in high-fidelity localization. " +
                $"Your task is to translate the text to {languageToTranslate}.\n\n" +
                $"Follow these graduation steps to ensure quality:\n" +
                $"1. ANALYSIS: Identify the tone, idioms, and technical terms.\n" +
                $"2. TRANSLATION: Translate accurately, preserving meaning.\n" +
                $"3. REFINEMENT: Adapt the text so it sounds natural to a native speaker.\n\n" +
                $"CRITICAL RULE: Output ONLY the final translation inside <translation>...</translation> tags. No notes.";

            var step1Body = new
            {
                model = _modelName,
                messages = new[]
                {
                    new { role = "system", content = step1SystemPrompt },
                    new { role = "user", content = textToTranslate }
                },
                temperature = 0.3
            };

            string firstResponse = await SendPostRequestAsync(step1Body);
            string intermediateTranslation = ExtractTranslation(firstResponse);
            
            if (string.IsNullOrEmpty(intermediateTranslation))
            {
                intermediateTranslation = firstResponse;
            }
            
            var step2SystemPrompt = "You are a senior editor and quality assurance assistant for translations.\n\n" +
                                    "Your task is to evaluate the provided translation based on the original English text using these criteria:\n" +
                                    "- Accuracy\n- Naturalness\n- Terminology\n\n" +
                                    "After evaluation, fix any issues and output ONLY the polished, final translated text. " +
                                    "No explanations, no markdown headers, just the final text.";

            var step2Body = new
            {
                model = _modelName,
                messages = new[]
                {
                    new { role = "system", content = step2SystemPrompt },
                    new
                    {
                        role = "user",
                        content =
                            $"ORIGINAL TEXT:\n{textToTranslate}\n\nPROPOSED TRANSLATION:\n{intermediateTranslation}"
                    }
                },
                temperature = 0.1
            };

            string finalTranslation = await SendPostRequestAsync(step2Body);
            return finalTranslation.Trim();
        }
        
        private async Task<string> SendPostRequestAsync(object body)
        {
            var jsonPayload = JsonSerializer.Serialize(body);
            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            
            using var jsonDoc = JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;
            return root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
        }
        
        private string ExtractTranslation(string input)
        {
            var match = Regex.Match(input, @"<translation>(.*?)</translation>", RegexOptions.Singleline);
            return match.Success ? match.Groups[1].Value.Trim() : "";
        }
    }
}