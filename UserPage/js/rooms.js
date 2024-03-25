let productData;
let currentPage = 1;
const itemsPerPage = 6;

function fetchData() {
    var check = getParameterByName('filter');
    var country = getParameterByName('country'); 
    var city = getParameterByName('city'); 
    var region = getParameterByName('region'); 
    var room = getParameterByName('room'); 

    if (check == null &&  country==null && city == null && region==null ) {
        fetch("https://localhost:44375/api/Homestays")
            .then(response => response.json())
            .then(data => {
                productData = data;
                console.log(data)
                fillTable();
                renderPagination();
            })
    } else if(check!=null) {
        fetch(`https://localhost:44375/api/Filter/filterType/${check}`)
            .then(response => response.json())
            .then(data => {
                productData = data;
                console.log(data)
                fillTable();
                renderPagination();
            })
    }else{
        fetch(`https://localhost:44375/api/Filter/Search`,{
            method: 'POST',
            headers:{
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                homestayCountry: country,
                homestayCity: city ,
                homestayRegion: region,
                homestayBedroom: room,
            })
        })
            .then(response => response.json())
            .then(data => {
                productData = data;
                console.log(data)
                fillTable();
                renderPagination();
            })
    }

};


async function fillTable() {
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = currentPage * itemsPerPage;
    var tableBody = document.getElementById("tableBody");
    tableBody.innerHTML = '';
    for (let i = startIndex; i < endIndex && i < productData.length; i++) {
        const item = productData[i];

        const room = document.createElement("div");
        room.className = "col-lg-4 col-md-6";
        var imgElm = document.createElement("img");

        // Fetch the image data for the current room
        const imageData = await fetch(`https://localhost:44375/Image/homestayId/${item.homestayId}`)
            .then(response => response.json())
            .then(data => data.image1)
            .catch(error => {
                console.error('Error fetching image data:', error);
                return null; // Handle error gracefully
            });
        const type_Name = await fetch(`https://localhost:44375/api/Homestays/getByTypeId?id=${item.homestayType}`)
            .then(response => response.json())
            .then(data => {
                console.log(data.typeName);
                return data.typeName;
            });
        // // If image data is available, set the src attribute of the image element
        if (imageData) {
            imgElm.src = "data:image/jpeg;base64," + imageData;
            imgElm.alt = "Room Image"; // Set alt text for accessibility
        } else {
            imgElm.src = "placeholder.jpg"; // Provide a placeholder image or handle the absence of image data
            imgElm.alt = "Image Not Available"; // Alt text for placeholder image
        }

        room.innerHTML = `
                        <div class="room-item">
                            
                            <div class="ri-text">
                            <div  style="height: 72px">
                            <h4 class="text-limt-2">${item.homestayName}</h4>
                            </div>
                                <h3>${item.homestayPrice}<span>/1 Đêm</span></h3>
                                <table>
                                <tbody>
                                    <tr style="height: 48px">
                                        <td class="r-o">Loại:</td>
                                        <td class="text-limt-2">${type_Name}</td>
                                    </tr>
                                    <tr style="height: 72px">
                                        <td class="r-o">Địa chỉ:</td>
                                        <td class="text-limt-2">${item.homestayStreet}, ${item.homestayRegion}, ${item.homestayCity}</td>
                                    </tr>
                                    <tr style="height: 48px">
                                        <td class="r-o">Số giường:</td>
                                        <td class="text-limt-2">${item.homestayBedroom}</td>
                                    </tr>
                                </tbody>
                                </table>
                                <a href="room-details.html?Id=${item.homestayId}" class="primary-btn" room-id="${item.homestayId}">Xem Chi Tiết</a>
                            </div>
                        </div>
                    `;

        room.querySelector('.room-item').prepend(imgElm);
        tableBody.appendChild(room);
    }
}
function renderPagination() {
    const totalPages = Math.ceil(productData.length / itemsPerPage);
    const pagination = document.querySelector('.pagination ul');
    pagination.innerHTML = ''; // Clear existing pagination
    for (let i = 1; i <= totalPages; i++) {
        const li = document.createElement('li');
        li.classList.add('numb');
        if (currentPage === i) {
            li.classList.add('active');
        }
        li.innerHTML = `<span>${i}</span>`;
        li.addEventListener('click', () => {
            currentPage = i;
            fillTable();
            renderPagination();
        });
        pagination.appendChild(li);
    }
}



function clearAllCookies() {
    var cookies = document.cookie.split("; ");
    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i];
        var eqPos = cookie.indexOf("=");
        var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
        document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=127.0.0.1; Secure";
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
function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}


document.addEventListener("DOMContentLoaded", fetchData);

