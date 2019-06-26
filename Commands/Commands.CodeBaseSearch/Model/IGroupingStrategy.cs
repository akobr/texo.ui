namespace Commands.CodeBaseSearch.Model
{
    public interface IGroupingStrategy
    {
        string GetGroup(ISubject subject);
    }
}
