
namespace BLL.HangfireTasks
{
    public interface IHangfireTask
    {
        string CronExpressions { get; }
        void Exexute();
    }
}
