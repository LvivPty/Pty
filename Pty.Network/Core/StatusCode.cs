namespace Pty.Network.Core
{
    public enum StatusCode
    {
        ServerError = 100,
        WrongSize = 101,
        Serialization = 102,
        UnknownCommand = 103,
        IncorrectParameters = 104,
        WrongPassword = 110,
        NotLoggedIn = 112,
        SendingFailed = 113,
        Successed = 400
    }
}
