function showResetPasswordPopup(key) {
    var popup = $("#popupResetPassword").dxPopup("instance");
    popup.show();
    popup.option("key", key);
}

function showCreateUserPopup(){
    showPopup(popupCreateUser);
}