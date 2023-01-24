using log4net;
using System.Reflection;

namespace Common
{
    public class Common
    {
        readonly static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


    }
}