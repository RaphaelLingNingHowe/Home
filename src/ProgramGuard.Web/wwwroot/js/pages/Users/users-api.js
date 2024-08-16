async function submitCreateUser() {
    const account = $("#txtAccount").dxTextBox("instance").option("value");
    const name = $("#txtName").dxTextBox("instance").option("value");
    const password = $("#txtPassword").dxTextBox("instance").option("value");
    const privilegeRule = $("#selPrivilegeRule").dxSelectBox("instance").option("value");
    const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$/;

    // 驗證帳號
    if (!account) {
        DevExpress.ui.notify("請輸入帳號", "warning", 3000);
        return;
    }
    if (account.length > 16) {
        DevExpress.ui.notify("帳號長度不能超過 16 個字元", "warning", 3000);
        return;
    }

    // 驗證名稱
    if (!name) {
        DevExpress.ui.notify("請輸入名稱", "warning", 3000);
        return;
    }
    if (name.length > 16) {
        DevExpress.ui.notify("名稱長度不能超過 16 個字元", "warning", 3000);
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
    if (!regex.test(password)) {
        DevExpress.ui.notify("新密碼必須包含大小寫字母、數字和特殊符號，且長度不能少於 8 個字元", "warning", 3000);
        return;
    }

    // 驗證權限
    if (!privilegeRuleId) {
        DevExpress.ui.notify("請輸入權限規則", "warning", 3000);
        return;
    }
    const requestData = {
        Account: account,
        Name: name,
        Password: password,
        PrivilegeRuleId: privilegeRule.Id
    };
    const url = 'User';

    const { status, data } = await fetchData(url, "POST", requestData);

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('帳號創建成功', "success", 3000);
        hidePopup('popupCreateUser');
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}

async function updateUserName(key, userName) {
    const name = $("#txtName").dxTextBox("instance").option("value");
    // 驗證名稱
    if (!name) {
        DevExpress.ui.notify("請輸入名稱", "warning", 3000);
        return;
    }
    if (name.length > 16) {
        DevExpress.ui.notify("名稱長度不能超過 16 個字元", "warning", 3000);
        return;
    }
    const requestData = {
        Name: userName
    };
    const url = '/User?Handler=Name&key=' + key;

    const { status, data } = await fetchData(url, "PATCH", requestData);

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('名稱變更成功', "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}
async function updateUserStatus(key, isEnabled) {
    const apiUrl = isEnabled ? 'User?Handler=Enable' : 'User?Handler=Disable';
    const url = apiUrl + '&key=' + key;

    const { status, data } = await fetchData(url, "PATCH");

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify("帳號狀態已更新" , "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}

async function unlockAccount(key) {
    const url = 'User?Handler=unlock&key=' + key;

    const { status, data } = await fetchData(url, "PATCH");

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('帳號已解鎖', "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}

async function submitResetPassword() {
    const account = $("#popupResetPassword").dxPopup("instance").option("account");
    const resetPassword = $("#resetPassword").dxTextBox("instance").option("value");

    const requestData = {
        ResetPassword : resetPassword
    };
    const url = 'User?Handler=ResetPassword&key=' + key;

    const { status, data } = await fetchData(url, "POST", requestData);

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('密碼重置成功', "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}

async function updateUserPrivilege(key, values) {
    const url = '/User?handler=Privilege&key=' + key
    const requestData = {
        privilegeRuleId : values
    };
    const { status, data } = await fetchData(url, "PATCH", requestData);

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('權限更新成功', "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}

async function getPrivileges() {
    const url = '/PrivilegeRule?Handler=Data';
    const { status, data } = await fetchData(url, "GET");
    if (status >= 200 && status < 300) {
        return data;
    } else {
        throw new Error(data.message || '獲取權限失敗');
    }
}
