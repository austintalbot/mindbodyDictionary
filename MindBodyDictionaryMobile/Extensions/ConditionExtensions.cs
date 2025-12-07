using MindBodyDictionary.Shared.Entities;

namespace MindBodyDictionaryMobile.Extensions
{
    public static class ConditionExtensions
    {
        public static bool IsNullOrNew(this MbdCondition? condition)
        {
            return condition == null || condition.ID == 0;
        }
    }
}
