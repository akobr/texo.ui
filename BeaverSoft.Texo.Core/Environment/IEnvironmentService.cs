namespace BeaverSoft.Texo.Core.Environment
{
    public interface IEnvironmentService
    {
        void SetVariable(string variable, string value);

        string GetVariable(string variable);
    }
}