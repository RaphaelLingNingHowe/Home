async function createRuleSubmit() {
    const visible = calculatePrivileges('visible-privileges-container');
    const operate = calculatePrivileges('operate-privileges-container');
    const name = $("#createRuleName").dxTextBox("instance").option("value");

    const requestData = {
        Name: name,
        Visible: visible,
        Operate: operate
    };
    const url = 'PrivilegeRule';
    const {status, data} = await fetchData(url, "POST", requestData);

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('權限創建成功', "success", 3000);
        hidePopup('popupCreateRule');
        clearPrivilegeSelected();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}

async function updateRuleSubmit() {
    const key = selectedPrivilege.Id;
    const visible = calculatePrivileges('visible-privileges-container');
    const operate = calculatePrivileges('operate-privileges-container');

    const requestData = {
        Visible: visible,
        Operate: operate
    };
    const url = 'PrivilegeRule?handler=Rule&key=' + key;
    const { status, data } = await fetchData(url, "PATCH", requestData);
    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('權限編輯成功', "success", 3000);
        hidePopup('popupUpdateRule');
        clearPrivilegeSelected();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}

async function deleteRuleSubmit() {
    const key = selectedPrivilege.Id;
    const url = '/PrivilegeRule?key=' + key;
    const { status, data } = await fetchData(url, "DELETE");
    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('權限刪除成功', "success", 3000);
        hidePopup('popupDeleteRule');
        clearPrivilegeSelected();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}