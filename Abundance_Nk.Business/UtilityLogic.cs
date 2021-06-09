using System;
using System.Linq;
using System.Text;

namespace Abundance_Nk.Business
{
    public class UtilityLogic
    {
        public static string PaddNumber(long id, int maxCount)
        {
            try
            {
                string idInString = id.ToString();
                string paddNumbers = "";
                if (idInString.Count() < maxCount)
                {
                    int zeroCount = maxCount - id.ToString().Count();
                    var builder = new StringBuilder();
                    for (int counter = 0; counter < zeroCount; counter++)
                    {
                        builder.Append("0");
                    }

                    builder.Append(id);
                    paddNumbers = builder.ToString();
                    return paddNumbers;
                }
                paddNumbers = id.ToString();

                return paddNumbers;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}