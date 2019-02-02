namespace Lofy.Modules.Core.Autorize
{
  public interface IAuthorize
  {
    bool Login(string username, string password);
  }
}