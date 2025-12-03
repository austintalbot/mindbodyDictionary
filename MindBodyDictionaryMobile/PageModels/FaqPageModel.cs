using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MindBodyDictionaryMobile.PageModels;

/// <summary>
/// Page model for the FAQ page with expandable Q&amp;A items.
/// </summary>
public partial class FaqPageModel : ObservableObject
{
[ObservableProperty]
private ObservableCollection<FaqItem> faqItems = [];

public FaqPageModel()
{
InitializeFaqItems();
}

private void InitializeFaqItems()
{
FaqItems =
[
new FaqItem
{
Question = "What is the Mind Body Dictionary?",
Answer = "The Mind Body Dictionary is an app that explores the mind-body connection, showing how physical symptoms relate to emotional and psychological states."
},
new FaqItem
{
Question = "How do I search for a condition?",
Answer = "Use the Conditions tab to browse all mind-body conditions. You can search by name or explore different categories."
},
new FaqItem
{
Question = "What does Premium offer?",
Answer = "Premium membership provides access to detailed recommendations, resources, and exclusive content for deeper exploration."
},
new FaqItem
{
Question = "Can I save my favorite conditions?",
Answer = "Yes, you can mark conditions as favorites for quick access later from your Projects."
},
new FaqItem
{
Question = "Is my data private?",
Answer = "Yes, your data is kept private. We never share personal information with third parties."
}
];
}
}

/// <summary>
/// Represents a FAQ item with question and answer.
/// </summary>
public class FaqItem
{
public string Question { get; set; } = string.Empty;
public string Answer { get; set; } = string.Empty;
public bool IsExpanded { get; set; }
}
