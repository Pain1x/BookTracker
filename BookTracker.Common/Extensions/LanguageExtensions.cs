using BookTracker.Common.Enums;

namespace BookTracker.Common.Extensions;

public static class LanguageExtensions
{
    public static Languages InvertLanguage(this Languages language) => 
        language == Languages.English ? Languages.Ukrainian : Languages.English;
    
    public static string GetLanguageName(this Languages language) => 
        language.ToString();
}