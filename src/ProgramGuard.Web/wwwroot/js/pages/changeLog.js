function updateCheckBoxUI(checkBoxElement, IsConfirmed) {
    checkBoxElement.option({
        value: IsConfirmed,
        disabled: IsConfirmed
    });
}

function DigitalSignatureTemplate(cellElement, cellInfo) {
    $("<div>").text(cellInfo.value ? "✔️" : "❌")
        .appendTo(cellElement);
}

function checkBoxTemplate(cellElement, cellInfo) {
    var IsConfirmed = isTrue(cellInfo.value);
    $("<div>").dxCheckBox({
        value: IsConfirmed,
        disabled: IsConfirmed,
        onValueChanged: function (e) {
            if (e.value === true && !IsConfirmed) {
                onCheckBoxChanged(cellInfo, this);
            }
        }
    }).appendTo(cellElement);
}

function isTrue(value) {
    if (typeof value === 'string') {
        return value.toLowerCase() === 'true';
    }
    return value === true;
}

function validateQuery() {
    let startTime = getDateBoxValue('startTime');
    let endTime = getDateBoxValue('endTime');
    const unConfirmed = getCheckBoxValue('unConfirmed');
    const fileName = getTextBoxValue('fileName');

    if (!startTime && !endTime && !fileName && !unConfirmed) {
        DevExpress.ui.notify("請至少提供一個查詢條件", "error", 3000);
        return false;
    }
    if (startTime || endTime) {
        if (!validateDateTime(startTime, endTime)) {
            return false;
        }
    }
    reloadGrid();
}

async function onCheckBoxChanged(cellInfo, checkBoxElement) {
    updateCheckBoxUI(checkBoxElement, true);
    const url = '/ChangeLog?Handler=Confirm&key=' + cellInfo.key;
    const { status, data } = await fetchData(url, "PATCH");
    if (status >= 200 && status < 300) {
        if (cellInfo.data) {
            cellInfo.data.ConfirmStatus = true;
        }
        DevExpress.ui.notify('審核成功', "success", 3000);
        reloadGrid();
    } else {
        DevExpress.ui.notify(data.message || '操作失敗', "error", 3000);
    }
}