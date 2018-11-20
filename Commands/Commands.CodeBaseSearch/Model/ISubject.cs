namespace Commands.CodeBaseSearch.Model
{
    public interface ISubject : ISearchable, ISearchTreeNode
    {
        SubjectTypeEnum Type { get; }
    }
}
