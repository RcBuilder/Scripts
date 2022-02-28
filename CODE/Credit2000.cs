using System;
using System.Collections.Generic;

namespace PaymentsService
{
    /*
        WCF Services:
        https://www.credit2000.co.il/pci_tkn_ver2/wcf/wscredit2000.asmx
        https://www.credit2000.co.il/pci_tkn_ver4/wcf/wscredit2000.asmx 

        -

        var proxy = new CreditApiProxyV2.wsCredit2000SoapClient();
        var proxy = new CreditApiProxyV4.wsCredit2000SoapClient();

        -

        CreditXMLPro
    */

    #region Entities
    public class Credit2000PaymentData
    {
        public string CVV { get; set; }
        public string CardNumber { get; set; }
        public string CardExpiry { get; set; } // MMYY            
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserFax { get; set; }
        public string UserTZ { get; set; }
        public string Comments { get; set; }
        public float Price { get; set; }
    }

    public class Credit2000ChargeResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }       
    }
    #endregion

    public class Credit2000
    {
        public class Constants {
            public const string ReturnCode = "888";
            public const string ConfirmationNumber = "0000000";
            public const string Club = "0";
            public const string Stars = "0";
            public const string ReaderData = "0";
            public const string ConfirmationSource = "0";
            public const string PaymentsNumber = "001";  // number of payments
            public const string FixedAmmount = "0"; 
        } 

        public enum eActionType { CHARGE = 4, CONFIRMATION = 5, REFUND = 7 } // actionType (see spec)
        public enum eCardReader { CONNECTED = 1, DISCONNECTED } // cardReader (see spec)
        public enum eCardType { ISRACARD = 1, VISA, DINERS, AMEX } // cardType (see spec)
        public enum ePurchaseType { REGULAR = 1, PAYMENTS = 2, CREDIT = 3 } // purchaseType (see spec)
        public enum eCurrency { NIS = 1, USD } // currency (see spec)  

        public readonly Dictionary<string, string> StatusCodesMap = new Dictionary<string, string> {
            ["000"] = "עיסקה תקינה",
            ["001"] = "חסום החרם כרטיס",
            ["002"] = "גנוב החרם כרטיס",
            ["003"] = "התקשר לחברת האשראי",
            ["004"] = "סירוב",
            ["005"] = "מזויף החרם כרטיס",
            ["006"] = "חובה להתקשר לחברת האשראי",
            ["008"] = "תקלה בבניית מפתח גישה לקובץ חסומים",
            ["009"] = "לא הצליח להתקשר",
            ["010"] = "תוכנית הופסקה עפ''י הוראת המפעיל",
            ["019"] = "רשומה בקובץ קלט לא תקינה",
            ["020"] = "קובץ קלט לא קיים",
            ["021"] = "קובץ חסומים לא מעודכן",
            ["022"] = "אחד מקבצי פרמטרים או ווקטורים לא קיים",
            ["023"] = "קובץ תאריכים לא קיים",
            ["024"] = "קובץ אתחול לא קיים",
            ["025"] = "הפרש בימים בקליטD7 חסומים גדול מדי",
            ["026"] = "הפרש דורות בקליטת חסומים גדול מידי",
            ["027"] = "כאשר לא הוכנס פס מגנטי כולו הגדר עיסקה כעיסקה טלפונית",
            ["028"] = "מספר מסוף מרכזי לא הוכנס למסוף המוגדר לעבודה כרב ספק",
            ["029"] = "ספר מוטב לא הוכנס למסוף המוגדר לעבודה כרב מוטב",
            ["030"] = "מסוף שאינו מעודכן כרב ספק / רב מוטב והוקלד מס' ספק/מס' מוטב",
            ["031"] = "מסוף מעודכן כרב ספק והוקלד גם מס' מוטב",
            ["032"] = "תנועות ישנות בצע שידור או בקשה לאישור עבור כל עיסקה",
            ["033"] = "כרטיס לא תקין",
            ["034"] = "כרטיס לא רשאי לבצע במסוף זה או אין אישור לעיסקה כזאת",
            ["035"] = "כרטיס לא רשאי לבצע עיסקה עם סוג אשראי זה",
            ["036"] = "פג תוקף",
            ["037"] = "שגיאה בתשלומים",
            ["038"] = "לא ניתן לבצע עיסקה מעל תקרה לכרטיס לאשראי חיוב מיידי",
            ["039"] = "סיפרת בקורת לא תקינה",
            ["040"] = "סוף שמוגדר כרב מוטב הוקלד מס' ספק",
            ["041"] = "סכום מעל תיקרה - בעיה הגדרת נתונים",
            ["042"] = "כרטיס חסום בספק",
            ["041"] = "סכום מעל תיקרה - בעיה הגדרת נתונים",
            ["044"] = "לא ראשאי לבקש אישור ב5",
            ["045"] = "מסוף לא רשאי לבקש אישור ביוזמת קמעונאי",
            ["046"] = "מסוף חייב לבקש אישור",
            ["047"] = "חייב להקליד מספר סודי",
            ["051"] = "מספר רכב לא תקין",
            ["052"] = "מד מרחק  לא הוקלד",
            ["053"] = "עיסקה לא מתאים",
            ["060"] = "צרוף ABS לא נמצא בהתחלת נתוני קלט בזיכרון",
            ["061"] = "מספר כרטיס לא נמצא או נמצא פעמיים",
            ["062"] = "סוג עיסקה לא תקי",
            ["063"] = "קוד עיסקה לא תקין",
            ["064"] = "סוג אשראי לא תקין",
            ["065"] = "מטבע לא תקין",
            ["066"] = "קיים תשלום ראשון ו / או תשלום קבוע לסוג אשראי שונה מתשלומים",
            ["067"] = "יים מספר תשלומים לסוג אשראי שאינו דורש זה",
            ["068"] = "לא ניתן להצמיד לדולר או למדד לסוג אשראי שונה",
            ["069"] = "אורך הפס המגנטי קצר מידי",
            ["071"] = "מספר שגוי",
            ["072"] = "מספר סודי לא הוקלד",
            ["073"] = "מספר סודי שגוי",
            ["074"] = "מספר סודי שגוי - ניסיון אחרון",
            ["099"] = "לא מצליח לקרוא / לכתוב / לפתוח  קובץ TRAN",
            ["100"] = "לא קיים מכשיר להקשת מספר סודי",
            ["101"] = "אין אישור מחברת אשראי לעבודה",
            ["107"] = "סכום העיסקה גדול מידי - חלק במספר העיסקאות",
            ["108"] = "למסוף אין אישור לבצע עסקאות מאולצות",
            ["109"] = "למסוף אין אישור לכרטיס עם קוד השר = ת 587",
            ["110"] = "למסוף אין אישור לכרטיס חיוב מיידי",
            ["111"] = "למסוף אין אישור לעיסקה בתשלומים",
            ["112"] = "למסוף אין אישור לעיסקה טלפון / חתימה בלבד בתשלומים",
            ["113"] = "למסוף אין אישור לעיסקה טלפונית",
            ["114"] = "למסוף אין אישור לעיסקה ''חתימ = 94 בלבד''",
            ["115"] = "למסוף אין אישור לעיסקה בדולרים",
            ["116"] = "למסוף אין אישור לעסקת מועדון",
            ["117"] = "למסוף אין אישור לעסקת כוכבים / נקודות / מיילים",
            ["118"] = "למסוף אין אישור לאשראי ישראקרדיט",
            ["119"] = "למסוף אין אישור לאשראי אמקס  קרדיט",
            ["120"] = "למסוף אין אישור להצמדה לדולר",
            ["121"] = "למסוף אין אישור להצמדה למדד",
            ["122"] = "למסוף אין אישור להצמדה למדד לכרטיסי חו''ל",
            ["123"] = "למסוף אין אישור לעסקת כוכבים / נקודות / מיילים לסוגאשראי זה",
            ["124"] = "למסוף אין אישור לאשראי ישרא 36",
            ["125"] = "למסוף איו אישור לאשראי אמקס 36",
            ["126"] = "למסוף אין אישור לקוד מועדון זה",
            ["127"] = "למסוף אין אישור לעסקת חיוב מיידי פרט לכרטיסי חיוב מיידי",
            ["128"] = "למסוף אין אישור לקבל כרטיסי ויזה אשר מתחילים ב - 3",
            ["129"] = "למ סוף אין אישור לבצע עסקת זכות מעל תקרה",
            ["130"] = "כרטיס  לא רשאי לבצע עסקת מועדון",
            ["131"] = "כרטיס לא רשאי לבצע עסקת כוכבים / נקודות / מיילים",
            ["132"] = "כרטיס לא רשאי לבצע עסקאות בדולרים(רגילות או טלפוניות)",
            ["133"] = "כרטיס לא תקף על פי רשימת כרטיסים",
            ["134"] = "כרטיס לא תקין עפ''י הגדרת המערכת",
            ["135"] = "כרטיס לא רשאי לבצע עסקאות דולריות עפ''י הדרת המערכת",
            ["136"] = "כרטיסים אשר אינה רשאית לבצע  עסקאות עפ''י הגדרת המערכת",
            ["137"] = "קידומת הכרטיס(7 ספרות) לא תקפה עפ''י הגדרת המערכת",
            ["138"] = "כרטיס לא רשאי לבצע עסקאות בתשלומים",
            ["139"] = "מספר תשלומים גדול מידי על פי רשימת כרטיסים",
            ["140"] = "כרטיסי ויזה ודיינרס לא רשאים לבצע עסקאות מועדון",
            ["141"] = "סידרת כרטיסים לא תקפה עפ''י הגדרת המערכת",
            ["142"] = "קוד שרות לא תקף עפ''י הגדרת המערכת",
            ["143"] = "קידומת הכרטיס(2 ספרות) לא תקפה עפ''י הגדרת המערכת",
            ["144"] = "קוד שרות לא תקף עפ''י הגדרת המערכת",
            ["145"] = "קוד שרות לא תקף עפ''י הגדרת המערכת",
            ["146"] = "לכרטיס חיוב מיידי אסור לבצע עסקת זכות",
            ["147"] = "כרטיס לא רשאי לבצע עיסקאות בתשלומים עפ''י",
            ["148"] = "כרטיס לא רשאי לבצע עסקאות טלפוניות וחתימה בלבד",
            ["149"] = "כרטיס אינ ו רשאי לבצע עיסקאות טלפוניות",
            ["150"] = "אשראי לא מאושר לכרטיסי חיוב מיידי",
            ["151"] = "אשראי לא מאושר לכרטיסי חו''ל",
            ["152"] = "קוד מועדון לא תקין",
            ["153"] = "כרטיס לא רשאי לבצע עסקאות אשראי גמיש",
            ["154"] = "כרטיס לא רשאי לבצע עסקאות חיוב מיידי עפ''י הגדרת",
            ["155"] = "סכום לתשלום בעסקת קרדיט קטן מידי",
            ["156"] = "מספר תשלומים לעסקת קרדיט לא תקין",
            ["157"] = "תקרה 0 לסוג כרטיס זה בעיסקה עם אשראי רגיל או קרדיט",
            ["158"] = "תקרה 0 לסוג כרטיס זה בעיסקה עם אשראי חיוב מיידי",
            ["159"] = "תקרה 0 לסוג כרטיס זה  בעסקת חיוב מיידי בדולרים",
            ["160"] = "תקרה 0 לסוג כרטיס זה בעיסקה טלפונית",
            ["161"] = "תקרה 0 לסוג כרטיס זה בעסקת זכות",
            ["162"] = "תקרה 0 לסוג כרטיס זה בעסקת תשלומים",
            ["163"] = "כרטיס אמריקן אקספרס אשר הונפק בחו''ל לא רשאי",
            ["164"] = "כרטיס רשאי לבצע עסקאות רק באשראי רגיל",
            ["165"] = "סכום בכוכבים / נקודות / מיילים גדול מסכום העיסקה",
            ["166"] = "כרטיס מועדון לא בתחום של המסוף",
            ["167"] = "לא ניתן לבצע עסקת כוכבים / נקודות / מיילים בדולרים",
            ["168"] = "לא ניתן לבצע עיסקה בדולרים עם סוג אשראי זה",
            ["169"] = "לא ניתן לבצע עסקת זכות עם אשראי שונה מהרגיל",
            ["170"] = "סכום הנחה בכוכבים / נקודות / מיילים גדול מהמותר",
            ["171"] = "לא ניתן לבצע עיסקה מאולצת לכרטיס / אשראי חיוב מיידי",
            ["172"] = "לא ניתן לבטל עיסקה קודמת(עיסקת זכות או מספר",
            ["173"] = "עיסקה כפולה",
            ["174"] = "למסוף אין אישור להצמדה למדד לאשראי זה",
            ["175"] = "למסוף אין אישור להצמדה לדולר לאשראי זה",
            ["176"] = "כרטיס אינו תקף ע''פ הגדרת ה מערכת",
            ["177"] = "בתחנות דלק לא ניתן לבצע שרות עצמי",
            ["178"] = "אסור לבצע עיסקת זכות בכוכבים / נקודות / מיילים",
            ["179"] = "אסור לבצע עיסקת זכות בדולר בכרטיס תייר",
            ["200"] = "שגיאה יישומית",
            ["999"] = "טבלאות לא מעודכנות",
            ["998"] = "תקלה בספריית הנתונים - INPUT",
            ["997"] = "תקלה בספריית הנתונים - OUTPUT",
            ["996"] = "רכיבי תוכנה חסרים",
            ["993"] = "אין תקשורת לשרת - זמני",
            ["992"] = "בעיה בקו תקשורת",
            ["990"] = "תקלה במחשב שרת - להפסיק שידור"
        };

        // ---

        protected string ApiUserName { get; set; }
        protected string ApiUserKey { get; set; }
        protected int ApiVersion { get; set; }

        public Credit2000(string ApiUserName, string ApiUserKey, int ApiVersion) {
            this.ApiUserName = ApiUserName;
            this.ApiUserKey = ApiUserKey;
            this.ApiVersion = ApiVersion;
        }

        public Credit2000ChargeResponse Charge(Credit2000PaymentData PaymentData) {
            try
            {                
                dynamic proxy = new CreditApiProxyV2.wsCredit2000SoapClient();
                if (this.ApiVersion == 4)
                    proxy = new CreditApiProxyV4.wsCredit2000SoapClient();

                string balance, idStatus, idCVV, companyId;
                string returnCode = Constants.ReturnCode,
                       confirmationNumber = Constants.ConfirmationNumber,
                       cardType = ((byte)eCardType.VISA).ToString(),
                       price = this.PriceConverter(PaymentData.Price),
                       cardExpiryMonth = PaymentData.CardExpiry.Substring(0, 2),
                       cardExpiryYear = PaymentData.CardExpiry.Substring(2, 2),
                       cardNumberRef = PaymentData.CardNumber ?? "";

                string result = proxy.CreditXMLPro(
                    PaymentData.CVV ?? "",
                    ref cardType,
                    this.ApiUserName,
                    ((byte)ePurchaseType.REGULAR).ToString(), 
                    ((byte)eCurrency.NIS).ToString(),
                    ref cardNumberRef,
                    ref cardExpiryMonth,
                    ref cardExpiryYear,
                    price, // total price
                    Constants.PaymentsNumber,
                    price, // 1st payment
                    Constants.FixedAmmount,
                    PaymentData.UserId ?? "",
                    ref returnCode,
                    ref confirmationNumber,
                    ((byte)eActionType.CHARGE).ToString(),
                    ((byte)eCardReader.DISCONNECTED).ToString(),
                    Constants.ConfirmationSource,
                    PaymentData.UserTZ ?? "",
                    Constants.Club,
                    Constants.Stars,
                    Constants.ReaderData,
                    this.ApiUserKey,
                    PaymentData.UserFullName ?? "",
                    PaymentData.UserEmail ?? "",
                    PaymentData.UserPhone ?? "",
                    PaymentData.UserFax ?? "",
                    PaymentData.Comments ?? "",
                    out balance,
                    out idStatus,
                    out idCVV,
                    out companyId
                )?.Trim();


                var returnCodeMessage = "";
                if (returnCode.Length < 3)
                    returnCode = returnCode.PadLeft(3, '0');

                if (StatusCodesMap.ContainsKey(returnCode))
                    returnCodeMessage = $"{StatusCodesMap[returnCode]}";

                return new Credit2000ChargeResponse { 
                    Code = returnCode,
                    Message = returnCodeMessage
                };
            }
            catch (Exception ex) {
                return new Credit2000ChargeResponse
                {
                    Code = "999",
                    Message = ex.Message
                };                
            }
        }

        private string PriceConverter(float Price) {
            return $"000{Price * 100}";  // Price -> agorot + leading 000 (000 x*100) - e.g: 100NIS = 00010000
        }
    }
}
