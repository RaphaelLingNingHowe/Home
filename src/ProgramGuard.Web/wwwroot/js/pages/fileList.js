async function submitCreateFile(){
    const filePath = $("#FilePath").dxTextBox("instance").option("value");
    const regex = /^([a-zA-Z]:)((?:\\|\/)[\w\s\u4e00-\u9fa5()\[\].-]+)+(?:\\|\/)[\w\s\u4e00-\u9fa5()\[\].-]+\.\w+$/i;
    // 驗證檔案路徑
    if (!filePath) {
        DevExpress.ui.notify("請輸入檔案路徑", "warning", 3000);
        return;
    }
    if (filePath.length > 2000) {
        DevExpress.ui.notify("檔案路徑長度不能超過 2000 個字元", "warning", 3000);
        return;
    }
    if (!regex.test(filePath)) {
        DevExpress.ui.notify("請輸入有效的檔案路徑", "warning", 3000);
        return;
    }

    const requestData = {
        Path: filePath
    };
    const url = '/FileList';
    const { status, data } = await fetchData(url, "POST", requestData);
    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('檔案添加成功', "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}

function validatePath(e) {
    return new Promise((resolve) => {
        var path = e.value;
        const regex = /^([a-zA-Z]:)((?:\\|\/)[\w\s\u4e00-\u9fa5()\[\].-]+)+(?:\\|\/)[\w\s\u4e00-\u9fa5()\[\].-]+\.\w+$/i;
        if (regex.test(path)) {
            resolve(true);
        } else {
            DevExpress.ui.notify("請輸入有效的檔案路徑", "warning", 3000);
            resolve(false);
        }
    });
}
