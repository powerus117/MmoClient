using Login.Domain;
using UniRx;

namespace Login
{
    public class LoginScreenModel
    {
        public ReactiveProperty<LoginScreenState> LoginScreenState { get; } =
            new ReactiveProperty<LoginScreenState>();
    }
}