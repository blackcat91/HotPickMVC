using HotPickMVC.Models;

namespace HotPickMVC.Data.Users
{
    public interface IUserData
    {
        Task<dynamic> ChangePassword(ChangePasswordModel? pwd);
        Task<UserModel> CreateUser(UserModel userNfo);
        Task<UserModel> DeleteUser(UserModel user);
        Task<UserModel> GetUser(string? id);
        Task<UserModel> LogIn(LogInModel logIn);
        Task<Task> LogOut(string id);
        Task<UserModel> UpdateUser(UserModel newUser);
    }
}