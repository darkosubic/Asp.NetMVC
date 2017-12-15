namespace DarkoMVCApp.Helpers
{
    public static class Sorting
    {
        public static bool? UpgradeSortOrder(bool? test)
        {
            bool? direction = null;
            
            if (test == null )
                direction = true;

            else if (test == true)
                direction = false;

            else if (test == false)
                direction = null;

            return direction;
        }
    }
}