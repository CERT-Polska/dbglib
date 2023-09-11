using System.Collections.Generic;
using System.IO;
using DbgLib.NativeApi;

namespace DbgLib
{
    public class DebugProcess
    {
        private static readonly Dictionary<ICorDebugProcess, DebugProcess> processes = new Dictionary<ICorDebugProcess, DebugProcess>();
        private ICorDebugProcess process;

        public DebugProcess(ICorDebugProcess process)
        {
            this.process = process;
        }

        private HashSet<ICorDebugModule> modules = new HashSet<ICorDebugModule>();

        public IEnumerable<ICorDebugModule> Modules
        {
            get { return modules; }
        }

        public ICorDebugProcess Process
        {
            get { return process; }
        }

        public static DebugProcess For(ICorDebugProcess coproc)
        {
            lock (processes)
            {
                DebugProcess proc;
                processes.TryGetValue(coproc, out proc);
                if (proc == null)
                {
                    proc = new DebugProcess(coproc);
                    processes.Add(coproc, proc);
                }
                return proc;
            }
        }

        public ICorDebugModule GetModuleByName(string moduleName)
        {
            foreach (ICorDebugModule m in Modules)
            {
                string mn = Path.GetFileName(m.GetName());
                if (mn == moduleName) { return m; }
            }
            return null;
        }

        public ICorDebugFunction ResolveFunction(string moduleName, string className, string functionName)
        {
            ICorDebugModule module = GetModuleByName(moduleName);
            if (module == null) { return null; }

            return module.ResolveFunction(className, functionName);
        }

        public void OnModuleLoad(ICorDebugModule module)
        {
            modules.Add(module);
        }
    }
}
