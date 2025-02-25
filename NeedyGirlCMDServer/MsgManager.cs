using Newtonsoft.Json;
using ngov3;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeedyGirlCMDServer
{
    [Serializable]
    public class ServerMsg
    {
        public string Id;
        public string EN;
        public string JP;
        public string CN;
        public string KO;
        public string TW;
        public string VN;
        public string FR;
        public string IT;
        public string GE;
        public string SP;
        public string RU;

    }

    internal class MsgManager
    {


        internal static LanguageType currentLang = LanguageType.EN;
        internal static List<ServerMsg> msgList = null;

        internal static void InitializeMsgs()
        {
            var data = Encoding.UTF8.GetString(Resource.serv_messages);
            msgList = JsonConvert.DeserializeObject<List<ServerMsg>>(data);
            //if (msgList == null || msgList.Count == 0)
            //    Initializer.logger.LogError("Failed to initialize messages!");
            //else Initializer.logger.LogInfo("Total messages loaded: " + msgList.Count);
        }
        internal static string SendMessage(ServerMessage msg, params object[] args)
        {
            var msgText = "";
            var msgObj = msgList.Find(m => m.Id == msg.ToString());
            if (msgObj == null && msg != ServerMessage.NONE)
            {
                Initializer.logger.LogError("Could not find message: " + msg.ToString());
                return "";
            }
            ;
            msgText = currentLang switch
            {
                LanguageType.JP => msgObj.JP,
                LanguageType.CN => msgObj.CN,
                LanguageType.KO => msgObj.KO,
                LanguageType.TW => msgObj.TW,
                LanguageType.VN => msgObj.VN,
                LanguageType.FR => msgObj.FR,
                LanguageType.IT => msgObj.IT,
                LanguageType.GE => msgObj.GE,
                LanguageType.SP => msgObj.SP,
                LanguageType.RU => msgObj.RU,
                _ => msgObj.EN,
            };
            if (string.IsNullOrEmpty(msgText))
                msgText = msgObj.EN;
            if (args.Length > 0)
            {
                msgText = string.Format(msgText, args);
            }
            return msgText;
        }
    }

    internal enum ServerMessage
    {
        NONE,
        INVALID_CMD,
        CMD_WRONG_ARGS,
        CMD_MISSING_ARGS,
        CMD_BUSY,
        CMD_SPECIFIC_BUSY,
        CMD_MISSING_ARGS_ONE,
        CMD_MISSING_ARGS_TWO,
        CMD_MISSING_ARGS_THREE,
        ACT_STREAM_LATE,
        ACT_STREAM_EARLY,
        ACT_INVALID,
        ACT_STREAM_END_ACTIVE,
        ACT_STREAM_INVALID,
        ACT_STREAM_NO_NEW,
        ACT_STREAM_USED,
        ACT_STREAM_NOT_FOUND,
        ACT_STREAM_ANGEL_OUTDATED,
        BOOT_SCENE_INVALID,
        BOOT_CAUTION_INACTIVE,
        BOOT_CAUTION_ACTIVE,
        DEBUG_NOT_ACTIVE,
        DEBUG_CAM_NOT_ACTIVE,
        WEBCAM_ANIM_ERROR,
        DEBUG_EVENT_NOT_FOUND,
        DEBUG_SPEED_ARG_ERROR,
        DEBUG_SPEED_HIGH,
        DEBUG_SPEED_LOW,
        DEBUG_STATUS_NAN,
        RESTART_BUSY,
        RESTART_CONFIRMED,
        RESTART_DIALOG_BUSY,
        RESTART_SDOWN_DIALOG_BUSY,
        INFO_HIATUS,
        INFO_RELIGION,
        INFO_GALACTIC_RAIL,
        INFO_STRESS_HORROR,
        INFO_STRESS_BREAK,
        INFO_STRESS_CUT,
        INFO_STRESS_BEFORE,
        INFO_TRAUMA_1,
        INFO_TRAUMA_2,
        INFO_TRAUMA_3,
        INFO_TRAUMA_4,
        INFO_TRAUMA_5,
        INFO_TRAUMA_NONE,
        INFO_ENDING_NONE,
        JINE_NO_WIN_MODIFY,
        JINE_WIN_INVALID_CMD,
        JINE_MISSING_CMD_THREE,
        JINE_HISTORY_COUNT,
        JINE_STICKER_NAN,
        JINE_BUSY,
        JINE_NO_STICKERS,
        JINE_STICKER_OUTRANGE,
        JINE_NO_MESSAGES,
        JINE_OPTION_NAN,
        JINE_NO_OPTIONS,
        JINE_OPTION_NOT_FOUND,
        JINE_READ_NOT_FOUND,
        JINE_HISTORY_EMPTY,
        JINE_READ_OUTRANGE,
        LOAD_USER_NAN,
        LOAD_USER_OUTRANGE,
        LOAD_DAY_OUTRANGE,
        LOAD_DAY_NAN,
        LOAD_SAVE_NOT_FOUND,
        LOAD_MISSING_ARGS_TWO,
        LOAD_SUCCESS,
        LOGIN_INVALID,
        PIC_WIN_INACTIVE,
        PIC_NO_WIN_MODIFY,
        PIC_WIN_INVALID_CMD,
        PIC_INVALID_ID,
        PIC_LOCKED_ZIP,
        PIC_INVALID_FOLDER,
        NOTIF_NO_ACTIVE,
        OPTIONS_NO_WIN_MODIFY,
        OPTIONS_WIN_INVALID_CMD,
        OPTIONS_INVALID_LANG,
        OPTIONS_LANG_SUCCESS,
        OPTIONS_VOL_NAN,
        OPTIONS_VOL_OUTRANGE,
        OPTIONS_BGM_VOL_SUCCESS,
        OPTIONS_SFX_VOL_SUCCESS,
        OPTIONS_SIZE_INVALID,
        OPTIONS_SIZE_SUCCESS,
        STREAM_GEN_IDX_NAN,
        STREAM_GEN_IDX_OUTRANGE,
        STREAM_GEN_INACTIVE,
        STREAM_SPEED_NAN,
        STREAM_SPEED_LOCK,
        STREAM_SPEED_OUTRANGE,
        STREAM_INACTIVE,
        STREAM_COMM_MISSING_ARGS,
        STREAM_COMM_IDX_NAN,
        STREAM_COMM_OVERRANGE,
        STREAM_COMM_UNDERRANGE,
        STREAM_COMM_DELETED,
        STREAM_COMM_READ,
        STREAM_UNSKIPPABLE,
        STREAM_READ_ONLY,
        STREAM_NO_SUPERS,
        TEXT_NOT_FOUND,
        TEXT_NO_PERMISSIONS,
        TEXT_DEBUG_MISSING_ARGS,
        TEXT_DEBUG_ARGS_NAN,
        TWEET_SEND_FOLLOW,
        TWEET_NO_FOLLOW,
        TWEET_NULL_FOLLOW,
        TWEET_MISC_FOLLOW,
        TWEET_NO_WIN_MODIFY,
        TWEET_WIN_INVALID_CMD,
        TWEET_HISTORY_COUNT,
        TWEET_HISTORY_EMPTY,
        TWEET_BUSY,
        TWEET_MSG_NAN,
        TWEET_MSG_OUTRANGE,
        TWEET_REPLIES,
        WEBCAM_NOT_ACTIVE,
        WEBCAM_PAT_OUTRANGE,
        WEBCAM_PAT_COUNT,
        WINDOW_BUTTON_INVALID,
        ZIP_NUM_NAN,
        ZIP_NUM_OUTRANGE,
        ZIP_OPENED,
        PI_ID,
        JINE_WIN_INACTIVE,
        TWEET_WIN_INACTIVE,
        OPTIONS_WIN_INACTIVE,
        LOAD_NO_WIN_MODIFY,
        LOAD_WIN_INACTIVE


    }
}
