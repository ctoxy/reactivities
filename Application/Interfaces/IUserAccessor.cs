// permet d'appeller la méthode GetUsername pour  connaitre qui a crée une activité par exemple voir Infrastructure security UserAccessor
namespace Application.Interfaces
{
    public interface IUserAccessor
    {
        string GetUsername();
    }
}