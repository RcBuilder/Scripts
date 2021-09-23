using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Entities
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
