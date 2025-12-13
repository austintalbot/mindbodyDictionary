using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

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
				new()
				{
					Question = "How can the mind affect the body?",
					ShortAnswer= "Our experience affects us, more importantly the way we respond to our experience creates and transforms us into our present mind, heart, body, and self.",
					Answer ="When we harbor negative thought and emotion patterns in our system it creates additional stress in our body and life that is not present when we choose a more balanced, neutral, or joyful outlook.\n\nOur body may express the memories, feelings, beliefs and more through pain, dysfunction, or disease. It can be difficult to discern why our body is expressing these maladaptations unless we understand the language of the body. Hence the Mindbody Dictionary.",
					IsExpanded=false
				},
				new()
				{
					Question = "If I change my thoughts, will it really change my health?",
					ShortAnswer="In short, yes and no.",
					Answer ="I know. Helpful. We know on a basic level our thoughts guide our actions, but this is deeper. This is a way of thinking and a way of being. Your thoughts, beliefs, emotions, and response patterns all affect your cortisol and other hormone levels, how peaceful your sleep is, how easy it is for your body to repair, and how your immune system responds to its environment. As you align your phycological, emotional, relational world to your physical world you will find balance, peace, harmony and a greater wellbeing.",
					IsExpanded=false
				},
				new()
				{
					Question = "How do you know the specific emotion connected to the body?",
					ShortAnswer="In short several decades of research and observation have been brought together to offer deep insights into the specific nature of the mind-body connection.",
					Answer ="It’s also interesting to note that this information has been known and understood for thousands of years, and then forgotten. We are simply bringing it back into the light. Phrases such as “Pain in the neck.” “Thorn in my side.” “Heartache”, “Cold feet”,  and so many more have been used to describe physical manifestations of emotional realities.",
					IsExpanded=false
				},
				new()
				{
					Question = "Is this only for REALLY sick people, or is it helpful for anyone?",
					ShortAnswer="If you have a physical body, this is for you.",
					Answer ="If you have a heart and a mind, this is definitely for you.",
					IsExpanded=false
				},
				new()
				{
					Question = "Is there any science or explanations behind how this works?",
					ShortAnswer="Yes.",
					Answer ="There is substantial research from the fields of psychology, functional medicine, neurology, physiology, physical therapy, acupuncture, and more to discover what is behind the effect of stress, and trauma on the physical body. It is important to keep an open mind and realize that most people know that overwhelming stress affects health directly and that major emotional problems have harmful effects on the body and mind. Thoughts and emotions are powerful. For additional information about the why and how of the mind body connection refer to our web page at this link: https://mindbodydictionary.com/science-the-mindbody-connection/.",
					IsExpanded=false
				},
				new()
				{
					Question = "What if I’m not sure I fully believe it, will it still help me?",
					ShortAnswer="There is one thing for certain, it can’t hurt.",
					Answer ="Shifting negative thinking into positive thinking can only do you, your mind, heart, body, and even relationships good. Eating good foods and getting the nutrition you need will only be of benefit. Getting professional help to deal with past traumas or sorrows can only help. If it happens to help you heal something specific, we rejoice with you.",
					IsExpanded=false
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
public string ShortAnswer { get; set; } = string.Empty;
public string Answer { get; set; } = string.Empty;
public bool IsExpanded { get; set; }
}
