function nameTemplate(cellElement, cellInfo) {
    $("<div>")
        .dxTextBox({
            value: cellInfo.value,
            onValueChanged: function (e) {
                updateUserName(cellInfo.key, e.value);
            }
        })
        .appendTo(cellElement);
}

function isEnabledTemplate(cellElement, cellInfo) {
    return $("<div>").dxSwitch({
        value: cellInfo.value,
        onValueChanged: function (e) {
            updateUserStatus(cellInfo.key, e.value)
        }
    }).appendTo(cellElement);
}

function unlockTemplate(cellElement, cellInfo) {
    if (cellInfo.value) {
        $("<div>")
            .addClass("d-flex justify-content-center align-items-center")
            .dxButton({
                text: "解鎖帳號",
                onClick: function () {
                    unlockAccount(cellInfo.key)
                },
            })
            .appendTo(cellElement);
    }
}

function resetPasswordTemplate(cellElement, cellInfo) {
    $("<div>")
        .addClass("reset-password-button")
        .dxButton({
            text: "重設密碼",
            type: "danger",
            stylingMode: "contained",
            onClick: function () {
                showResetPasswordPopup(cellInfo.key)
            }
        })
        .appendTo(cellElement);
}

const cachedPrivileges = createCachedFunction(getPrivileges);
async function privilegeRuleTemplate(cellElement, cellInfo) {
    const privilegeRules = await cachedPrivileges();
    
    $("<div>")
        .dxSelectBox({
            value: cellInfo.value,
            dataSource: privilegeRules,
            valueExpr: "Id",
            displayExpr: "Name",
            onValueChanged: function (e) {
                updateUserPrivilege(cellInfo.key, e.value)
            }
        })
        .appendTo(cellElement);
}
