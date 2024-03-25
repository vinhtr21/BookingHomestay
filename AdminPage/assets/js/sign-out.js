function clearAllCookies() {
    var cookies = document.cookie.split("; ");
    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i];
        var eqPos = cookie.indexOf("=");
        var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
        document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=127.0.0.1; Secure";
    }
}


function logout_owner() {
    fetch(`https://localhost:44375/logout`, {
        method: "POST",
    })
        .then(response => {
            if (response.ok) {
                clearAllCookies(); // Call clearAllCookies() if the fetch request is successful
                window.location.href = "../pages/owner-login.html";
            } else {
                console.error("Failed to logout");
            }
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

function logout_admin() {
    fetch(`https://localhost:44375/logout`, {
        method: "POST",
    })
        .then(response => {
            if (response.ok) {
                clearAllCookies(); // Call clearAllCookies() if the fetch request is successful
                window.location.href = "../pages/sign-in.html";
            } else {
                console.error("Failed to logout");
            }
        })
        .catch(error => {
            console.error("Error:", error);
        });
}