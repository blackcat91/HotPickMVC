using HotPickMVC.Models;
using MongoDB.Driver;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

namespace HotPickMVC.Data.Users
{
    public class UserData : IUserData
    {
        private readonly MongoDBAccess _db;
        private readonly IConfiguration _config;
        private const string UserCollection = "users";


        public UserData(MongoDBAccess db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<UserModel> CreateUser(UserModel userNfo)
        {
            var userCol = _db.ConnectToMongo<UserModel>(UserCollection);
            var filter = Builders<UserModel>.Filter.Eq("Email", userNfo.Email);
            var exist = await userCol.FindAsync(filter);
            if (exist.Any())
            {
                var error = new UserModel();
                error.Error = "User Already Exists";
                return error;
            }
            userNfo.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userNfo.Password);
            await userCol.InsertOneAsync(userNfo);
            return userNfo;

        }

        public async Task<UserModel> UpdateUser(UserModel newUser)
        {
            try
            {
                var userCol = _db.ConnectToMongo<UserModel>(UserCollection);
                var u = await userCol.Find(n => n.Id == newUser.Id).FirstAsync();
                u.UserName = newUser.UserName;
                u.Email = newUser.Email;
                var filter = Builders<UserModel>.Filter.Eq("Id", u.Id);
                await userCol.ReplaceOneAsync(filter, u, new ReplaceOptions { IsUpsert=true });
                return u;
            }
            catch (Exception e)
            {
                var error = new UserModel();
                error.Error = e.Message;
                return error;
            }

        }

        public async Task<UserModel> DeleteUser(UserModel user)
        {
            try
            {
                var userCol = _db.ConnectToMongo<UserModel>(UserCollection);
                return await userCol.FindOneAndDeleteAsync(n => n.Id == user.Id);
            }
            catch (Exception e)
            {
                var error = new UserModel();
                error.Error = e.Message;
                return error;
            }
        }


        public async Task<UserModel> GetUser(string? id)
        {
            var userCol = _db.ConnectToMongo<UserModel>(UserCollection);

            if (id == null)
            {
                var error = new UserModel();
                error.Error = "Please Pass Data!";
                return error;

            }
            var u = userCol.Find(u => u.Id == id);
            if (!u.Any())
            {
                var error = new UserModel();
                error.Error = "User Not Found!";
                return error;
            }

            var uData = await u.FirstOrDefaultAsync();
            uData.Password = null;
            uData.Jwt = null;

            return uData;


        }
        public async Task<UserModel> LogIn(LogInModel logIn)
        {

            var userCol = _db.ConnectToMongo<UserModel>(UserCollection);
            if (logIn == null)
            {
                var error = new UserModel();
                error.Error = "Please pass some data";
                return error;
            }
            var filter = Builders<UserModel>.Filter.Eq("Email", logIn.Email);
            var user = userCol.Find(filter);
            if (!user.Any())
            {
                var error = new UserModel();
                error.Error = "User Not Found!";
                return error;
            }

            var userData = await user.FirstAsync();
            var correctPass = BCrypt.Net.BCrypt.EnhancedVerify(logIn.Password, userData.Password);

            if (!correctPass)
            {
                var error = new UserModel();
                error.Error = "Incorrect Credentials!";
                return error;
            }

            List<Claim> authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userData.UserName),
                    new Claim(ClaimTypes.Email, userData.Email)
                };

            JwtSecurityToken token = GetToken(authClaims);
            userData.Jwt.Id = userData.Id;
            userData.Jwt.Expiry = token.ValidTo;
            userData.Jwt.Token = new JwtSecurityTokenHandler().WriteToken(token);
            var jwt = JsonConvert.SerializeObject(userData.Jwt);
            await userCol.FindOneAndReplaceAsync(filter, userData);
            StreamWriter sw = new StreamWriter(Path.GetTempPath() + "token.json", true, Encoding.UTF8);
            sw.Write(jwt);
            sw.Close();
            return userData;
        }

        public async Task<dynamic> ChangePassword(ChangePasswordModel? pwd)
        {
            var error = new UserModel();
            var userCol = _db.ConnectToMongo<UserModel>(UserCollection);
            if (pwd.CurrentPassword == null)
            {
          
                error.Error = "Please pass some data";
                return error;
            }
            var filter = Builders<UserModel>.Filter.Eq("Id", pwd.Id);
            var user = userCol.Find(filter);
            if (!user.Any())
            {
               
                error.Error = "User Not Found!";
                return error;
            }

            var userData = await user.FirstAsync();
            var correctPass = BCrypt.Net.BCrypt.EnhancedVerify(pwd.CurrentPassword, userData.Password);

            if (!correctPass)
            {
                
                error.Error = "Incorrect Credentials!";
                return error;
            }
            if(pwd.NewPassword == pwd.ConfirmNewPassword)
            {
                userData.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(pwd.NewPassword);
                await userCol.FindOneAndReplaceAsync(filter, userData);
                return 1;
            }
            error.Error = "Passwords Need to Match";
            return error;
            
        }


        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }


        public async Task<Task> LogOut(string id)
        {

            var userCol = _db.ConnectToMongo<UserModel>(UserCollection);
            var user = await userCol.Find(u => u.Id == id).FirstAsync();
            user.Jwt = new Jwt();
            var filter = Builders<UserModel>.Filter.Eq("Id", user.Id);
            File.Delete(Path.GetTempPath() + "token.json");
            return userCol.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
        }
    }
}
