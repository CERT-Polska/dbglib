using DbgLib.NativeApi;
using System;
using System.Collections;
using System.Linq;

namespace DbgLib
{
    public sealed class MetadataType
    {
        private readonly IMetadataImport importer;
        private readonly int typeToken;

        internal MetadataType(IMetadataImport importer, int typeToken)
        {
            this.typeToken = typeToken;
            this.importer = importer;
        }

        public MetadataMethodInfo[] GetMethods()
        {
            IntPtr hEnum = new IntPtr();
            ArrayList methods = new ArrayList();

            int methodToken;
            try
            {
                while (true)
                {
                    int size;
                    importer.EnumMethods(ref hEnum, typeToken, out methodToken, 1, out size);
                    if (size == 0)
                        break;
                    methods.Add(new MetadataMethodInfo(importer, methodToken));
                }
            }
            finally
            {
                importer.CloseEnum(hEnum);
            }
            return (MetadataMethodInfo[])methods.ToArray(typeof(MetadataMethodInfo));
        }

        public MetadataMethodInfo GetFunctionInfoFromName(string name)
        {
            return GetMethods().FirstOrDefault(mi => mi.Name == name);
        }

        public ICorDebugFunction GetFunctionFromName(ICorDebugModule module, string name)
        {
            var mi = GetFunctionInfoFromName(name);
            return mi != null ? mi.ToCorFunction(module) : null;
        }
    }
}
