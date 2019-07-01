using System;
using System.IO;

namespace BeaverSoft.Texo.Core.Environment
{
    internal class CurrentDirectoryStrategy : IVariableStrategy
    {
        public bool IsValueValid(string newValue, string currentValue)
        {
            return Directory.Exists(newValue);
        }

        public bool CanBeRemoved(string currentValue)
        {
            return false;
        }

        public void OnValueChange(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            string fullPath = System.IO.Path.GetFullPath(value);
            System.Environment.CurrentDirectory = fullPath;
        }
    }
}