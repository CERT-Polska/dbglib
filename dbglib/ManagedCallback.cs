using DbgLib.NativeApi;
using System;

namespace DbgLib
{
    public class ManagedCallback : ICorDebugManagedCallback, ICorDebugManagedCallback2
    {
        bool debugMode;
        public ManagedCallback(bool debugMode = false)
        {
            this.debugMode = debugMode;
        }

        private void DebugLog(string message)
        {
            if (debugMode)
            {
                Console.WriteLine(message);
            }
        }

        public virtual void Breakpoint(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint pBreakpoint)
        {
            DebugLog("Breakpoint event");
            pAppDomain.Continue(0);
        }

        public virtual void StepComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugStepper pStepper, CorDebugStepReason reason)
        {
            DebugLog("StepComplete event");
            pAppDomain.Continue(0);
        }

        public virtual void Break(ICorDebugAppDomain pAppDomain, ICorDebugThread thread)
        {
            DebugLog("Break event");
            pAppDomain.Continue(0);
        }

        public virtual void Exception(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int unhandled)
        {
            DebugLog("Exception event");
            pAppDomain.Continue(0);
        }

        public virtual void EvalComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval pEval)
        {
            DebugLog("EvalComplete event");
            pAppDomain.Continue(0);
        }

        public virtual void EvalException(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval pEval)
        {
            DebugLog("EvalException event");
            pAppDomain.Continue(0);
        }

        public virtual void CreateProcess(ICorDebugProcess pProcess)
        {
            DebugLog("CreateProcess event");
            pProcess.Continue(0);
        }

        public virtual void ExitProcess(ICorDebugProcess pProcess)
        {
            DebugLog("ExitProcess event");
            pProcess.Continue(0);
        }

        public virtual void CreateThread(ICorDebugAppDomain pAppDomain, ICorDebugThread thread)
        {
            DebugLog("CreateThread event");
            pAppDomain.Continue(0);
        }

        public virtual void ExitThread(ICorDebugAppDomain pAppDomain, ICorDebugThread thread)
        {
            DebugLog("ExitThread event");
            pAppDomain.Continue(0);
        }

        public virtual void LoadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
        {
            DebugLog("LoadModule event");
            DebugProcess.For(pAppDomain.GetProcess()).OnModuleLoad(pModule);
            pAppDomain.Continue(0);
        }

        public virtual void UnloadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
        {
            DebugLog("UnloadModule event");
            pAppDomain.Continue(0);
        }

        public virtual void LoadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
        {
            DebugLog("LoadClass event");
            pAppDomain.Continue(0);
        }

        public virtual void UnloadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
        {
            DebugLog("UnloadClass event");
            pAppDomain.Continue(0);
        }

        public virtual void DebuggerError(ICorDebugProcess pProcess, int errorHR, uint errorCode)
        {
            DebugLog("DebuggerError event");
            pProcess.Continue(0);
        }

        public virtual void LogMessage(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, string pLogSwitchName, string pMessage)
        {
            DebugLog("LogMessage event");
            pAppDomain.Continue(0);
        }

        public virtual void LogSwitch(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, uint ulReason, string pLogSwitchName, string pParentName)
        {
            DebugLog("LogSwitch event");
            pAppDomain.Continue(0);
        }

        public virtual void CreateAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
        {
            DebugLog("CreateAppDomain event");
            pAppDomain.Attach();
            pAppDomain.Continue(0);
        }

        public virtual void ExitAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
        {
            DebugLog("ExitAppDomain event");
            pAppDomain.Continue(0);
        }

        public virtual void LoadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
        {
            DebugLog("LoadAssembly event");
            pAppDomain.Continue(0);
        }

        public virtual void UnloadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
        {
            DebugLog("UnloadAssembly event");
            pAppDomain.Continue(0);
        }

        public virtual void ControlCTrap(ICorDebugProcess pProcess)
        {
            DebugLog("ControlCTrap event");
            pProcess.Continue(0);
        }

        public virtual void NameChange(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
        {
            DebugLog("NameChange event");
            pAppDomain.Continue(0);
        }

        public virtual void UpdateModuleSymbols(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule, System.Runtime.InteropServices.ComTypes.IStream pSymbolStream)
        {
            DebugLog("UpdateModuleSymbols event");
            pAppDomain.Continue(0);
        }

        public virtual void EditAndContinueRemap(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction, int fAccurate)
        {
            // TODO HandleEvent(ManagedCallbackType.On new CorEventArgs(new CorAppDomain(pAppDomain)));
        }

        public virtual void BreakpointSetError(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint pBreakpoint, uint dwError)
        {
            DebugLog("BreakpointSetError event");
            pAppDomain.Continue(0);
        }

        public virtual void FunctionRemapOpportunity(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pOldFunction, ICorDebugFunction pNewFunction, uint oldILOffset)
        {
            DebugLog("FunctionRemapOpportunity event");
            pAppDomain.Continue(0);
        }

        public virtual void CreateConnection(ICorDebugProcess pProcess, uint dwConnectionId, ref ushort pConnName)
        {
            // TODO HandleEvent(ManagedCallbackType.On new CorEventArgs(new CorProcess(pProcess)));
        }

        public virtual void ChangeConnection(ICorDebugProcess pProcess, uint dwConnectionId)
        {
            // TODO HandleEvent(ManagedCallbackType.oncha new CorEventArgs(new CorProcess(pProcess)));
        }

        public virtual void DestroyConnection(ICorDebugProcess pProcess, uint dwConnectionId)
        {
            // TODO HandleEvent(new CorEventArgs(new CorProcess(pProcess)));
        }

        public virtual void Exception(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFrame pFrame, uint nOffset, CorDebugExceptionCallbackType dwEventType, uint dwFlags)
        {
            DebugLog("Exception event");
            pAppDomain.Continue(0);
        }

        public virtual void ExceptionUnwind(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, CorDebugExceptionUnwindCallbackType dwEventType, uint dwFlags)
        {
            DebugLog("ExceptionUnwind event");
            pAppDomain.Continue(0);
        }

        public virtual void FunctionRemapComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction)
        {
            // TODO HandleEvent(<new CorEventArgs(new CorAppDomain(pAppDomain)));
        }

        public virtual void MDANotification(ICorDebugController pController, ICorDebugThread pThread, ICorDebugMDA pMDA)
        {
            // TODO nothing for now
        }
    }
}
