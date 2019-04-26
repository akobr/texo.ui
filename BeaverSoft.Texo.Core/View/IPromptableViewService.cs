namespace BeaverSoft.Texo.Core.View
{
    public interface IPromptableViewService
    {
        string GetNewInput();

        // int Prompt(IPromptRequest);

        // IEnumerable<int> PromptMultiple(IPromptRequest);

        void ShowProgress(int id, string name, int progress);
    }
}
