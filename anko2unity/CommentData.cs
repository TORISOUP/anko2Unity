﻿using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using ankoPlugin2;
using LibAnko;

namespace anko2unity
{
    [DataContract]
    public class CommentData
    {

        #region params

        /// <summary>
        /// 184
        /// </summary>
        [DataMember]
        public bool Anonymity;

        /// <summary>
        /// 投稿時刻
        /// 
        /// </summary>
        [DataMember]
        public DateTime Date;
        /// <summary>
        /// コマンド
        /// 
        /// </summary>
        [DataMember]
        public string Mail;
        /// <summary>
        /// 名前（運営コメント)
        /// 
        /// </summary>
        [DataMember]
        public string Name;

        /// <summary>
        /// コメビュによって命名される名前
        /// </summary>
        [DataMember]
        public string NickName;

        /// <summary>
        /// コメント
        /// 
        /// </summary>
        [DataMember]
        public string Message;
        /// <summary>
        /// コメント番号
        /// 
        /// </summary>
        [DataMember]
        public int No;
        /// <summary>
        /// 投稿者の属性をあらわす数値
        /// 
        /// </summary>
        [DataMember]
        public int Premium;
        /// <summary>
        /// スレッドID
        /// 
        /// </summary>
        [DataMember]
        public int Thread;
        /// <summary>
        /// ユーザーID
        /// 
        /// </summary>
        [DataMember]
        public string UserId;
        /// <summary>
        /// コメント位置
        /// 
        /// </summary>
        [DataMember]
        public int Vpos;
        /// <summary>
        /// 地域
        /// 
        /// </summary>
        [DataMember]
        public string Locale;

        #endregion


        DataContractJsonSerializer jsonSerializer;

        public CommentData(IChat chat, UserInfo user)
        {
            jsonSerializer = new DataContractJsonSerializer(typeof(CommentData));

            this.Anonymity = chat.Anonymity;
            this.Date = chat.Date;
            this.Locale = chat.locale;
            this.Mail = chat.Mail;
            this.Message = chat.Message;
            this.Name = user == null ? chat.Name : (user.DisplayName ?? "");
            this.NickName = user == null ? "" : (user.CharaName ?? "");
            this.No = chat.No;
            this.Premium = chat.Premium;
            this.Thread = chat.Thread;
            this.UserId = chat.UserId;
            this.Vpos = chat.Vpos;

        }

        public string ToJson()
        {
            var result = "";
            using (var stream = new MemoryStream())
            {
                jsonSerializer.WriteObject(stream, this);

                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }

    }
}
