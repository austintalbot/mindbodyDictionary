using CommunityToolkit.Mvvm.ComponentModel;

namespace MindBodyDictionaryMobile.Models;

public partial class FaqItem : ObservableObject
{
    public string Id { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string ShortAnswer { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isExpanded;
}
