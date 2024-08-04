using System.ComponentModel;

namespace ProgramGuard.Enums
{
    public enum CHANGE_TYPE : byte
    {
        [Description("創建")]
        CREATED = 1,

        [Description("刪除")]
        DELETED = 2,

        [Description("修改")]
        CHANGED = 3,

        [Description("重命名")]
        RENAMED = 4
    }
}