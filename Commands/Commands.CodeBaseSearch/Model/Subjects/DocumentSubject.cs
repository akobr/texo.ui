using System.Collections.Immutable;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Markdown.Builder;
using Commands.CodeBaseSearch.Model.KeywordBuilders;
using Microsoft.CodeAnalysis;

namespace Commands.CodeBaseSearch.Model.Subjects
{
    public class DocumentSubject : Subject
    {
        private readonly Document file;       

        public DocumentSubject(Document file)
            : base(SubjectTypeEnum.Document, file.Name, new DocumentKeywordBuilder(file))
        {
            this.file = file;
        }

        public bool IsLoaded { get; private set; }

        public async Task LoadAsync()
        {
            IsLoaded = true;
            SyntaxNode root = await file.GetSyntaxRootAsync();
            SetChildren(LoadTypes(root));
        }

        public override void WriteToMarkdown(MarkdownBuilder builder, int intent)
        {
            builder.Bullet(intent);
            builder.Write("**");
            builder.Link(Name, ActionBuilder.PathOpenUri(file.FilePath));
            builder.Write("** *(file)*");
        }

        private void PreLoad()
        {
            if (!file.TryGetSyntaxRoot(out SyntaxNode root))
            {
                return;
            }

            IsLoaded = true;
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
