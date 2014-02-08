using Yintai.Hangzhou.Data.Models;

namespace com.intime.fashion.data.sync
{
    public class DbContextHelper
    {
        public static YintaiHangzhouContext GetDbContext()
        {
            return new YintaiHangzhouContext("YintaiHangzhouContext");
        }
    }
}
