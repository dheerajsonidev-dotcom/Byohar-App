namespace Byohar.Application.Interfaces.Common;

public interface IBuildingService
{
    Task CheckBuildingFireCertificateExpiration(DateTime date);
}
