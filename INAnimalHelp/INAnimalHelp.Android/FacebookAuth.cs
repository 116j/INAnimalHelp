
using Android.Content;
using INAnimalHelp.Droid;
using INAnimalHelp.Models;
using INAnimalHelp.Models.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(FacebookAuth))]
namespace INAnimalHelp.Droid
{
    public class FacebookAuth : Java.Lang.Object, IFacebookService
    {
        TaskCompletionSource<LoginResult> _completionSource = new TaskCompletionSource<LoginResult>();
        LoginResult user = null;
        /// <summary>
        /// Создание запроса на аутентификацию
        /// </summary>
        /// <returns></returns>
        public Task<LoginResult> Login()
        {
            _completionSource = new TaskCompletionSource<LoginResult>();
            OAuth2Authenticator authenticator =
            new OAuth2Authenticator(
            FacebookConstants.AppID,
            FacebookConstants.Scope,
            new Uri(FacebookConstants.AuthorizeUrl),
            new Uri(FacebookConstants.RedirectUrl))
            { AllowCancel = true };
            authenticator.Completed += OnAuthCompleted;
            authenticator.Error += OnAuthError;

            Intent intent = authenticator.GetUI(Android.App.Application.Context);
            intent.AddFlags(ActivityFlags.NewTask);
            authenticator.ClearCookiesBeforeLogin = true;
            Android.App.Application.Context.StartActivity(intent);
            return _completionSource.Task;
        }
        /// <summary>
        /// Успешное выполнение аутентификации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (!e.IsAuthenticated || e.Account == null)
            {
                //если аутентификация отменена
                user = new LoginResult { LoginState = LoginState.Canceled };
                SetResult();
                return;
            }
            //получение данных о пользователе
            OAuth2Request request = new OAuth2Request("GET", new Uri(FacebookConstants.UserInfoUrl + e.Account.Properties["access_token"]), null, e.Account);
            Response response = await request.GetResponseAsync();
            if (response != null)
            {
                string userJson = await response.GetResponseTextAsync();
                JObject jobject = JObject.Parse(userJson);
                user = new LoginResult()
                {
                    ImageUrl = jobject["picture"]["data"]["url"]?.ToString(),
                    UserId = jobject["id"]?.ToString(),
                    Email = jobject["email"]?.ToString(),
                    FirstName = jobject["first_name"]?.ToString(),
                    Token = e.Account.Properties["access_token"],
                    LoginState = LoginState.Success
                };
            }
            SetResult();
        }
        /// <summary>
        /// Ошибка при аутентификации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            OAuth2Authenticator authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
                user = new LoginResult() { LoginState = LoginState.Failed };
            }
        }
        /// <summary>
        /// Результат аутентификации
        /// </summary>
        void SetResult()
        {
            _completionSource?.TrySetResult(user);
            _completionSource = null;
        }
        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        public void Logout()
        {
            _completionSource = null;
        }
    }
}