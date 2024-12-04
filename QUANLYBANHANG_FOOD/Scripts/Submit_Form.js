
function submitLogin() {
    var formLogin = document.getElementById("form-login");
    var username = document.getElementById("username");
    var pass = document.getElementById("pass");
    var isFormValid = true;

    if (username.value.trim() === "") {
        showError("Vui lòng nhập tên tài khoản !", e_user);
        isFormValid = false;
    }
    if (pass.value.trim() === "") {
        showError("Vui lòng nhập mật khẩu !", e_pass);
        isFormValid = false;
    }


    if (!isFormValid) {
        return false; // Ngăn chặn sự kiện submit khi form không hợp lệ
    }
    else {
        formLogin.submit();
    }
    // Nếu form hợp lệ, tiếp tục submit
    return true;
}


function submitSignup() {
    var formSignup = document.getElementById("form-login");
    var username = document.getElementById("username");

    var phone = document.getElementById("phone");
    var pass = document.getElementById("pass");
    var repass = document.getElementById("repass");
    var isFormValid = true;

    if (username.value.trim() === "") {
        showError("Vui lòng nhập tên tài khoản !", e_user);
        isFormValid = false;
    }
    if (phone.value.trim() === "") {
        showError("Vui lòng nhập số điện thoại !", e_phone);
        isFormValid = false;
    }
    if (pass.value.trim() === "") {
        showError("Vui lòng nhập mật khẩu !", e_pass);
        isFormValid = false;
    }
    if (repass.value.trim() === "") {
        showError("Vui lòng nhập lại mật khẩu !", e_repass);
        isFormValid = false;
    }
    //if (pass.value.trim() !== repass.value.trim()) {
    //    showError("Mật khẩu nhập lại không trùng khớp !", e_check);
    //    isFormValid = false;
    //}


    if (!isFormValid) {
        return false; // Ngăn chặn sự kiện submit khi form không hợp lệ
    }
    else {
        if (pass.value.trim() !== repass.value.trim()) {
            showError("Mật khẩu nhập lại không trùng khớp !", e_check);
            return false;
        }
        formSignup.submit();
    }
    // Nếu form hợp lệ, tiếp tục submit
    return true;
}



function showError(errorMessage, id) {
    $(id).text(errorMessage);
    setTimeout(hideErrors, 3000);
}

function hideErrors() {
    $('.rq').text('');
}
