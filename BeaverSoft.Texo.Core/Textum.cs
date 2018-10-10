using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Model.Configuration;
using BeaverSoft.Texo.Core.Model.View;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Core
{
    public class Textum
    {
        public ITextumFactory<ICommand> CommandFactory { get; set; }

        public Task ConfigureAsync(ITextumConfiguration configuration)
        {
            return Task.FromResult(0);
        }

        public static void Foo()
        {
            ItemsResult<Item> test = new ItemsResult<Item>("Test", "Text", "Result");

            ItemsResult test2 = new ItemsResult("Hi");

            DynamicResult test3 = new DynamicResult(ResultTypeEnum.Success);

            Item item1 = new Item("Text item");

        }
    }
}
