$(document).ready(function () {
    $("#navChangePassword").on("click", function (e) {
        e.preventDefault();
        showPopup('popupChangePassword');
    });
});

function onPopupHidden() {
    $("#CurrentPassword").dxTextBox("instance").reset();
    $("#NewPassword").dxTextBox("instance").reset();
    $("#ConfirmPassword").dxTextBox("instance").reset();
}

async function changePasswordSubmit() {
    const currentPassword = $("#CurrentPassword").dxTextBox("instance").option("value");
    const newPassword = $("#NewPassword").dxTextBox("instance").option("value");
    const confirmPassword = $("#ConfirmPassword").dxTextBox("instance").option("value");
    const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$/;
    if (!currentPassword) {
        DevExpress.ui.notify("請輸入密碼", "warning", 3000);
        return;
    }
    if (currentPassword.length > 128) {
        DevExpress.ui.notify("密碼長度不能超過 128 個字元", "warning", 3000);
        return;
    }

    if (!newPassword) {
        DevExpress.ui.notify("請輸入新密碼", "warning", 3000);
        return;
    }
    if (newPassword.length > 128) {
        DevExpress.ui.notify("密碼長度不能超過 128 個字元", "warning", 3000);
        return;
    }
    if (!regex.test(newPassword)) {
        DevExpress.ui.notify("新密碼必須包含大小寫字母、數字和特殊符號，且長度不能少於 8 個字元", "warning", 3000);
        return;
    }

    if (!confirmPassword) {
        DevExpress.ui.notify("請輸入確認密碼", "warning", 3000);
        return;
    }
    if (confirmPassword.length > 128) {
        DevExpress.ui.notify("確認密碼長度不能超過 128 個字元", "warning", 3000);
        return;
    }
    if (newPassword !== confirmPassword) {
        DevExpress.ui.notify("新密碼和確認密碼不匹配", "warning", 3000);
        return;
    }

    const key = $('#hiddenAccount').text();
    const requestData = {
        CurrentPassword: currentPassword,
        NewPassword: newPassword,
        ConfirmPassword: confirmPassword
    };
    const url = '/ChangePassword?Handler=ChangePassword&key=' + key;

    const { status, data } = await fetchData(url, "POST", requestData);

    if (status >= 200 && status < 300) {
        DevExpress.ui.notify('密碼更換成功', "success", 3000);
        hidePopup('popupChangePassword');
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}