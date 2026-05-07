
using Byohar.Shared.Wrapper;

namespace Byohar.Application.Interfaces.Common;
public interface IDuplicateRecordsForBulkService
{
    Task<bool> CheckSiteCode(string code);
    Task<bool> CheckSupplierCode(string code);
    Task<bool> CheckStockPartCode(string code);
    Task<bool> CheckTechnicianIdNumber(string code);
  
}
