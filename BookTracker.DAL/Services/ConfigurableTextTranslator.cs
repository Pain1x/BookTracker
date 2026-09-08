using BookTracker.DAL.Abstractions;

using Microsoft.Extensions.Configuration;

using System.Net.Http.Json;
using System.Text.Json;

namespace BookTracker.DAL.Services
{
	public class ConfigurableTextTranslator(
		IHttpClientFactory httpClientFactory,
		IConfiguration configuration) : ITextTranslator
	{
		private const string DefaultOllamaEndpoint = "http://localhost:11434";
		private const string DefaultOllamaPromptTemplate = """
			Translate to Ukrainian.

			Rules:
			- Keep proper names recognizable.
			- Do not invent anything.
			- If the text is already a proper name, transliterate it.
			- Return only translation.

			Text:
			{{text}}
			""";

		public async Task<string> TranslateToUkrainianAsync(string sourceText, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(sourceText))
			{
				return sourceText;
			}

			var provider = configuration["Translation:Provider"]?.Trim();

			if (string.IsNullOrWhiteSpace(provider) || provider.Equals("none", StringComparison.OrdinalIgnoreCase))
			{
				return sourceText;
			}

			return provider.ToLowerInvariant() switch
			{
				"ollama" => await TranslateWithOllamaAsync(sourceText, cancellationToken),
				_ => sourceText
			};
		}

		private async Task<string> TranslateWithOllamaAsync(string sourceText, CancellationToken cancellationToken)
		{
			var endpoint = configuration["Translation:Ollama:Endpoint"];
			var model = configuration["Translation:Ollama:Model"];
			var promptTemplate = configuration["Translation:Ollama:PromptTemplate"];

			if (string.IsNullOrWhiteSpace(model))
			{
				return sourceText;
			}

			var baseUrl = string.IsNullOrWhiteSpace(endpoint)
				? DefaultOllamaEndpoint
				: endpoint.Trim();

			var template = string.IsNullOrWhiteSpace(promptTemplate)
				? DefaultOllamaPromptTemplate
				: promptTemplate;

			var prompt = template.Replace("{{text}}", sourceText);

			try
			{
				var client = httpClientFactory.CreateClient();
				var requestUri = baseUrl.TrimEnd('/') + "/api/generate";

				using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
				{
					Content = JsonContent.Create(new
					{
						model,
						prompt,
						stream = false,
						options = new
						{
							temperature = 0.1
						}
					})
				};

				using var response = await client.SendAsync(request, cancellationToken);
				if (!response.IsSuccessStatusCode)
				{
					return sourceText;
				}

				await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
				using var jsonDocument = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);

				if (!jsonDocument.RootElement.TryGetProperty("response", out var responseElement))
				{
					return sourceText;
				}

				var translated = responseElement.GetString()?.Trim();
				return string.IsNullOrWhiteSpace(translated) ? sourceText : translated;
			}
			catch
			{
				return sourceText;
			}
		}
	}
}
