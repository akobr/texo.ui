using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Commands.CodeBaseSearch.Model
{
    public class DocumentSubject : Subject
    {
        private readonly Document file;

        public DocumentSubject(Document file)
        {
            this.file = file;
            Name = file.Name;
            Keywords = new DocumentKeywordBuilder(file).Build();
            PreLoad();
        }

        public async Task LoadAsync()
        {
            SyntaxNode root = await file.GetSyntaxRootAsync();
            SetChildren(LoadTypes(root));
        }

        private void PreLoad()
        {
            if (!file.TryGetSyntaxRoot(out SyntaxNode root))
            {
                return;
            }

            SetChildren(LoadTypes(root));
        }

        private IImmutableList<ISubject> LoadTypes(SyntaxNode root)
        {
            var walker = new TypeAndMembersWalker(this);
            walker.Visit(root);
            return walker.GetTypeSubjects();
        }
    }
}
