using System.Threading.Tasks;

namespace ConsoleLockAccountDemo
{
    static class Program
    {
        private static async Task Main()
        {
            await AccountDemo.Run();
        }
    }
}