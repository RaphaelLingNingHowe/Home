async function submitLogin() {
    const account = $("#txtAccount").dxTextBox("instance").option("value");
    const password = $("#txtPassword").dxTextBox("instance").option("value");

    // 驗證帳號
    if (!account) {
        DevExpress.ui.notify("請輸入帳號", "warning", 3000);
        return;
    }
    if (account.length > 16) {
        DevExpress.ui.notify("帳號長度不能超過 16 個字元", "warning", 3000);
        return;
    }

    // 驗證密碼
    if (!password) {
        DevExpress.ui.notify("請輸入密碼", "warning", 3000);
        return;
    }
    if (password.length > 128) {
        DevExpress.ui.notify("密碼長度不能超過 128 個字元", "warning", 3000);
        return;
    }

    // 通過驗證，發送請求
    const requestData = {
        Account: account,
        Password: password
    };
    const url = '/Login?Handler=Login';

    try {
        const { status, data } = await fetchData(url, "POST", requestData);
        if (status >= 200 && status < 300) {
            if (data) {
                showPopup('popupChangePassword');
                $("#changePasswordMessage").text(data);
                DevExpress.ui.notify(data, "error", 3000);
            } else {
                window.location.href = '/FileList';
            }
        } else {
            DevExpress.ui.notify(data || '操作失敗', "error", 3000);
        }
    } catch (error) {
        DevExpress.ui.notify('請求失敗', "error", 3000);
        console.error('Login request failed:', error);
    }
}