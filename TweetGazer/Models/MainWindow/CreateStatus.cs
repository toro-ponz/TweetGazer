namespace TweetGazer.Models.MainWindow
{
    public class CreateStatus : CreateStatusBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CreateStatus() : base()
        {

        }

        /// <summary>
        /// Flyoutを閉じるとき
        /// </summary>
        public override void Close()
        {
            base.Close();
            this.StatusText = "";
            this.ReplyId = null;
            this.ReplyText = "リプライ先:なし";
            this.DeleteMedia();
        }
    }
}
