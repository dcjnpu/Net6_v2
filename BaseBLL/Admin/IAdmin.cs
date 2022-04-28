namespace BaseBLL
{
    public interface IAdmin
    {
        TAdmin Get();

        void Set(TAdmin tadmin, int expireHours = 8);

        bool IsLogin();

        void LoginOut();
    }
}