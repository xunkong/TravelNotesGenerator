using System.ComponentModel;

namespace TravelNotesGenerator.TravelNotes
{
    public enum TravelRecordAwardType
    {

        None = 0,

        /// <summary>
        /// 原石
        /// </summary>
        [Description("原石")]
        Primogems = 1,

        /// <summary>
        /// 摩拉
        /// </summary>
        [Description("摩拉")]
        Mora = 2,

    }
}
