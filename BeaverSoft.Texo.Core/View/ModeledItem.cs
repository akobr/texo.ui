using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Model.Text;

namespace BeaverSoft.Texo.Core.View
{
    public class ModeledItem : IItem
    {
        private readonly IElement model;

        public ModeledItem(IElement model)
        {
            this.model = model;
        }

        public string Text => model.ToString();

        public TextFormatEnum Format => TextFormatEnum.Model;

        public IImmutableList<IAction> Actions { get; set; }
    }
}