using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Diverse
{
    public static class MethodCapture
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static MethodBase CaptureCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            var method = sf.GetMethod();

            return method;
        }
    }
}