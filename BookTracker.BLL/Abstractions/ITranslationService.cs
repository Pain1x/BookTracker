using System;
using System.Collections.Generic;
using BookTracker.BLL.Models; // Assuming this model is available here

namespace BookTracker.BLL.Abstractions
{
    /// <summary>
    /// Contract for translating book metadata using any LLM service (local or remote).
    /// </summary>
    public interface ITranslationService
    {
        /// <summary>
        /// Translates the core book metadata fields into specified target languages.
        /// </summary>
        /// <param name="sourceLang">The language of the original input data.</param>
        /// <param name="targetLangs">A list of desired output languages (e.g., English, Ukrainian).</param>
        /// <param name="book">The source BookModel containing original metadata.</param>
        /// <returns>A dictionary containing translated fields mapped by language and field type.</returns>
        Task<Dictionary<string, Dictionary<string, string>>> TranslateMetadataAsync(byte sourceLang, List<Language> targetLangs, BookModel book);
    }

    // Define a simple Language enum or class if it doesn't exist in the project.
    public enum Language
    {
        English = 1,
        Ukrainian = 2,
        Unknown = 0
    }
}