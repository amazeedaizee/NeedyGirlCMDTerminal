namespace NeedyGirlCMDServer
{
    internal class ErrorMessages
    {
        internal const string INVALID_CMD = "Invalid command.";
        internal const string CMD_WRONG_ARGS = "This command has invalid parameters.";
        internal const string CMD_MISSING_ARGS = "This command has missing parameters.";
        internal const string CMD_BUSY = "Can't do any commands now.";
        internal const string CMD_SPECIFIC_BUSY = "Can't do this command now.";
    }

    internal enum ServerMessage
    {
        NONE,
        INVALID_CMD,
        CMD_WRONG_ARGS,
        CMD_MISSING_ARGS,
        CMD_BUSY,
        CMD_SPECIFIC_BUSY

    }
}
