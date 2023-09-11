using System;
using System.Collections;
using System.Text;
using DbgLib.NativeApi;

namespace DbgLib
{
    public sealed class MetadataMethodInfo
    {
        private readonly int methodToken;
        private readonly int classToken;
        private readonly IMetadataImport importer;
        private readonly string p_name;

        public MetadataMethodInfo(IMetadataImport importer, int methodToken)
        {
            this.importer = importer;
            this.methodToken = methodToken;

            int size;
            uint pdwAttr;
            IntPtr ppvSigBlob;
            uint pulCodeRVA, pdwImplFlags;
            uint pcbSigBlob;

            importer.GetMethodProps((uint)methodToken,
                                      out classToken,
                                      null,
                                      0,
                                      out size,
                                      out pdwAttr,
                                      out ppvSigBlob,
                                      out pcbSigBlob,
                                      out pulCodeRVA,
                                      out pdwImplFlags);

            StringBuilder szMethodName = new StringBuilder(size);
            importer.GetMethodProps((uint)methodToken,
                                    out classToken,
                                    szMethodName,
                                    szMethodName.Capacity,
                                    out size,
                                    out pdwAttr,
                                    out ppvSigBlob,
                                    out pcbSigBlob,
                                    out pulCodeRVA,
                                    out pdwImplFlags);

            p_name = szMethodName.ToString();
        }

        public string Name
        {
            get { return p_name; }
        }

        public int MetadataToken
        {
            get { return this.methodToken; }
        }

        public MetadataType DeclaringType
        {
            get
            {
                if (TokenUtils.IsNullToken(classToken))
                    return null;

                return new MetadataType(importer, classToken);
            }
        }

        public MetadataParameterInfo[] GetParameters()
        {
            ArrayList al = new ArrayList();
            IntPtr hEnum = new IntPtr();
            try
            {
                while (true)
                {
                    uint count;
                    int paramToken;
                    importer.EnumParams(ref hEnum,
                                          methodToken, out paramToken, 1, out count);
                    if (count != 1)
                        break;
                    al.Add(new MetadataParameterInfo(importer, paramToken,
                                                     this, DeclaringType));
                }
            }
            finally
            {
                importer.CloseEnum(hEnum);
            }
            return (MetadataParameterInfo[])al.ToArray(typeof(MetadataParameterInfo));
        }

        public ICorDebugFunction ToCorFunction(ICorDebugModule module)
        {
            return module.GetFunctionFromToken(MetadataToken);
        }
    }
}
