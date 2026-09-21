using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BookTracker.BLL.Abstractions;
using Newtonsoft.Json; // Assuming this JSON library is available or needs to be added

namespace BookTracker.BLL.Services
{
    /// <summary>
    /// Concrete implementation for translation using a local LLM endpoint (LM Studio).
    /// </summary>
    public class LocalLlmTranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        // Use the provided local endpoint URL
        private const string LlmApiUrl = "http://192.168.0.250:1234/v1/completions"; 
        // Note: Assuming OpenAI compatible completion endpoint structure for simplicity

        public LocalLlmTranslationService()
        {
            _httpClient = new HttpClient();
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, Dictionary<string, string>>> TranslateMetadataAsync(byte sourceLang, List<Language> targetLangs, BookModel book)
        {
            // 1. Build the System Prompt (The core instruction set for the LLM)
            var systemPrompt = BuildSystemPrompt(sourceLang, targetLangs);

            // 2. Construct the API Request Payload
            var payload = new
            {
                model = "local-llm-model", // Placeholder model name
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = $"Translate the book metadata provided. Source Lang: {(Language)sourceLang}. Target Lgs: {string.Join(", ", targetLangs)}." }
                },
                temperature = 0.1,
                max_tokens = 500
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                // 3. Execute the HTTP Call
                HttpResponseMessage response = await _httpClient.PostAsync(LlmApiUrl, content);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                
                // 4. Parse and return structured data (requires careful parsing based on actual LLM API output)
                return ParseTranslationResponse(jsonResponse);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"ERROR connecting to local LLM service at {LlmApiUrl}. Ensure LM Studio is running and accessible: {ex.Message}");
                // Return empty dictionary on failure, allowing the calling code to handle fallback.
                return new Dictionary<string, Dictionary<string, string>>(); 
            }
        }

        /// <summary>
        /// Builds a detailed system prompt instructing the LLM how to perform the translation and what format to use.
        /// </summary>
        private string BuildSystemPrompt(byte sourceLang, List<Language> targetLangs)
        {
             // This is a highly critical prompt that needs continuous refinement.
            var targetsList = string.Join(", ", targetLangs.Select(l => l.ToString()));

            return $@"You are an expert localization service for book metadata. Your task is to translate the following title, author name, and genre from {Language.ToString((Language)sourceLang)} into {targetsList}.
            
            RULES:
            1. Do not add any explanatory text, markdown formatting (like ```json```), or preamble outside of the required JSON object.
            2. You must provide translations for ALL requested target languages and ALL specified fields (title, author, genre).
            3. The output MUST be a single, valid JSON object that adheres to the schema provided below.

            INPUT BOOK DATA:
            - Title: {book.Title}
            - Author: {book.Author}
            - Genre: {book.Genre}

            REQUIRED OUTPUT JSON SCHEMA:
            {{
              ""title"": {{
                ""en"": ""[English translation of the title]"", 
                ""uk"": ""[Ukrainian translation of the title]""
              }},
              ""author"": {{
                ""en"": ""[English translation of the author's name]"",
                ""uk"": ""[Ukrainian translation of the author's name]"""
              }},
              ""genre"": {{
                ""en"": ""[English translation of the genre]"", 
                ""uk"": ""[Ukrainian translation of the genre]""
              }}
            }}";
        }

        /// <summary>
        /// Placeholder for parsing the complex JSON response from the LLM.
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> ParseTranslationResponse(string jsonResponse)
        {
             // In a real application, this would use reflection/strong typing 
             // to parse the specific structure requested in BuildSystemPrompt.
            Console.WriteLine("Successfully received response JSON payload (Parsing logic required).");
            return new Dictionary<string, Dictionary<string, string>>(); // Return empty for now
        }
    }
}