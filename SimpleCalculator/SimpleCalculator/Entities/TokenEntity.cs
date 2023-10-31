using SimpleCalculator.Types;

namespace SimpleCalculator.Entities
{
    public class TokenEntity
    {
        public string Content { get; set; }
        public TokenTypes Type { get; set; }

        public TokenEntity(string content, TokenTypes type)
        {
            Content = content;
            Type = type;
        }   
    }
}
