
using System.Threading.Tasks;

namespace BLL
{
    public interface IProcess
    {
        bool IsRunning { get; }
        void Run();
    }

    public interface IProcessAsync : IProcess
    {
        Task RunAsync();
    }
}
