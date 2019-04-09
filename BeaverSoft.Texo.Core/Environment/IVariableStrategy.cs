namespace BeaverSoft.Texo.Core.Environment
{
    public interface IVariableStrategy
    {
        bool IsValueValid(string newValue, string currentValue);

        bool CanBeRemoved(string currentValue);

        void OnValueChange(string value);
    }
}