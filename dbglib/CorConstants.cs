﻿namespace DbgLib
{
    internal static class CorConstants
    {
        public const int TokenNotFound = -1;
        public const int TokenGlobalNamespace = 0;
    }

    public enum CorTokenType
    {
        mdtModule = 0x00000000,
        mdtTypeRef = 0x01000000,
        mdtTypeDef = 0x02000000,
        mdtFieldDef = 0x04000000,
        mdtMethodDef = 0x06000000,
        mdtParamDef = 0x08000000,
        mdtInterfaceImpl = 0x09000000,
        mdtMemberRef = 0x0a000000,
        mdtCustomAttribute = 0x0c000000,
        mdtPermission = 0x0e000000,
        mdtSignature = 0x11000000,
        mdtEvent = 0x14000000,
        mdtProperty = 0x17000000,
        mdtModuleRef = 0x1a000000,
        mdtTypeSpec = 0x1b000000,
        mdtAssembly = 0x20000000,
        mdtAssemblyRef = 0x23000000,
        mdtFile = 0x26000000,
        mdtExportedType = 0x27000000,
        mdtManifestResource = 0x28000000,
        mdtGenericParam = 0x2a000000,
        mdtMethodSpec = 0x2b000000,
        mdtGenericParamConstraint = 0x2c000000,

        mdtString = 0x70000000,
        mdtName = 0x71000000,
        mdtBaseType = 0x72000000,       // Leave this on the high end value. This does not correspond to metadata table
    }

    public static class TokenUtils
    {
        public static CorTokenType TypeFromToken(int token)
        {
            return (CorTokenType)((uint)token & 0xff000000);
        }

        public static int RidFromToken(int token)
        {
            return (int)((uint)token & 0x00ffffff);
        }

        public static bool IsNullToken(int token)
        {
            return (RidFromToken(token) == 0);
        }
    }
}
