namespace CommunityPlugin.Objects.Interface
{
    public interface ITranslator
    {
        ICriterionParser Parser { get; }

        string GetFieldValue(string fieldID, IMapping mapping);

        string GetTranslatioinValue(string translation, IMapping mapping);
    }
}
