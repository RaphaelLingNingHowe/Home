﻿using System.ComponentModel;

namespace ProgramGuard.Enums
{
    [Flags]
    public enum VISIBLE_PRIVILEGE : uint
    {
        [Description("預設值, 代表無權限")]
        UNKNOWN = 0,

        [Description("可查閱檔案清單")]
        SHOW_FILELIST = 1,

        [Description("可查閱檔案異動")]
        SHOW_CHANGELOG = 2,

        [Description("可查閱使用者管理")]
        SHOW_USER = 4,

        [Description("可查閱權限管理")]
        SHOW_PRIVILEGE_RULE = 8,

        [Description("可查閱操作記錄")]
        SHOW_OPERATELOG = 16
    }
}
