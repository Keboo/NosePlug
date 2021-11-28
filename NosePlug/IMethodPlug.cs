namespace NosePlug;

public interface IBaseMethodPlug
{

}

public partial interface IMethodPlug : IBaseMethodPlug
{
    IMethodPlug CallOriginal(bool shouldCallOriginal = true);
}

public partial interface IMethodPlug<TReturn> : IBaseMethodPlug
{
    IMethodPlug<TReturn> CallOriginal(bool shouldCallOriginal = true);
}
