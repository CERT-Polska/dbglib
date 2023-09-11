using DbgLib;
using DbgLib.NativeApi;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace dotrunpex_dump
{
    public class UnpackerCallback : ManagedCallback
    {
        string filename;

        public UnpackerCallback(string filename) : base(true)
        {
            this.filename = filename;
        }

        public override void Breakpoint(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint pBreakpoint)
        {
            Console.WriteLine("Breakpoint hit!");
            ICorDebugValue value = pThread.GetActiveFrame().GetILFrame().GetArgument(0);
            string keyCandidate = value.AsString().GetStringValue();
            Console.WriteLine("Parameter: {0}", keyCandidate);
            if (keyCandidate.EndsWith("="))
            {
                File.WriteAllText(filename + ".key.txt", keyCandidate);
                pAppDomain.GetProcess().Terminate(0);
                Process.GetCurrentProcess().Kill();
            }
            base.Breakpoint(pAppDomain, pThread, pBreakpoint);
        }

        public override void LoadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
        {
            var func = pModule.ResolveFunction("System.Convert", "FromBase64String");
            if (func != null)
            {
                Console.WriteLine("Time to add my breakpoint, found {0}!", func);
                func.CreateBreakpoint();
                Console.WriteLine("Ok, hopefully done.");
            }
            base.LoadModule(pAppDomain, pModule);
        }

        public override void ExitProcess(ICorDebugProcess pProcess)
        {
            Console.WriteLine("Exit Process");
            Process.GetCurrentProcess().Kill();
            base.ExitProcess(pProcess);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var callback = new UnpackerCallback(args[0]);
            var debugger = DebuggerManager.Create(callback);
            debugger.CreateProcess(args[0]);

            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine("waiting {0}...", i);
                Thread.Sleep(1000);
            }
        }
    }
}
