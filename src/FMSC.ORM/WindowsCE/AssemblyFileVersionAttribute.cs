#if WindowsCE

namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    internal sealed class AssemblyFileVersionAttribute : Attribute
    {
        private String _version;

        public AssemblyFileVersionAttribute(String version)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            _version = version;
        }

        public String Version
        {
            get { return _version; }
        }
    }
}
#endif