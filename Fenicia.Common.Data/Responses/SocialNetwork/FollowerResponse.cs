namespace Fenicia.Common.Data.Responses.SocialNetwork;

public class FollowerResponse
{
    public Guid Id
    {
        get;
        set;
    }

    public Guid UserId
    {
        get;
        set;
    }

    public Guid FollowerId
    {
        get;
        set;
    }

    public DateTime FollowDate
    {
        get;
        set;
    }

    public bool IsActive
    {
        get;
        set;
    }
}
