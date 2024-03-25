async function login() {
    var phone = document.getElementById("phoneNumber").value;
    var password = document.getElementById("password").value;
    console.log(phone + ":" + password)
    try {
        const res = await fetch(`https://localhost:44375/login`, {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                guestPhone: phone,
                guestPassword: password,
            })
        });
        if (res.ok) {
            const data = await res.json();
            document.cookie = "UserId=" + data.userId;
            document.cookie = "AccessToken=" + data.token;
            document.cookie = "RefreshToken=" + data.refreshToken;
            window.location.href = '../UserPage/index.html';
        } else if (!res.ok) {
            const data = await res.json(); // Parse response body as JSON
            if (data.errors) {
                const errorMessages = Object.values(data.errors).flat(); // Extract error messages
                window.alert(errorMessages.join("\n")); // Alert error messages
            }
        } else {
            const errorMessage = await res.text(); // Get response body as text
            window.alert(errorMessage); // Alert error message
            window.location.reload();
        }
    } catch (error) {
        window.alert("An error occurred. Please try again."); // Alert if an error occurs
    }
}

