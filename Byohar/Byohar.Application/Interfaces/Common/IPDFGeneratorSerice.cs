namespace Byohar.Application.Interfaces.Common
{
    public interface IPDFGeneratorSerice
    {
        byte[] GeneratePdf<T>(string heading, List<T> data);
    }
}
