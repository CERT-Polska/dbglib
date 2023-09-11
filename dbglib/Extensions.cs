using DbgLib.NativeApi;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DbgLib
{
    public static class CorThread
    {
        public static ICorDebugFrame GetActiveFrame(this ICorDebugThread cothread)
        {
            ICorDebugFrame coframe;
            cothread.GetActiveFrame(out coframe);
            return coframe;
        }
    }

    public static class CorFrame
    {
        public static ICorDebugILFrame GetILFrame(this ICorDebugFrame coframe)
        {
            return coframe as ICorDebugILFrame;
        }

        public static void GetIP(this ICorDebugFrame coframe, out uint offset, out CorDebugMappingResult mappingResult)
        {
            ICorDebugILFrame ilframe = coframe.GetILFrame();
            if (ilframe == null)
            {
                offset = 0;
                mappingResult = CorDebugMappingResult.MAPPING_NO_INFO;
            }
            else
            {
                ilframe.GetIP(out offset, out mappingResult);
            }
        }

        public static ICorDebugFunction GetFunction(this ICorDebugFrame coframe)
        {
            ICorDebugFunction cofunc;
            coframe.GetFunction(out cofunc);
            return cofunc;
        }

        public static uint GetFunctionToken(this ICorDebugFrame coframe)
        {
            uint token;
            coframe.GetFunctionToken(out token);
            return token;
        }
    }
    
    public static class CorAppDomain
    {
        public static ICorDebugProcess GetProcess(this ICorDebugAppDomain appDomain)
        {
            ICorDebugProcess proc;
            appDomain.GetProcess(out proc);
            return proc != null ? DebugProcess.For(proc).Process : null;
        }
    }

    public static class CorBreakpoint
    {
        public static void Activate(this ICorDebugBreakpoint cobreakpoint, bool active)
        {
            cobreakpoint.Activate(active ? 1 : 0);
        }

        public static bool IsActive(this ICorDebugBreakpoint cobreakpoint)
        {
            int active;
            cobreakpoint.IsActive(out active);
            return active != 0;
        }
    }

    public static class CorFunctionBreakpoint
    {
        public static ICorDebugFunction GetFunction(this ICorDebugFunctionBreakpoint p_cobreakpoint)
        {
            ICorDebugFunction cofunc;
            p_cobreakpoint.GetFunction(out cofunc);
            return cofunc;
        }

        public static uint GetOffset(this ICorDebugFunctionBreakpoint p_cobreakpoint)
        {
            uint offset;
            p_cobreakpoint.GetOffset(out offset);
            return offset;
        }
    }

    public static class CorCode
    {
        public static ICorDebugFunctionBreakpoint CreateBreakpoint(this ICorDebugCode cocode, int iloffset)
        {
            ICorDebugFunctionBreakpoint cobreak;
            cocode.CreateBreakpoint((uint)iloffset, out cobreak);

            return cobreak;
        }
    }
    public static class CorFunction
    {
        public static ICorDebugFunctionBreakpoint CreateBreakpoint(this ICorDebugFunction cofunc)
        {
            //return cofunc.CreateBreakpoint();
            return cofunc.GetILCode().CreateBreakpoint(0);
        }

        public static ICorDebugModule GetModule(this ICorDebugFunction cofunc)
        {
            ICorDebugModule comodule;
            cofunc.GetModule(out comodule);
            return comodule;
        }

        public static uint GetToken(this ICorDebugFunction cofunc)
        {
            uint token;
            cofunc.GetToken(out token);
            return token;
        }

        public static uint GetVersionNumber(this ICorDebugFunction cofunc)
        {
            uint version;
            ((ICorDebugFunction2)cofunc).GetVersionNumber(out version);
            return version;
        }

        public static ICorDebugCode GetILCode(this ICorDebugFunction cofunc)
        {
            ICorDebugCode cocode = null;
            cofunc.GetILCode(out cocode);
            return cocode;
        }
    }

    public static class CorModule
    {
        public static ICorDebugProcess GetProcess(this ICorDebugModule comodule)
        {
            ICorDebugProcess coproc;
            comodule.GetProcess(out coproc);
            return coproc;
        }

        public static ICorDebugFunction GetFunctionFromToken(this ICorDebugModule comodule, int token)
        {
            ICorDebugFunction cofunc;
            comodule.GetFunctionFromToken((uint)token, out cofunc);
            return cofunc;
        }

        public static string GetName(this ICorDebugModule comodule)
        {
            char[] name = new char[300];
            uint fetched;
            comodule.GetName((uint)name.Length, out fetched, name);

            // fetched - 1 because of the ending 0
            return new string(name, 0, (int)fetched - 1);
        }

        public static T GetMetadataInterface<T>(this ICorDebugModule comodule)
        {
            object res;
            Guid guid = typeof(T).GUID;
            comodule.GetMetaDataInterface(ref guid, out res);
            return (T)res;
        }

        public static int GetTypeTokenFromName(this ICorDebugModule comodule, string name)
        {
            IMetadataImport importer = comodule.GetMetadataInterface<IMetadataImport>();

            int token;
            if (name.Length == 0)
            {
                token = CorConstants.TokenGlobalNamespace;
            }
            else
            {
                try
                {
                    importer.FindTypeDefByName(name, 0, out token);
                }
                catch (COMException e)
                {
                    token = CorConstants.TokenNotFound;
                    if ((HResult)e.ErrorCode == HResult.CLDB_E_RECORD_NOTFOUND)
                    {
                        int i = name.LastIndexOf('.');
                        if (i > 0)
                        {
                            int parentToken = comodule.GetTypeTokenFromName(name.Substring(0, i));
                            if (parentToken != CorConstants.TokenNotFound)
                            {
                                try
                                {
                                    importer.FindTypeDefByName(name.Substring(i + 1), parentToken, out token);
                                }
                                catch (COMException e2)
                                {
                                    token = CorConstants.TokenNotFound;
                                    if ((HResult)e2.ErrorCode != HResult.CLDB_E_RECORD_NOTFOUND)
                                        throw;
                                }
                            }
                        }
                    }
                    else
                        throw;
                }
            }
            return token;
        }

        public static MetadataType GetTypeFromName(this ICorDebugModule module, string name)
        {
            int typeToken = module.GetTypeTokenFromName(name);
            if (typeToken == CorConstants.TokenNotFound)
                return null;
            return new MetadataType(module.GetMetadataInterface<IMetadataImport>(), typeToken);
        }

        public static ICorDebugFunction ResolveFunction(this ICorDebugModule module, string className, string functionName)
        {
            MetadataType t = module.GetTypeFromName(className);
            if (t == null) { return null; }

            return t.GetFunctionFromName(module, functionName);
        }
    }

    public static class CorProcess
    {
        public static int GetId(this ICorDebugProcess coprocess)
        {
            uint id;
            coprocess.GetID(out id);
            return (int)id;
        }
    }
    
    public static class CorIlFrame
    {
        public static ICorDebugValue GetArgument(this ICorDebugILFrame coframe, uint dwIndex)
        {
            ICorDebugValue ppValue;
            coframe.GetArgument(dwIndex, out ppValue);
            return ppValue;
        }
    }

    public static class CorValue
    {
        public static ulong GetAddress(this ICorDebugValue value)
        {
            ulong addr;
            value.GetAddress(out addr);
            return addr;
        }

        public static ICorDebugStringValue AsString(this ICorDebugValue value)
        {
            return (ICorDebugStringValue)value.Resolve();
        }

        /// <summary>
        /// For some reason - unknown to me - corvalue may be a reference that
        /// we have to resolve first. No-op for non references.
        /// </summary>
        public static ICorDebugValue Resolve(this ICorDebugValue value)
        {
            while (true)
            {
                ICorDebugReferenceValue refv = value as ICorDebugReferenceValue;
                if (refv != null)
                {
                    refv.Dereference(out value);
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// This is a simplified type ID, not a real type token. See:
        /// https://learn.microsoft.com/en-us/dotnet/framework/unmanaged-api/metadata/corelementtype-enumeration
        /// </summary>
        public static uint GetTypeID(this ICorDebugValue value)
        {
            uint pType;
            value.GetType(out pType);
            return pType;
        }
    }

    public static class CorStringValue
    {
        public static uint Length(this ICorDebugStringValue corstring) {
            uint len;
            corstring.GetLength(out len);
            return len;
        }

        public static string GetStringValue(this ICorDebugStringValue corstring)
        {
            uint stringSize;
            StringBuilder sb = new StringBuilder((int)corstring.Length() + 1);
            corstring.GetString((uint)sb.Capacity, out stringSize, sb);
            return sb.ToString();
        }
    }

    public static class CorDebug
    {
        public static ICorDebugProcess CreateProcess(this ICorDebug codebugger, string exepath)
        {
            STARTUPINFO si = new STARTUPINFO();
            si.cb = Marshal.SizeOf(si);

            // initialize safe handles 
            si.hStdInput = new Microsoft.Win32.SafeHandles.SafeFileHandle(new IntPtr(0), false);
            si.hStdOutput = new Microsoft.Win32.SafeHandles.SafeFileHandle(new IntPtr(0), false);
            si.hStdError = new Microsoft.Win32.SafeHandles.SafeFileHandle(new IntPtr(0), false);

            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            ICorDebugProcess proc;
            codebugger.CreateProcess(
                                null,
                                exepath,
                                null,
                                null,
                                0, // inherit handles
                                (uint)CreateProcessFlags.CREATE_NEW_CONSOLE,
                                new IntPtr(0),
                                null,
                                si,
                                pi,
                                CorDebugCreateProcessFlags.DEBUG_NO_SPECIAL_OPTIONS,
                                out proc);
            return proc;
        }

        public static ICorDebugProcess DebugActiveProcess(this ICorDebug codebugger, int pid, bool win32Attach = false)
        {
            ICorDebugProcess coproc;
            codebugger.DebugActiveProcess(Convert.ToUInt32(pid), win32Attach ? 1 : 0, out coproc);
            return coproc;
        }
    }
}
