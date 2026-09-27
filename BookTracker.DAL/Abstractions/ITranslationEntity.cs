using BookTracker.DAL.Entities.Enums;
using BookTracker.DAL.Entities.Languages;

namespace BookTracker.DAL.Abstractions;

public interface ITranslationEntity
{
    public byte LanguagePk { get; set; }
    
    public Language Language { get; set; } 
}