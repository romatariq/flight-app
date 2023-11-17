namespace Base.Helpers;

public static class EFHelpers
{
    public static int GetPageCount(int count, int pageSize)
    {
        return count / pageSize + (count % pageSize == 0 && count >= pageSize ? 0 : 1);
    }
    
}