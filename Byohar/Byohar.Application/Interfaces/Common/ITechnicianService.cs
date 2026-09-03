namespace Byohar.Application.Interfaces.Common;

public interface ITechnicianService
{
    Task CheckTechnicianQualification(DateTime date);
    Task CheckTechnicianFinishDate(DateTime date);
}
