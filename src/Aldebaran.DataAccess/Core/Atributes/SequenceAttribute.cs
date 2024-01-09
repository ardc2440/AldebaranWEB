namespace Aldebaran.DataAccess.Core.Atributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SequenceAttribute : Attribute
    {
        public int Length { get; }

        public SequenceAttribute(int length)
        {
            Length = length;
        }
    }
}
