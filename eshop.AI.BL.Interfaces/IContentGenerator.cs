namespace eshop.AI.BL.Interfaces
{
    public interface IContentGenerator
    {
        string GenerateDescriptionFromSpecification(string requestMessage, string specification);
    }
}
