namespace Byohar.Application.Interfaces.Common
{
    public interface IWorkOrderService
    {
        Task CheckOverDueWorkOrder(DateTime date);
    }
}
