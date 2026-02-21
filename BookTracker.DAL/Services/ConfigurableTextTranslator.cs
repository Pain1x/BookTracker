using BookTracker.DAL.Abstractions;

using Microsoft.Extensions.Configuration;

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BookTracker.DAL.Services
{
	public class ConfigurableTextTranslator(
		IHttpClientFactory httpClientFactory,
		IConfiguration configuration) : ITextTranslator
	{
		private const string UkrainianLanguageCode = "uk";

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
				"libretranslate" => await TranslateWithLibreTranslateAsync(sourceText, cancellationToken),
				_ => sourceText
			};
		}

		private async Task<string> TranslateWithLibreTranslateAsync(string sourceText, CancellationToken cancellationToken)
		{
			var endpoint = configuration["Translation:LibreTranslate:Endpoint"];
			var apiKey = configuration["Translation:LibreTranslate:ApiKey"];

			if (string.IsNullOrWhiteSpace(endpoint))
			{
				return sourceText;
			}

			try
			{
				var client = httpClientFactory.CreateClient();
				var requestUri = endpoint.TrimEnd('/') + "/translate";

				using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
				{
					Content = JsonContent.Create(new
					{
						q = sourceText,
						source = "auto",
						target = UkrainianLanguageCode,
						format = "text",
						api_key = apiKey
					})
				};

				using var response = await client.SendAsync(request, cancellationToken);
				if (!response.IsSuccessStatusCode)
				{
					return sourceText;
				}

				await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
				using var jsonDocument = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);

				if (!jsonDocument.RootElement.TryGetProperty("translatedText", out var textElement))
				{
					return sourceText;
				}

				var translated = textElement.GetString();
				return string.IsNullOrWhiteSpace(translated) ? sourceText : translated;
			}
			catch
			{
				return sourceText;
			}
		}
	}
}
