async function init() {
    const url = '/PrivilegeRule?handler=Privilege';
    const { status, data } = await fetchData(url, "GET");
    if (status >= 200 && status < 300) {
        if (data.VisiblePrivileges && data.OperatePrivileges) {
            createCheckboxes('visible-privileges-container', data.VisiblePrivileges, 'visible');
            createCheckboxes('operate-privileges-container', data.OperatePrivileges, 'operate');
        } else {
            console.error('Unexpected data format');
        }
    } else {
        throw new Error('Network response was not ok');
    }
}

document.addEventListener('DOMContentLoaded', function () {
    init();
});
