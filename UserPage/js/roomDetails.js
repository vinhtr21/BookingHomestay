var priceHome = 0;
function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

async function fetchData() {
    var Id = getParameterByName('Id');
    const response = await fetch(`https://localhost:44375/checkAccessToken`, {
        method: 'POST',
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(getCookie("AccessToken"))
    });
    console.log(response);
    if (!response.ok) {
        const refreshRq = await fetch(`https://localhost:44375/refresh-token`, {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ refreshToken: getCookie("RefreshToken") })
        });
        if (refreshRq.ok) {
            const data = await refreshRq.json();
            document.cookie = "AccessToken=" + data.token;
        }
    }
    await fetch(`https://localhost:44375/api/Homestays/${Id}`, {
        headers: {
            "Authorization": `Bearer ${getCookie("AccessToken")}`
        }
    })
        .then(response => response.json())
        .then(data => {
            console.log(data);
            var tableBody = document.getElementsByClassName("room-details-item")[0]; // accessing the first element in the collection
            fetchImage(Id);
            priceHome = data[0].homestayPrice;
            tableBody.innerHTML = `
                <div class="rd-text">
                    <div class="rd-title">
                        <h3>${data[0].homestayName}</h3>
                        <div class="rdt-right">
                            <div class="rating">
                                <i class="icon_star"></i>
                                <i class="icon_star"></i>
                                <i class="icon_star"></i>
                                <i class="icon_star"></i>
                                <i class="icon_star-half_alt"></i>
                            </div>
                        </div>
                    </div>
                    <h2>${data[0].homestayPrice}<span>/1 đêm</span></h2>
                    <table>
                        <tbody>
                            <tr>
                                <td class="r-o">Loại:</td>
                                <td>${data[0].homestayType}</td>
                            </tr>
                            <tr>
                                <td class="r-o">Địa chỉ:</td>
                                <td>${data[0].homestayStreet}, ${data.homestayRegion}, ${data.homestayCity}</td>
                            </tr>
                            <tr>
                                <td class="r-o">Số giường:</td>
                                <td>${data[0].homestayBedroom}</td>
                            </tr>
                            <tr>
                                <td class="r-o">Dịch Vụ:</td>
                                <td>Wifi, TV, Bồn Tắm, Ghế tình yêu...</td>
                            </tr>
                        </tbody>
                    </table>
                    <p class="f-para">Mô tả</p>
                    <p class="f-para">${data[0].homestayDescription}</p>
                </div>
            `;
        })
        .catch(error => {
            console.error('Error fetching data:', error);
        });
}

function fetchImage() {
    var Id = getParameterByName('Id');
    fetch(`https://localhost:44375/Image/list/${Id}`)
        .then(response => response.json())
        .then(data => {
            console.log(data);
            // var tableBody = document.getElementById("image-carousel");
            var tableBody = document.getElementsByClassName("room-details-item")[0];
            if (!tableBody) {
                console.error("Table body not found");
                return;
            }
            data.forEach(item => {
                var imgElme = document.createElement("img");
                // imgElme.className ="room-image";
                var base64 = item.image1;
                imgElme.setAttribute("src", "data:image/jpg;base64," + base64);
                // tableBody.prepend(imgElme);
                tableBody.appendChild(imgElme);
            });
        })
        .catch(error => {
            console.error('Error fetching images:', error);
        });
    // const wrapper = document.querySelector("#wrapper");
    // const carousel = document.querySelector("#image-carousel");
    // const images = document.querySelectorAll(".room-image");
    // const btn = document.querySelectorAll("button");
    // const previous = document.querySelector("#prev");
    // const nxt = document.querySelector("#next");

    // images.forEach((slide, index) => {
    //     slide.style.left = `${index * 100}%`;
    // });
    // let counter = 0;

    // const slideImage = () => {
    //     images.forEach((e) => {
    //         e.style.transform = `translateX(-${counter * 100}%)`;
    //     });
    // };

    // const prev = () => {
    //     if (counter > 0) {
    //         counter--;
    //         slideImage();
    //         console.log(counter);
    //     }
    // };
    // const next = () => {
    //     if (counter <= images.length - 2) {
    //         counter++;
    //         slideImage();
    //         console.log(counter);
    //     }
    // };
}

function getParameterByName(name) {
    const url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function clearAllCookies() {
    var cookies = document.cookie.split("; ");
    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i];
        var eqPos = cookie.indexOf("=");
        var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
        document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
    }
}

function logout() {
    fetch(`https://localhost:44375/logout`, {
        method: "POST",
    })
        .then(response => {
            if (response.ok) {
                clearAllCookies(); // Call clearAllCookies() if the fetch request is successful
                window.location.href = "../UserPage/guest-sign-in.html";
            } else {
                console.error("Failed to logout");
            }
        })
        .catch(error => {
            console.error("Error:", error);
        });
}


async function bookingHomestay() {
    var cusDateIn = document.getElementById('date-in').value;
    var cusDateOut = document.getElementById('date-out').value;
    var guestIdCookie = getCookie("UserId");
    var homeId = getParameterByName('Id');

    console.log(guestIdCookie);
    console.log(homeId);
    console.log(priceHome);

    const response = await fetch('https://localhost:44375/Transaction/Pay', {
        method: 'POST',
        headers: {
            "Content-Type": "application/json",
            // 'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: JSON.stringify({
            guestId: guestIdCookie,
            homstayId: homeId,
            checkinTime: null,
            checkoutTime: null,
            bookingFrom: cusDateIn,
            bookingTo: cusDateOut,
            totalCost: priceHome,
            bookingDate: null,
            bookingPhone: null
        })
    });
    const homeData = await response.json();
    console.log(homeData, {
        guestId: guestIdCookie,
        homstayId: homeId,
        checkinTime: null,
        checkoutTime: null,
        bookingFrom: cusDateIn,
        bookingTo: cusDateOut,
        totalCost: 10,
        bookingDate: null,
        bookingPhone: null
    });
}


document.addEventListener("DOMContentLoaded", fetchData);







