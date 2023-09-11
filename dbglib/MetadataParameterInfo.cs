using DbgLib.NativeApi;
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace DbgLib
{
    public sealed class MetadataParameterInfo
    {
        public MetadataParameterInfo(IMetadataImport importer, int paramToken,
                                       MetadataMethodInfo memberImpl, MetadataType typeImpl)
        {
            int parentToken;
            uint pulSequence, pdwAttr, pdwCPlusTypeFlag, pcchValue, size;

            IntPtr ppValue;
            importer.GetParamProps(paramToken,
                                   out parentToken,
                                   out pulSequence,
                                   null,
                                   0,
                                   out size,
                                   out pdwAttr,
                                   out pdwCPlusTypeFlag,
                                   out ppValue,
                                   out pcchValue
                                   );
            StringBuilder szName = new StringBuilder((int)size);
            importer.GetParamProps(paramToken,
                                   out parentToken,
                                   out pulSequence,
                                   szName,
                                   (uint)szName.Capacity,
                                   out size,
                                   out pdwAttr,
                                   out pdwCPlusTypeFlag,
                                   out ppValue,
                                   out pcchValue
                                   );

            Name = szName.ToString();
            Type = typeImpl;
            Position = (int)pulSequence;
            Attributes = (ParameterAttributes)pdwAttr;
            Method = memberImpl;
        }

        private MetadataParameterInfo(SerializationInfo info, StreamingContext context)
        {

        }

        public string Name { get; private set; }

        public int Position { get; private set; }

        public MetadataType Type { get; private set; }

        public MetadataMethodInfo Method { get; private set; }

        public ParameterAttributes Attributes { get; private set; }
    }
}
