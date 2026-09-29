using BookTracker.DAL.Entities.Languages;

namespace BookTracker.DAL.Abstractions;

public abstract class TranslationEntity
{
    public byte LanguagePk { get; set; }
    
    public Language Language { get; set; }
}