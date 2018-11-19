namespace Commands.CodeBaseSearch.Model
{
    public interface ISubject
    {
        string Title { get; }

        string Path { get; }

        SubjectTypeEnum Type { get; }


    }
}
