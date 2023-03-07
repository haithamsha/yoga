namespace yoga.Controllers
{
    public static class GlobalHelpers
    {
        public static string getTeachingType(int typeId)
        {
            if (typeId == 1)
            {
                return "Yin";
            }
            else if (typeId == 2)
            {
                return "Prenatal";
            }
            else if (typeId == 3)
            {
                return "Therapy";
            }
            else if (typeId == 4)
            {
                return "Aerial";
            }
            else if (typeId == 5)
            {
                return "Hatha";
            }
            else if (typeId == 6)
            {
                return "Ashtanga";
            }
            else if (typeId == 7)
            {
                return "Vinyasa Flow";
            }
            else if (typeId == 8)
            {
                return "Iyengar";
            }
            else
            {
                return "";
            }
        }

    }
}