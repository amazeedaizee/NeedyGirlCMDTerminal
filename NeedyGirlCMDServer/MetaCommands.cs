using ngov3;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NeedyGirlCMDServer
{
    internal class MetaCommands
    {
        static Button button;
        internal static string RespondToAngel(string command)
        {
            string buttonText;
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            if (!isDataActive)
            {
                return "Can't do this command now.";
            }
            if (!GameObject.Find("EndingCover").transform.Find("jimaku").gameObject.activeInHierarchy)
            {
                return "Can't do this command now.";
            }
            try
            {
                button = GameObject.Find("Metacover").transform.Find("Button1").gameObject.GetComponent<Button>();
                buttonText = button.GetComponentInChildren<TMP_Text>().text;
                if (CheckValidFirstResponse(buttonText) && CheckValidFirstResponse(command))
                    button.onClick.Invoke();
                else if (CheckValidSecondResponse(buttonText) && CheckValidSecondResponse(command))
                    button.onClick.Invoke();
                else if (CheckValidThirdResponse(buttonText) && CheckValidThirdResponse(command))
                    button.onClick.Invoke();
                else return "...";
            }
            catch { return "..."; }
            return "";

        }

        static bool CheckValidFirstResponse(string response)
        {
            string responseJP = NgoEx.TenTalk("Ending_Meta006_pi", LanguageType.JP);
            string responseEN = NgoEx.TenTalk("Ending_Meta006_pi", LanguageType.EN);
            string responseCN = NgoEx.TenTalk("Ending_Meta006_pi", LanguageType.CN);
            string responseKO = NgoEx.TenTalk("Ending_Meta006_pi", LanguageType.KO);
            string responseTW = NgoEx.TenTalk("Ending_Meta006_pi", LanguageType.TW);
            string responseVN = NgoEx.TenTalk("Ending_Meta006_pi", LanguageType.VN);
            string responseFR = NgoEx.TenTalk("Ending_Meta006_pi", LanguageType.FR);
            string responseIT = NgoEx.TenTalk("Ending_Meta006_pi", LanguageType.IT);
            string responseGE = NgoEx.TenTalk("Ending_Meta006_pi", LanguageType.GE);
            string responseSP = NgoEx.TenTalk("Ending_Meta006_pi", LanguageType.SP);
            string responseRU = NgoEx.TenTalk("Ending_Meta006_pi", LanguageType.RU);
            if (response == responseJP)
                return true;
            if (response == responseEN)
                return true;
            if (response == responseCN)
                return true;
            if (response == responseKO)
                return true;
            if (response == responseTW)
                return true;
            if (response == responseVN)
                return true;
            if (response == responseFR)
                return true;
            if (response == responseIT)
                return true;
            if (response == responseGE)
                return true;
            if (response == responseSP)
                return true;
            if (response == responseRU)
                return true;
            return false;

        }

        static bool CheckValidSecondResponse(string response)
        {
            string responseJP = NgoEx.TenTalk("Ending_Meta011_pi", LanguageType.JP);
            string responseEN = NgoEx.TenTalk("Ending_Meta011_pi", LanguageType.EN);
            string responseCN = NgoEx.TenTalk("Ending_Meta011_pi", LanguageType.CN);
            string responseKO = NgoEx.TenTalk("Ending_Meta011_pi", LanguageType.KO);
            string responseTW = NgoEx.TenTalk("Ending_Meta011_pi", LanguageType.TW);
            string responseVN = NgoEx.TenTalk("Ending_Meta011_pi", LanguageType.VN);
            string responseFR = NgoEx.TenTalk("Ending_Meta011_pi", LanguageType.FR);
            string responseIT = NgoEx.TenTalk("Ending_Meta011_pi", LanguageType.IT);
            string responseGE = NgoEx.TenTalk("Ending_Meta011_pi", LanguageType.GE);
            string responseSP = NgoEx.TenTalk("Ending_Meta011_pi", LanguageType.SP);
            string responseRU = NgoEx.TenTalk("Ending_Meta011_pi", LanguageType.RU);
            if (response == responseJP)
                return true;
            if (response == responseEN)
                return true;
            if (response == responseCN)
                return true;
            if (response == responseKO)
                return true;
            if (response == responseTW)
                return true;
            if (response == responseVN)
                return true;
            if (response == responseFR)
                return true;
            if (response == responseIT)
                return true;
            if (response == responseGE)
                return true;
            if (response == responseSP)
                return true;
            if (response == responseRU)
                return true;
            return false;
        }
        static bool CheckValidThirdResponse(string piResponse)
        {
            string firstResponseJP = NgoEx.TenTalk("Ending_Meta014_pi", LanguageType.JP);
            string firstResponseEN = NgoEx.TenTalk("Ending_Meta014_pi", LanguageType.EN);
            string firstResponseCN = NgoEx.TenTalk("Ending_Meta014_pi", LanguageType.CN);
            string firstResponseKO = NgoEx.TenTalk("Ending_Meta014_pi", LanguageType.KO);
            string firstResponseTW = NgoEx.TenTalk("Ending_Meta014_pi", LanguageType.TW);
            string firstResponseVN = NgoEx.TenTalk("Ending_Meta014_pi", LanguageType.VN);
            string firstResponseFR = NgoEx.TenTalk("Ending_Meta014_pi", LanguageType.FR);
            string firstResponseIT = NgoEx.TenTalk("Ending_Meta014_pi", LanguageType.IT);
            string firstResponseGE = NgoEx.TenTalk("Ending_Meta014_pi", LanguageType.GE);
            string firstResponseSP = NgoEx.TenTalk("Ending_Meta014_pi", LanguageType.SP);
            string firstResponseRU = NgoEx.TenTalk("Ending_Meta014_pi", LanguageType.RU);
            string secondResponseJP = NgoEx.TenTalk("Ending_Meta015_pi", LanguageType.JP);
            string secondResponseEN = NgoEx.TenTalk("Ending_Meta015_pi", LanguageType.EN);
            string secondResponseCN = NgoEx.TenTalk("Ending_Meta015_pi", LanguageType.CN);
            string secondResponseKO = NgoEx.TenTalk("Ending_Meta015_pi", LanguageType.KO);
            string secondResponseTW = NgoEx.TenTalk("Ending_Meta015_pi", LanguageType.TW);
            string secondResponseVN = NgoEx.TenTalk("Ending_Meta015_pi", LanguageType.VN);
            string secondResponseFR = NgoEx.TenTalk("Ending_Meta015_pi", LanguageType.FR);
            string secondResponseIT = NgoEx.TenTalk("Ending_Meta015_pi", LanguageType.IT);
            string secondResponseGE = NgoEx.TenTalk("Ending_Meta015_pi", LanguageType.GE);
            string secondResponseSP = NgoEx.TenTalk("Ending_Meta015_pi", LanguageType.SP);
            string secondResponseRU = NgoEx.TenTalk("Ending_Meta015_pi", LanguageType.RU);
            if (piResponse == firstResponseJP)
                return true;
            if (piResponse == firstResponseEN)
                return true;
            if (piResponse == firstResponseCN)
                return true;
            if (piResponse == firstResponseKO)
                return true;
            if (piResponse == firstResponseTW)
                return true;
            if (piResponse == firstResponseVN)
                return true;
            if (piResponse == firstResponseFR)
                return true;
            if (piResponse == firstResponseIT)
                return true;
            if (piResponse == firstResponseGE)
                return true;
            if (piResponse == firstResponseSP)
                return true;
            if (piResponse == firstResponseRU)
                return true;
            if (piResponse == secondResponseJP)
                return true;
            if (piResponse == secondResponseEN)
                return true;
            if (piResponse == secondResponseCN)
                return true;
            if (piResponse == secondResponseKO)
                return true;
            if (piResponse == secondResponseTW)
                return true;
            if (piResponse == secondResponseVN)
                return true;
            if (piResponse == secondResponseFR)
                return true;
            if (piResponse == secondResponseIT)
                return true;
            if (piResponse == secondResponseGE)
                return true;
            if (piResponse == secondResponseSP)
                return true;
            if (piResponse == secondResponseRU)
                return true;
            return false;
        }
    }
}
